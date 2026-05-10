# AGENTS.md - Dialog System Unity Project

## Project Overview
Unity 6 dialog system using URP, Cinemachine 3, DOTween, and the new Input System. NPC interactions with dialogue boxes and typewriter effect. AI Navigation and Animation Rigging for NPC behavior. ScriptableObject-based event channel architecture for decoupled communication.

## Project Structure
```
Assets/
  Scripts/
    Core/           IInteractable.cs, DialogueEventChannelSO.cs
    Core/Input/     GameInput.cs, PlayerInputActions.cs
    Dialogue/       CharacterSO.cs, DialogueSO.cs, DialogueBoxView.cs,
                    DialogueManager.cs, TypewriterEffect.cs
    Gameplay/       Player.cs, NPC.cs, NPCHeadTracking.cs,
                    CameraController.cs, TargetCamController.cs
  Plugins/           DOTween (Demigiant DLL)
  QuickOutline/      Third-party outline effect
  Prefabs/
  Scenes/
  Data/              ScriptableObject assets (Character1.asset, etc.)
  Settings/          URP assets
```

## Build Commands
Unity projects compile automatically in the Editor. From command line:

```bash
# Build Windows executable
"C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system" -buildTarget Win64

# Open in Unity Editor
"C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe" -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system"

# Run all tests
"C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system" -runTests -testResults "test-results.xml"
```

## Test Commands
Uses `com.unity.test-framework` 1.6.0 (installed but no tests written yet). Create tests in `Assets/Tests/`.

```bash
# Run a single test class
"C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system" -runTests -testFilter "Namespace.ClassName" -testResults "test-results.xml"
```

In-Editor: **Window > General > Test Runner**.

## Namespaces
- `DialogSystem.Core` – `IInteractable`, `DialogueEventChannelSO`, `GameInput`
- `DialogSystem.Dialogue` – `CharacterSO`, `DialogueSO`, `DialogueManager`, `DialogueBoxView`, `TypewriterEffect`
- `DialogSystem.Gameplay` – `Player`, `NPC`, `CameraController`, `NPCHeadTracking`

## Code Style Guidelines

### File Structure
- One class per file, filename matches class name
- Scripts in `Assets/Scripts/<namespace-dir>/`
- Editor scripts in `Assets/Editor/`
- ScriptableObjects in `Assets/Data/`

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
using DialogSystem.Core;
```

### Naming
| Element | Convention | Examples |
|---|---|---|
| Classes | PascalCase | `DialogueManager`, `CharacterSO` |
| Public fields | PascalCase | `characterData`, `dialogues` |
| [SerializeField] private | camelCase | `dialogueEventChannel`, `moveSpeed` |
| Private fields | camelCase | `currentDialogue`, `lineIndex` |
| Methods | PascalCase | `StartDialogue()`, `HandleMovement()` |
| Parameters | camelCase | `NPC npc`, `CharacterSO characterData` |
| Properties | PascalCase | `public bool CanInteract => true;` |
| Interfaces | `I` prefix | `IInteractable` |
| ScriptableObjects | `SO` suffix | `CharacterSO`, `DialogueSO`, `DialogueEventChannelSO` |

### Formatting
- **Braces**: Prefer Allman (brace on new line). Codebase has mixed Allman/K&R; stay consistent with surrounding file.
```csharp
private void Start()
{
    if (condition)
    {
        DoSomething();
    }
}
```
- **Indentation**: 4 spaces
- **`var`**: Avoid; use explicit types
- **Properties**: Expression-bodied when get-only: `public bool CanInteract => true;`

### Unity-Specific Patterns
- **Lifecycle order**: `Awake()`, `OnEnable()`, `Start()`, `Update()`, `OnDisable()`, `OnDestroy()`
- **Singleton** (GameInput only): `Instance` property + `Awake()` check
```csharp
public static GameInput Instance { get; private set; }
private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
}
```
- **ScriptableObject event channel**: `DialogueEventChannelSO` with `UnityAction` fields and `Raise*()` methods. Subscribe in `OnEnable()`/`Start()`, unsubscribe in `OnDisable()`.
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
- **CreateAssetMenu**: Every ScriptableObject needs `[CreateAssetMenu]` with a `Dialogue System/` path

### Events
- `System.Action` for simple input events (`GameInput.InteractPerformed`)
- `event Action` for component callbacks (`TypewriterEffect.Completed`)
- `UnityAction` for ScriptableObject event channels (`DialogueEventChannelSO`)
- Always unsubscribe in `OnDisable()` (never leak subscriptions)

### Error Handling
- Null check with `if (x != null)` before access
- `TryGetComponent<T>(out T component)` over `GetComponent<T>()` when uncertain
- Null-conditional operator: `eventField?.Invoke()`
- No try-catch in Update loops
- `Debug.Log()`/`Debug.LogWarning()`/`Debug.LogError()` for debugging
- `[TextArea]` attribute on string fields that need multi-line editing

### Comments
- Avoid comments. Omit them unless explaining non-obvious logic.
- TODO format: `// TODO: Implement dialogue branching`

### Third-Party Assets
- **DOTween**: DLL-based, in `Assets/Plugins/Demigiant/`
- **QuickOutline**: In `Assets/QuickOutline/` (Outline.cs component)
- Add packages via Unity Package Manager or edit `Packages/manifest.json`

### Key Packages
- `com.unity.cinemachine` 3.1.6
- `com.unity.inputsystem` 1.14.2
- `com.unity.render-pipelines.universal` 17.0.4
- `com.unity.animation.rigging` 1.3.1
- `com.unity.ai.navigation` 2.0.9
- `com.unity.test-framework` 1.6.0
- `com.unity.ugui` 2.0.0

### VS Code
- `.vscode/settings.json` hides `.meta`, `.prefab`, `.unity`, `.asset` files from explorer
- File nesting: `*.sln` nests `*.csproj`
- File associations: Unity YAML files open as YAML

### Git
- Never commit: `Library/`, `Temp/`, `Logs/`, `.vs/`, `*.csproj`, `*.sln`
- Always commit: `Assets/`, `Packages/`, `ProjectSettings/`
