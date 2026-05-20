# AGENTS.md - Dialog System Unity Project

## Project Overview
Unity 6 dialog system using URP, Cinemachine 3, DOTween, and the new Input System. NPC interactions with dialogue boxes and typewriter effect with inline tag commands (emotion, speed, pause). AI Navigation and Animation Rigging for NPC behavior. ScriptableObject-based event channel architecture for decoupled communication. Includes a Unity Graph Toolkit-based dialogue graph editor and a world-bending shader manager.

## Project Structure
```
Assets/Scripts/
  Core/                   IInteractable.cs, DialogueEventChannelSO.cs
  Core/Input/             GameInput.cs, PlayerInputActions.cs
  Dialogue/               CharacterSO.cs, DialogueSO.cs, DialogueBoxView.cs,
                          DialogueManager.cs, TypewriterEffect.cs, TagProcessor.cs
  DialogueGraph/          RuntimeDialogueGraph.cs (RuntimeDialogueNode, ChoiceData)
  DialogueGraph/Editor/   DialogueGraph.cs, DialogueNodes.cs, DialogueGraphImporter.cs
  Gameplay/               Player.cs, NPC.cs, NPCHeadTracking.cs,
                          CameraController.cs, TargetCamController.cs
  BendingManager.cs       (ExecuteAlways, world-bending shader, global namespace)
Assets/Plugins/Demigiant/ DOTween (DLL)
Assets/QuickOutline/       Outline.cs (third-party)
```

## Build/Test Commands
Unity projects compile automatically in the Editor. From command line:
```bash
# Build Windows executable
"$UNITY_EDITOR" -quit -batchmode -projectPath "." -buildTarget Win64

# Open in Unity Editor
"$UNITY_EDITOR" -projectPath "."

# Run all tests
"$UNITY_EDITOR" -quit -batchmode -projectPath "." -runTests -testResults "test-results.xml"

# Run single test class
"$UNITY_EDITOR" -quit -batchmode -projectPath "." -runTests -testFilter "Namespace.ClassName" -testResults "test-results.xml"
```
Where `$UNITY_EDITOR` = `C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe`. Tests in `Assets/Tests/` using `com.unity.test-framework` 1.6.0. In-Editor: **Window > General > Test Runner**.

## Namespaces
- `DialogSystem.Core` – `IInteractable`, `DialogueEventChannelSO`, `GameInput`
- `DialogSystem.Dialogue` – `CharacterSO`, `DialogueSO`, `DialogueBoxView`, `DialogueManager`, `TypewriterEffect`, `TagProcessor`, `Emotion`, `TagCommand`, `ParseResult`
- `DialogSystem.Gameplay` – `Player`, `NPC`, `CameraController`, `TargetCamController`, `NPCHeadTracking`
- *(global)* – `RuntimeDialogueGraph`, `RuntimeDialogueNode`, `ChoiceData`, `BendingManager`, `DialogueGraph` (Editor), `DialogueNode`, `ChoiceNode`, `StartNode`, `EndNode`, `DialogueGraphImporter`

## Code Style Guidelines

### File Structure
- One class per file, filename matches class name
- Editor scripts in `Assets/Scripts/*/Editor/` (no separate `Assets/Editor/`)
- ScriptableObject assets in `Assets/Data/`, graph assets use `.dialoguegraph` extension

### Imports
- `using` statements only (no fully qualified names)
- Order: System, UnityEngine, Unity modules, third-party, project
```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using DG.Tweening;
using DialogSystem.Core;
```

### Naming
| Element | Convention | Examples |
|---|---|---|
| Classes | PascalCase | `DialogueManager`, `CharacterSO` |
| Public fields | PascalCase | `CharacterData`, `dialogues` |
| [SerializeField] private | camelCase | `dialogueEventChannel`, `moveSpeed` |
| Private fields | camelCase | `currentDialogue`, `lineIndex` |
| Methods | PascalCase | `StartDialogue()`, `HandleMovement()` |
| Parameters | camelCase | `NPC npc`, `CharacterSO characterData` |
| Properties | PascalCase expression-bodied | `public bool CanInteract => true;` |
| Interfaces | `I` prefix | `IInteractable` |
| ScriptableObjects | `SO` suffix | `CharacterSO`, `DialogueSO`, `DialogueEventChannelSO` |
| Enums | PascalCase | `Emotion` |
| Structs | PascalCase | `TagCommand`, `ParseResult`, `ChoiceData` |

