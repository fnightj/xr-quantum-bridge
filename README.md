# xr-quantum-bridge

Prototype bridge connecting Qiskit quantum backends to Unity for immersive XR art experiences.  
Part of my PhD research proposal: **"Collapse as Interaction – Quantum computation as a material for immersive and participatory art"**  

---

## About

This repository contains a client-server architecture that sends quantum measurement results from Qiskit simulators (or real hardware) into **Unity**, enabling real-time spatial and sonic interactions driven by quantum randomness and entanglement.

The server (Python/Qiskit) handles circuit execution and shot generation. The Unity client receives shot data via WebSocket and drives scene objects (color, position, scale, audio). This prototype directly supports the **"Collapse Engine"** concept described in my research proposal.

Current status: **working prototype** – functional Unity ↔ Qiskit communication with example scenes.

---

## Why Unity?

As a lecturer teaching *Real-time Virtual Reality Technology* and *Online Game Development*, Unity is my primary development environment. This bridge is designed to integrate seamlessly into Unity workflows, making quantum-driven XR art accessible to artists and designers familiar with the engine.

---

## Repository Structure


xr-quantum-bridge/
├── python_server/                 # Qiskit backend server
│   ├── server.py                  # Flask-SocketIO server, accepts circuit params, returns shots
│   ├── circuit_bank.py            # Predefined circuits (Bell state, GHZ, random rotation)
│   ├── shot_pool.py               # Buffered shot generation for low-latency delivery
│   └── requirements.txt           # qiskit, flask-socketio, eventlet, numpy
│
├── unity_client/                  # Unity project (core scripts)
│   ├── Scripts/
│   │   ├── QuantumBridge.cs       # Main manager: WebSocket connection, shot parsing, event dispatch
│   │   ├── QuantumReceiver.cs     # Receives JSON shot data, fires UnityEvents
│   │   ├── CubeReactor.cs         # Demo: changes color/size based on measurement outcome
│   │   ├── AudioReactor.cs        # Demo: maps qubit state to pitch/volume/pan
│   │   ├── SceneSwitcher.cs       # Cycles through demo scenes (Collapse Room / Entanglement Room)
│   │   └── UI_ShotDisplay.cs      # On-screen display of current shot and circuit info
│   ├── Scenes/
│   │   ├── Demo_CubeReaction.unity       # Basic: click → get shot → cube responds
│   │   ├── Demo_SpatialAudio.unity       # 3D audio sources driven by Bell pair measurements
│   │   └── Demo_EntanglementVisual.unity # Two linked objects showing correlated behavior
│   ├── Prefabs/
│   │   ├── QuantumCube.prefab
│   │   └── QuantumAudioSource.prefab
│   └── README.md                  # Unity-specific setup guide
│
├── docs/
│   ├── ARCHITECTURE.md            # System design overview
│   ├── MAPPING_GRAMMAR.md         # Detailed qubit → spatial/sonic parameter mappings
│   └── SETUP_GUIDE.md             # Step-by-step installation for both server and Unity
│
└── README.md                      # This file


---

## Getting Started

### Prerequisites
- Python 3.9+
- Unity 2022 LTS or newer (with Netcode for GameObjects if needed)
- Git

### Quick Start (Full Pipeline)

#### 1. Start the Python Server
bash
cd python_server
pip install -r requirements.txt
python server.py

Server starts on `ws://localhost:5050` by default. You should see:

[*] Quantum Bridge Server running on ws://localhost:5050
[*] Default circuit: Bell state (2 qubits), 1024 shots


#### 2. Open the Unity Project
- Open `unity_client/` as a Unity project (or create a new project and import the Scripts folder)
- In the Hierarchy, create an empty GameObject named `QuantumBridge`
- Attach `QuantumBridge.cs` to it
- Set the Server URL to `ws://localhost:5050`

#### 3. Run a Demo Scene
- Open `Demo_CubeReaction.unity`
- Press Play
- Click the cube → it sends a circuit request to the server → the server returns a shot → the cube changes color (red for |0⟩, blue for |1⟩)
- Each click produces a different result due to quantum randomness

### What You Should See
A cube that changes color unpredictably with each click – true quantum randomness, not pseudo-random. The same pipeline can drive audio pitch, object scale, particle systems, or any Unity component.

---

## Core Components Explained

### QuantumBridge.cs
- Manages WebSocket connection lifecycle
- Sends JSON requests: `{"circuit": "bell", "shots": 1}`
- Receives JSON responses: `{"status": "success", "shot": "01", "counts": {"00": 512, "11": 512}}`
- Dispatches UnityEvents that other scripts can listen to

### Shot Pool Architecture
To reduce latency, the server pre-generates a batch of shots (default: 100) and stores them in a buffer. The Unity client pulls from this buffer instantly. When the buffer runs low, the server refills it in the background.


[Unity] --request--> [Server Buffer] --instant--> [Unity receives shot]
                          |
                    [Background refill]


This means the first request may take ~50ms (simulator time), but subsequent requests are <5ms.

### Mapping Grammar (docs/MAPPING_GRAMMAR.md)
Defines how quantum measurement outcomes map to Unity parameters:

| Qubit State | Visual Mapping | Audio Mapping |
|-------------|----------------|---------------|
| \|0⟩ (ground) | Red, small scale (0.5) | Low pitch (C3), left pan |
| \|1⟩ (excited) | Blue, large scale (1.5) | High pitch (C5), right pan |
| Entangled pair | Correlated colors | Stereo width expands |

Full grammar documented in `docs/MAPPING_GRAMMAR.md`.

---

## Relation to Research Proposal

| RP Concept | Bridge Implementation |
|------------|----------------------|
| Collapse Engine | `QuantumBridge.cs` + `shot_pool.py` = real-time collapse event loop |
| Shot pool (low-latency buffer) | `shot_pool.py` pre-generates 100 shots, serves on demand |
| Spatial-sonic mapping | `MAPPING_GRAMMAR.md` + `AudioReactor.cs` |
| Three artworks (Collapse Room, Entanglement Room, Growing Circuit) | Corresponding demo scenes in `unity_client/Scenes/` |

---

## Teaching Integration

This bridge is also designed for classroom use. Students in my *Real-time VR Technology* course can:
- Understand quantum measurement through immediate visual feedback
- Modify mapping grammars to create their own quantum-driven art
- Learn client-server architecture in a creative context

Future plans include a simplified Unity package with no Python setup required (using a cloud-hosted simulator).

---

## Current Limitations

- **Simulator only**: Real IBM hardware adds 5-30 second latency; shot pool mitigates but doesn't eliminate
- **Single-user**: No multi-client synchronization yet
- **No error mitigation**: Raw measurement results only
- **Security**: Designed for localhost experimentation only; not network-safe

---

## Roadmap

- [x] Python server with basic circuit execution
- [x] Unity WebSocket client with shot reception
- [x] Three demo scenes (cube, audio, entanglement)
- [ ] Unreal Engine blueprint support (future)
- [ ] Cloud-hosted simulator option for students
- [ ] Real IBM backend with fallback to simulator

---

## License

MIT – free to use, adapt, and share.

---

## Contact

Jing Fu – [fu2en1@icloud.com](mailto:fu2en1@icloud.com)  
Lecturer, Guangzhou Institute of Technology  
PhD applicant, HKUST(GZ) xQAP group  
GitHub: [github.com/fnightj](https://github.com/fnightj)
