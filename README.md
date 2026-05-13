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

```
┌──────────────────────────────────────────────────────────┐
│                DialogueEventChannelSO                     │
│                  (ScriptableObject)                        │
│  OnNPCInteracted | OnContinueInteraction                  │
│  OnStoppedInteraction | OnEmotionTriggered                │
└────┬──────────────┬──────────────┬──────────────────┬─────┘
     │              │              │                  │
     ▼              ▼              ▼                  ▼
┌──────────┐ ┌──────────────┐ ┌────────────┐ ┌───────────┐
│   NPC    │ │DialogueManager│ │   Camera   │ │  Player   │
│(Producer)│ │  (Consumer)   │ │ Controller │ │(Consumer) │
└──────────┘ └──────────────┘ └────────────┘ └───────────┘
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
│   │   ├── DialogueSO.cs                 — Dialogue lines asset
│   │   ├── DialogueManager.cs            — State machine orchestrator
│   │   ├── DialogueBoxView.cs            — Animated UI panel
│   │   ├── TypewriterEffect.cs           — Per-char text reveal
│   │   └── TagProcessor.cs               — Inline tag parser
│   └── Gameplay/
│       ├── Player.cs                     — FPS movement + interaction
│       ├── NPC.cs                        — Interactable NPC + emotions
│       ├── NPCHeadTracking.cs            — Animation Rigging aim
│       ├── CameraController.cs           — Cinemachine camera swap
│       └── TargetCamController.cs        — (stub)
├── Prefabs/                              — Player, NPC, Canvas, Cameras
├── Data/                                 — ScriptableObject assets
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

1. Right-click in the Project window → **Create → Dialogue System → Character / Dialogue**.
2. Add dialogue lines to a `DialogueSO` asset.
3. Assign `DialogueSO` references to a `CharacterSO`.
4. Drag the `CharacterSO` onto an NPC prefab in the Inspector.

### Inline Tags

```
<emotion=happy>    Triggers NPC "happy" animation
<emotion=angry>    Triggers NPC "angry" animation
<speed=50>         Sets typewriter speed (chars/sec)
<pause=0.5>        Pauses typing for 0.5 seconds
```

Standard Unity rich text (`<color=#00FF00>`, `<b>`, `<i>`, `<size=>`) passes through the parser untouched.

---

## Key Scripts

| Script | What It Does |
|---|---|
| [`DialogueEventChannelSO.cs`](./Assets/Scripts/Core/DialogueEventChannelSO.cs) | ScriptableObject event bus — the backbone of decoupled communication |
| [`DialogueManager.cs`](./Assets/Scripts/Dialogue/DialogueManager.cs) | Finite state machine orchestrating the full dialogue flow |
| [`TagProcessor.cs`](./Assets/Scripts/Dialogue/TagProcessor.cs) | Parses inline tags from dialogue text, returns clean text + command list |
| [`TypewriterEffect.cs`](./Assets/Scripts/Dialogue/TypewriterEffect.cs) | Coroutine-based character-by-character reveal with tag execution |
| [`NPC.cs`](./Assets/Scripts/Gameplay/NPC.cs) | Implements `IInteractable`, listens for emotion events, drives animator |
| [`Player.cs`](./Assets/Scripts/Gameplay/Player.cs) | Capsule-cast FPS movement + raycast interaction with `IInteractable` |
| [`CameraController.cs`](./Assets/Scripts/Gameplay/CameraController.cs) | Swaps Cinemachine cameras and manages `CinemachineTargetGroup` |
| [`NPCHeadTracking.cs`](./Assets/Scripts/Gameplay/NPCHeadTracking.cs) | `MultiAimConstraint`-based head tracking with smooth lerping |
| [`DialogueBoxView.cs`](./Assets/Scripts/Dialogue/DialogueBoxView.cs) | DOTween-animated show/hide with character name/color configuration |
| [`GameInput.cs`](./Assets/Scripts/Core/Input/GameInput.cs) | Singleton wrapping the new Input System actions |
| [`BendingManager.cs`](./Assets/Scripts/BendingManager.cs) | URP world bending via `RenderPipelineManager` callbacks |

