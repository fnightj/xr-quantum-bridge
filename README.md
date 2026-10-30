\# xr-quantum-bridge



A minimal working prototype for my PhD research idea:

\*\*"Collapse as Interaction – Quantum computation as a material for immersive and participatory art."\*\*



This repo is the smallest testable piece of that idea:

\*\*a single quantum measurement, triggered by a physical interaction, that cannot be undone.\*\*



\## What it actually does



1\. You press \*\*Play\*\* in Unity and click the cube in the scene.

2\. Unity sends an HTTP request to a local Python server (`localhost:5050/measure`).

3\. The server returns a measurement result: `"0"` or `"1"`.

4\. The cube changes color — \*\*red for |0⟩, blue for |1⟩\*\*.

5\. The event is appended to a log file (`collapse\\\_log.csv`) with a timestamp.



Each click is treated as a \*\*collapse event\*\*: once the result is in, it is not rolled back.

That is the whole point of the project.



> ⚠️ Status: \\\*\\\*working prototype, built and tested on 2026-10-30\\\*\\\*.

> Simulator-only, single-user, no audio, no shot pooling. See "What's next" below.



\## Why start here



The broader research frames quantum measurement as an \*interaction material\*

(Prepare → Collapse → Record).

This demo implements exactly that loop, at the smallest possible scale:

one qubit, one click, one irreversible result.



\## Repository structure



xr-quantum-bridge/

├── python\_server/

│   ├── server.py           # Flask server, one endpoint: GET /measure

│   └── requirements.txt    # flask

├── Assets/

│   ├── Scripts/

│   │   ├── QuantumClicker.cs    # click → HTTP request → recolor cube

│   │   └── CollapseLogger.cs    # writes every collapse to collapse\_log.csv

│   └── Scenes/

│       └── ClickToCollapse.unity # one cube, one click

└── README.md               # this file



Only files that exist are listed above.



\## How to run



\### 1. Python server



You need Python 3.10+. On this machine `py` is the launcher.



&#x20;   cd python\_server

&#x20;   py -m pip install -r requirements.txt

&#x20;   py server.py



You should see:



&#x20;   Quantum Bridge server running on http://localhost:5050

&#x20;    \* Running on http://127.0.0.1:5050



\*\*Keep this window open.\*\* Don't close it.



> Test it in a browser: http://localhost:5050/measure

> You should get something like `{"result":"0"}` or `{"result":"1"}`.



\### 2. Unity



\- Open this folder as a Unity project (2022 LTS or newer).

\- Open the `ClickToCollapse` scene.

\- Select the \*\*Cube\*\* in the Hierarchy.

\- In the Inspector, under the \*\*Quantum Clicker\*\* component,

&#x20; drag a Material into the \*\*Cube Material\*\* slot.

&#x20; (If you skip this, Unity prints an `UnassignedReferenceException`.)

\- Press \*\*Play ▶\*\*, click the cube.



Expected result: the cube flips red/blue on each click.



\### 3. Check the log



The log is written to the Unity persistent data path as `collapse\\\_log.csv`.

Each line records `timestamp, click\\\_number, result, server`.

This is the \*\*Record\*\* step of Prepare → Collapse → Record.



\## How it falls short (honest limitations)



\- The server currently uses `random.choice`, so the result is \*quantum-like\*,

&#x20; not a real quantum measurement yet.

\- No real IBM Quantum backend, no latency buffering, no multi-user sync,

&#x20; no sound, no mapping grammar, no extra scenes.

\- Tested only in the Unity Editor on Windows, with the server on the same machine.



I'm keeping this list visible on purpose: the point of this repo is to

grow one honest layer at a time.



\## What's next



1\. Replace `random.choice` with a real Qiskit Aer circuit

&#x20;  (`QCircuit.h(0); measure(0,0); shots=1`) — same endpoint, no Unity changes.

2\. Add a second cube wired to the same result, to sketch entanglement visually.

3\. Only after (1) and (2) feel solid: sound mapping, then a shot-pool buffer

&#x20;  to hide simulator latency.



\## Relation to the research proposal



| RP concept | What exists here |

|---|---|

| Measurement as an interaction event | Click → HTTP request → result |

| Collapse (irreversible) | One result per click, never reversed |

| Record | `collapse\\\_log.csv` |

| Spatial mapping | Cube color (red/blue) |



The three artworks in the RP (Collapse Room, Entanglement Room,

Growing Circuit) are future work and are \*\*not\*\* in this repository yet.



\## Author



Jing Fu — October 2026

Part of an early-stage PhD inquiry into quantum measurement as an artistic material.

