using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class QuantumClicker : MonoBehaviour
{
    [Header("Server")]
    public string serverUrl = "http://localhost:5050/measure";

    [Header("Visual")]
    public Material cubeMaterial;
    public Color colorZero = Color.red;
    public Color colorOne = Color.blue;

    [Header("State")]
    private int clickCount = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == this.transform)
                {
                    StartCoroutine(RequestMeasurement());
                }
            }
        }
        // 按空格键模拟量子结果
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool fakeResult = Random.value > 0.5f;
            cubeMaterial.color = fakeResult ? colorOne : colorZero;
            Debug.Log("[TEST] Fake collapse: " + (fakeResult ? "|1⟩" : "|0⟩"));
            clickCount++;
            CollapseLogger.Instance?.Log(clickCount, fakeResult ? "1" : "0", "TEST_MODE");
        }
    }

    IEnumerator RequestMeasurement()
    {
        // GET 请求，服务器返回一个 JSON: {"result": "0"} 或 {"result": "1"}
        UnityWebRequest request = UnityWebRequest.Get(serverUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            // 简单解析（不依赖外部库）
            bool isOne = json.Contains("\"1\"");
            bool isZero = json.Contains("\"0\"");

            if (isOne)
            {
                cubeMaterial.color = colorOne;
                Debug.Log("[Collapse] Result: |1⟩ → BLUE");
            }
            else if (isZero)
            {
                cubeMaterial.color = colorZero;
                Debug.Log("[Collapse] Result: |0⟩ → RED");
            }

            clickCount++;
            CollapseLogger.Instance?.Log(clickCount, isOne ? "1" : "0", serverUrl);
        }
        else
        {
            Debug.LogError("[QuantumClicker] Server not reachable. Is Python server running?");
        }
    }
}
