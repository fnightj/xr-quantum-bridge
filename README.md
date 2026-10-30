# xr-quantum-bridge

A small experiment connecting a Unity scene to a Python backend that simulates quantum measurement outcomes.

## What it actually does

Click a cube in Unity → HTTP request goes to a local Flask server → server returns "0" or "1" (randomly, standing in for qubit collapse) → cube changes color (red for |0⟩, blue for |1⟩) → each click gets written to a CSV log with timestamp, click number, result, and server info.

That's the whole loop. One interaction, one measurement, one visible result.

## Project structure

- `unity_client/` — Unity project (2022 LTS). Two main scripts: `QuantumClicker.cs` handles the click→request→color change, `CollapseLogger.cs` writes `collapse_log.csv`.
- `python_server/` — Flask app, single endpoint `GET /measure`, currently returns random 0/1. Swap-in point for Qiskit Aer later.
- `docs/` — rough design notes and paper references.

## How to run it

1. `cd python_server && py server.py` (server runs on `http://localhost:5050`)
2. Open `unity_client/` in Unity Hub, open the scene, hit Play
3. Click the cube, watch it change color, check `collapse_log.csv`

## Current status

Built as a minimal proof-of-concept for the xQAP application. The "quantum" part is simulated with `random` right now — the point was to get the full interaction loop working first (Unity → HTTP → server → response → visual feedback → logging). Replacing the random return with an actual Qiskit circuit is the immediate next step.

## About

Built by [Jing], lecturer in Real-time VR Technology and Online Game Development, exploring the intersection of immersive interaction and quantum computing concepts.

---

# xr-quantum-bridge（中文版）

把 Unity 里的交互和"量子测量"概念串起来的一个小实验。

## 实际做了什么

在 Unity 里点一下立方体 → 发 HTTP 请求到本地 Flask 服务器 → 服务器随机返回 "0" 或 "1"（模拟量子坍缩）→ 立方体变颜色（红=|0⟩，蓝=|1⟩）→ 每次点击写一条记录到 CSV（时间戳、点击序号、结果、服务器信息）。

就这一个闭环。一次交互，一次"测量"，一个看得见的反馈。

## 目录结构

- `unity_client/` — Unity 工程（2022 LTS）。核心就两个脚本：`QuantumClicker.cs` 管点击→请求→变色，`CollapseLogger.cs` 管写日志。
- `python_server/` — Flask 服务，一个接口 `GET /measure`，目前用 random 返回 0/1。后面换成 Qiskit Aer 电路的地方已经留好了。
- `docs/` — 随手记的设计想法和参考论文。

## 怎么跑

1. `cd python_server && py server.py`（默认跑在 localhost:5050）
2. Unity Hub 打开 `unity_client/`，进场景，点 Play
3. 点方块，看变色，查 `collapse_log.csv`

## 目前状态

这是为 xQAP 申请做的最小可行性验证。量子部分目前是 random 模拟——先把完整交互链路跑通（Unity → HTTP → 服务器 → 返回 → 视觉反馈 → 记录），再换真电路。下一步就是接 Qiskit。

## 关于

作者 [符婧]，高校讲师（教实时 VR 技术和网络游戏开发），在探索沉浸式交互和量子计算的交叉方向。