### Formatting
- **Braces**: Codebase has mixed K&R and Allman. Stay consistent with the surrounding file. K&R is more common in newer files.
```csharp
// K&R (common):
private void Start() {
    if (condition) {
        DoSomething();
    }
}

// Allman (BendingManager only):
private void OnEnable ()
{
    if ( Application.isPlaying )
        return;
}
```
- **Indentation**: 4 spaces
- **`var`**: Avoid; use explicit types
- **`[SerializeField]`** on separate line before field

### Unity-Specific Patterns
- **Lifecycle order**: `Awake()`, `OnEnable()`, `Start()`, `Update()`, `OnDisable()`, `OnDestroy()`
- **Event subscription**: Prefer `OnEnable()`/`OnDisable()` for event channels. Some scripts use `Start()` for subscribe but always use `OnDisable()` for unsubscribe.
```csharp
private void OnEnable()
{
    dialogueEventChannel.OnNPCInteracted += OnNPCInteracted;
}
private void OnDisable()
{
    dialogueEventChannel.OnNPCInteracted -= OnNPCInteracted;
}
```
- **Singleton** (GameInput only): `Instance` property + `Awake()` DontDestroyOnLoad pattern
- **ScriptableObject event channel**: `DialogueEventChannelSO` with `event UnityAction<...>` fields and `Raise*()` methods calling `?.Invoke()`
- **CreateAssetMenu**: Every ScriptableObject needs `[CreateAssetMenu]` with `Dialogue System/` path

### Tag System (TagProcessor)
Inline tags in dialogue text: `<emotion=happy>`, `<speed=40>`, `<pause=1.5>`. `TagProcessor.Parse()` returns `ParseResult` with `CleanText` and `List<TagCommand>`. `TypewriterEffect` processes commands at matching char indices.

### Dialogue Graph (Editor)
Unity Graph Toolkit-based visual editor (`com.unity.graphtoolkit` 0.4.0-exp.2). Node types: `StartNode`, `EndNode`, `DialogueNode`, `ChoiceNode`. Imports to `RuntimeDialogueGraph` ScriptableObject via `DialogueGraphImporter` with `.dialoguegraph` extension.

### Events
- `System.Action` for simple input events (`GameInput.InteractPerformed`)
- `event Action` for component callbacks (`TypewriterEffect.Completed`, `TypewriterEffect.EmotionTriggered`)
- `UnityAction` for ScriptableObject event channels (`DialogueEventChannelSO`)
- Always unsubscribe in `OnDisable()` (never leak subscriptions)

### Error Handling
- Null check with `if (x != null)` before access
- `TryGetComponent<T>(out T component)` over `GetComponent<T>()` when uncertain
- Null-conditional operator: `eventField?.Invoke()`
- No try-catch in Update loops
- `Debug.Log()`/`Debug.LogWarning()`/`Debug.LogError()` for debugging
- `[TextArea(3,10)]` attribute on string fields needing multi-line editing
- `OnValidate()` for ScriptableObject validation warnings

### Comments
- Avoid comments. Omit them unless explaining non-obvious logic.
- TODO format: `// TODO: Implement dialogue branching`

### Third-Party
- **DOTween**: DLL-based, in `Assets/Plugins/Demigiant/`
- **QuickOutline**: `Assets/QuickOutline/` (Outline.cs component)
- Add packages via Unity Package Manager or edit `Packages/manifest.json`

### Key Packages
- `com.unity.cinemachine` 3.1.6
- `com.unity.inputsystem` 1.19.0
- `com.unity.render-pipelines.universal` 17.3.0
- `com.unity.animation.rigging` 1.4.1
- `com.unity.ai.navigation` 2.0.12
- `com.unity.test-framework` 1.6.0
- `com.unity.ugui` 2.0.0
- `com.unity.graphtoolkit` 0.4.0-exp.2

### VS Code
- `.vscode/settings.json` hides `.meta`, `.prefab`, `.unity`, `.asset` etc. from explorer
- File nesting: `*.sln` nests `*.csproj`
- File associations: Unity YAML files (`*.asset`, `*.meta`, `*.prefab`, `*.unity`) open as YAML

### Git (from .gitignore)
- Never commit: `Library/`, `Temp/`, `Logs/`, `Build/`, `.vs/`, `*.csproj`, `*.sln`, `*.user`, `*.pidb`
- Always commit: `Assets/`, `Packages/`, `ProjectSettings/`
