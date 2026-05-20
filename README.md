# Dialog System

> A modular, event-driven dialogue system for Unity 6, built with URP, Cinemachine 3, DOTween, and the new Input System. Demonstrates clean architecture, decoupled communication, and polished player-NPC interactions.

![Unity 6000.0.59f2](https://img.shields.io/badge/Unity-6000.0.59f2-000000?logo=unity)
![URP](https://img.shields.io/badge/URP-17.0.4-purple?logo=unity)
![Cinemachine](https://img.shields.io/badge/Cinemachine-3.1-red?logo=unity)
![Input System](https://img.shields.io/badge/Input%20System-1.14-blue?logo=unity)
![DOTween](https://img.shields.io/badge/DOTween-✔-brightgreen)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)

---

## Features

- **Visual Dialogue Graph Editor** — Unity Graph Toolkit-based node editor for authoring branching, non-linear conversations.
- **Branching Dialogues** — `ChoiceNode` with configurable options for player-driven narrative paths.
- **ScriptableObject Event Channel** — fully decoupled event bus. Producers and consumers never reference each other directly.
- **Inline Tag Parser** — embed `<emotion=happy>`, `<speed=50>`, `<pause=0.5>` directly in dialogue lines to trigger NPC animations, change typewriter speed, or insert pauses.
- **Typewriter Effect** — character-by-character text reveal with punctuation-aware delay, skip-to-end, and inline tag processing.
- **NPC Emotion System** — 6 emotions (happy, angry, stretch, dismiss, defeated, nervous) driven by animation triggers from dialogue tags.
- **Cinemachine 3 Camera Blending** — smooth transition from first-person to dialogue camera with NPC target-framing via `CinemachineTargetGroup`.
- **Animation Rigging Head Tracking** — NPCs smoothly track the player using `MultiAimConstraint` with lerped weight and position.
- **DOTween UI Animations** — fade + scale sequences on dialogue box show/hide with easing.
- **URP World Bending** — shader-based environment bending effect using URP render pipeline callbacks.
- **New Input System** — action-map-based input (WASD movement, E interact) via auto-generated `PlayerInputActions`.

---

## Architecture

All inter-component communication flows through a single `DialogueEventChannelSO` — a ScriptableObject asset wired in the Inspector. Producers raise events; consumers subscribe. No direct references between gameplay systems.

Dialogue content is authored as `.dialoguegraph` assets in the visual graph editor. At import time, `DialogueGraphImporter` bakes each graph into a `RuntimeDialogueGraph` ScriptableObject consumed by `DialogueManager` at runtime.

```
┌─────────────────────────────────────────────────────────────┐
│                 DialogueEventChannelSO                       │
│                   (ScriptableObject)                           │
│   OnNPCInteracted | OnContinueInteraction                     │
│   OnStoppedInteraction | OnEmotionTriggered                   │
└────┬──────────────┬──────────────┬──────────────────┬─────────┘
     │              │              │                  │
     ▼              ▼              ▼                  ▼
┌──────────┐ ┌──────────────┐ ┌────────────┐ ┌───────────┐
│   NPC    │ │DialogueManager│ │   Camera   │ │  Player   │
│(Producer)│ │  (Consumer)   │ │ Controller │ │(Consumer) │
└──────────┘ └──────────────┘ └────────────┘ └───────────┘
                                  ▲
                                  │
                    ┌─────────────────────────┐
                    │  DialogueGraphImporter  │
                    │  (.dialoguegraph →      │
                    │   RuntimeDialogueGraph) │
                    └─────────────────────────┘
```

### Project Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── IInteractable.cs              — Interaction contract
│   │   ├── DialogueEventChannelSO.cs     — Event bus (SO asset)
│   │   └── Input/
│   │       ├── GameInput.cs              — Singleton input wrapper
│   │       └── PlayerInputActions.cs     — Auto-generated input map
│   ├── Dialogue/
│   │   ├── CharacterSO.cs                — Character data asset
│   │   ├── DialogueManager.cs            — State machine orchestrator
│   │   ├── DialogueBoxView.cs            — Animated UI panel
│   │   ├── TypewriterEffect.cs           — Per-char text reveal
│   │   └── TagProcessor.cs               — Inline tag parser
│   ├── DialogueGraph/
│   │   ├── RuntimeDialogueGraph.cs       — Runtime dialogue data (SO)
│   │   └── Editor/
│   │       ├── DialogueGraph.cs          — Graph definition & asset creation
│   │       ├── DialogueNodes.cs          — Start, Dialogue, Choice, End nodes
│   │       └── DialogueGraphImporter.cs  — ScriptedImporter (.dialoguegraph → SO)
│   └── Gameplay/
│       ├── Player.cs                     — FPS movement + interaction
│       ├── NPC.cs                        — Interactable NPC + emotions
│       ├── NPCHeadTracking.cs            — Animation Rigging aim
│       ├── CameraController.cs           — Cinemachine camera swap
│       └── TargetCamController.cs        — (stub)
├── Prefabs/                              — Player, NPC, Canvas, Cameras
├── Data/                                 — CharacterSO & .dialoguegraph assets
├── Scenes/                               — MainScene.unity
├── Plugins/Demigiant/                    — DOTween DLL
└── QuickOutline/                         — Outline highlight effect
```

---

## Getting Started

1. Open the project in **Unity 6000.0.59f2**.
2. Open `Assets/Scenes/MainScene.unity`.
3. Press **Play** — walk up to an NPC and press **E**.

### Creating Dialogue

1. **Create a Character** — Right-click in the Project window → **Create → Dialogue System → Character**.
2. **Create a Dialogue Graph** — Select the `CharacterSO` asset and click **Create New Dialogue Graph** in the Inspector, or right-click → **Create → Dialogue Graph**.
3. **Open the Graph** — Double-click the `.dialoguegraph` asset to open the visual editor.
4. **Wire the Graph** — Connect nodes: `StartNode → DialogueNode → (ChoiceNode →) → EndNode`.
5. **Write Dialogue** — Fill the `DialogueLine` port on each `DialogueNode` / `ChoiceNode`. Use inline tags freely.
6. **Save** — The asset auto-imports into a `RuntimeDialogueGraph` via `DialogueGraphImporter`.
7. **Assign** — The new graph is automatically added to the `CharacterSO`'s dialogue list. Drag the `CharacterSO` onto an NPC prefab.

### Inline Tags

```
<emotion=happy>    Triggers NPC "happy" animation
<emotion=angry>    Triggers NPC "angry" animation
<speed=50>         Sets typewriter speed (chars/sec)
<pause=0.5>        Pauses typing for 0.5 seconds
```

Standard Unity rich text (`<color=#00FF00>`, `<b>`, `<i>`, `<size=>`) passes through the parser untouched.

### Graph Node Types

| Node | Inputs | Outputs | Purpose |
|---|---|---|---|
| `StartNode` | — | 1 (`out`) | Entry point of the dialogue graph |
| `DialogueNode` | 1 (`in`) | 1 (`out`) | Single line of dialogue with `DialogueLine` string port |
| `ChoiceNode` | 1 (`in`) | N (`Choice 0…N`) | Branching node; configurable option count, each with a `Choice Text` string port |
| `EndNode` | 1 (`in`) | — | Terminator; ends the dialogue |

---

## Key Scripts

| Script | What It Does |
|---|---|
| [`DialogueEventChannelSO.cs`](./Assets/Scripts/Core/DialogueEventChannelSO.cs) | ScriptableObject event bus — the backbone of decoupled communication |
| [`DialogueManager.cs`](./Assets/Scripts/Dialogue/DialogueManager.cs) | Finite state machine orchestrating the full dialogue flow |
| [`DialogueGraph.cs`](./Assets/Scripts/DialogueGraph/Editor/DialogueGraph.cs) | Graph definition, asset extension, and menu-item creation |
| [`DialogueNodes.cs`](./Assets/Scripts/DialogueGraph/Editor/DialogueNodes.cs) | Node implementations: Start, Dialogue, Choice, End |
| [`DialogueGraphImporter.cs`](./Assets/Scripts/DialogueGraph/Editor/DialogueGraphImporter.cs) | ScriptedImporter that bakes `.dialoguegraph` into `RuntimeDialogueGraph` |
| [`RuntimeDialogueGraph.cs`](./Assets/Scripts/DialogueGraph/RuntimeDialogueGraph.cs) | Runtime ScriptableObject consumed by `DialogueManager` |
| [`TagProcessor.cs`](./Assets/Scripts/Dialogue/TagProcessor.cs) | Parses inline tags from dialogue text, returns clean text + command list |
| [`TypewriterEffect.cs`](./Assets/Scripts/Dialogue/TypewriterEffect.cs) | Coroutine-based character-by-character reveal with tag execution |
| [`NPC.cs`](./Assets/Scripts/Gameplay/NPC.cs) | Implements `IInteractable`, listens for emotion events, drives animator |
| [`Player.cs`](./Assets/Scripts/Gameplay/Player.cs) | Capsule-cast FPS movement + raycast interaction with `IInteractable` |
| [`CameraController.cs`](./Assets/Scripts/Gameplay/CameraController.cs) | Swaps Cinemachine cameras and manages `CinemachineTargetGroup` |
| [`NPCHeadTracking.cs`](./Assets/Scripts/Gameplay/NPCHeadTracking.cs) | `MultiAimConstraint`-based head tracking with smooth lerping |
| [`DialogueBoxView.cs`](./Assets/Scripts/Dialogue/DialogueBoxView.cs) | DOTween-animated show/hide with character name/color configuration |
| [`GameInput.cs`](./Assets/Scripts/Core/Input/GameInput.cs) | Singleton wrapping the new Input System actions |
| [`BendingManager.cs`](./Assets/Scripts/BendingManager.cs) | URP world bending via `RenderPipelineManager` callbacks |
