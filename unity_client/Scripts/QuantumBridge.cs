using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace QuantumBridge
{
    public class QuantumBridge : MonoBehaviour
    {
        [Header("Connection Settings")]
        [SerializeField] private string serverUrl = "ws://localhost:5050";
        [SerializeField] private float reconnectDelay = 2f;

        [Header("Events")]
        public UnityEvent OnConnected;
        public UnityEvent OnDisconnected;
        public UnityEvent<string> OnError;
        public ShotReceivedEvent OnShotReceived;

        [Serializable]
        public class ShotReceivedEvent : UnityEvent<ShotData> { }

        [Serializable]
        public struct ShotData
        {
            public string circuitName;
            public int[] qubitValues;
            public string binaryString;
            public int decimalValue;
        }

        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cts;
        private bool _isConnected;
        private readonly Queue<Action> _mainThreadActions = new Queue<Action>();

        private void Awake()
        {
            if (FindObjectsOfType<QuantumBridge>().Length > 1)
            {
                Debug.LogWarning("Multiple QuantumBridge instances found. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
        }

        private void Start() => Connect();

        private void Update()
        {
            lock (_mainThreadActions)
            {
                while (_mainThreadActions.Count > 0)
                    _mainThreadActions.Dequeue()?.Invoke();
            }
        }

        private void OnDestroy() => Disconnect();

        public async void Connect()
        {
            if (_isConnected) return;
            _cts = new CancellationTokenSource();
            _webSocket = new ClientWebSocket();
            try
            {
                Uri uri = new Uri(serverUrl);
                await _webSocket.ConnectAsync(uri, _cts.Token);
                _isConnected = true;
                Debug.Log($"[QuantumBridge] Connected to {serverUrl}");
                EnqueueMainThreadAction(() => OnConnected?.Invoke());
                _ = ReceiveLoop();
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuantumBridge] Connection failed: {e.Message}");
                EnqueueMainThreadAction(() => OnError?.Invoke(e.Message));
                _isConnected = false;
                Invoke(nameof(Connect), reconnectDelay);
            }
        }

        public void Disconnect()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                _cts?.Cancel();
                _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            }
            _isConnected = false;
            EnqueueMainThreadAction(() => OnDisconnected?.Invoke());
        }

        public async void RequestShot(string circuitName, int numShots = 1)
        {
            if (!_isConnected || _webSocket.State != WebSocketState.Open)
            {
                Debug.LogWarning("[QuantumBridge] Cannot send request: not connected.");
                return;
            }
            string requestJson = $"{{\"circuit\": \"{circuitName}\", \"shots\": {numShots}}}";
            byte[] bytes = Encoding.UTF8.GetBytes(requestJson);
            try
            {
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
                Debug.Log($"[QuantumBridge] Sent: {requestJson}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuantumBridge] Send error: {e.Message}");
                EnqueueMainThreadAction(() => OnError?.Invoke(e.Message));
            }
        }

        private async Task ReceiveLoop()
        {
            byte[] buffer = new byte[4096];
            while (_webSocket.State == WebSocketState.Open && !_cts.IsCancellationRequested)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close) break;
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        ProcessReceivedJson(json);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception e)
                {
                    Debug.LogError($"[QuantumBridge] Receive error: {e.Message}");
                    EnqueueMainThreadAction(() => OnError?.Invoke(e.Message));
                    break;
                }
            }
            _isConnected = false;
            EnqueueMainThreadAction(() => OnDisconnected?.Invoke());
        }

        private void ProcessReceivedJson(string json)
        {
            try
            {
                ResponseData response = JsonUtility.FromJson<ResponseData>(json);
                if (response.status == "success")
                {
                    ShotData shot = new ShotData
                    {
                        circuitName = response.circuit ?? "unknown",
                        binaryString = response.shot,
                        qubitValues = ConvertBinaryStringToArray(response.shot),
                        decimalValue = Convert.ToInt32(response.shot, 2)
                    };
                    EnqueueMainThreadAction(() => OnShotReceived?.Invoke(shot));
                }
                else
                {
                    Debug.LogWarning($"[QuantumBridge] Server returned error: {response.message}");
                    EnqueueMainThreadAction(() => OnError?.Invoke(response.message));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuantumBridge] Failed to parse JSON: {json}\n{e.Message}");
            }
        }

        private int[] ConvertBinaryStringToArray(string binary)
        {
            int[] arr = new int[binary.Length];
            for (int i = 0; i < binary.Length; i++) arr[i] = binary[i] - '0';
            return arr;
        }

        private void EnqueueMainThreadAction(Action action)
        {
            lock (_mainThreadActions) { _mainThreadActions.Enqueue(action); }
        }

        [Serializable]
        private class ResponseData
        {
            public string status;
            public string shot;
            public string circuit;
            public string message;
        }
    }
}