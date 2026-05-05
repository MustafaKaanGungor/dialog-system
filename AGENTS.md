# AGENTS.md - Dialog System Unity Project

## Project Overview
Unity dialog system project using Universal Render Pipeline (URP), Cinemachine, DOTween, and the new Input System. Implements NPC interactions with dialogue boxes featuring a typewriter effect.

## Build Commands
Unity projects compile automatically in the Editor. To build from command line:

```bash
# Windows - Build the project (requires Unity installation)
"C:\Program Files\Unity\Hub\Editor\2023.3.0f1\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system" -buildTarget Win64

# Open project in Unity Editor
"C:\Program Files\Unity\Hub\Editor\2023.3.0f1\Editor\Unity.exe" -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system"
```

## Test Commands
This project uses Unity Test Framework (`com.unity.test-framework`). Tests should be placed in `Assets/Editor/Tests/` or `Assets/Tests/`.

```bash
# Run all tests via Unity CLI
"C:\Program Files\Unity\Hub\Editor\2023.3.0f1\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system" -runTests -testResults "test-results.xml"

# Run specific test class
"C:\Program Files\Unity\Hub\Editor\2023.3.0f1\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\mkaan\Programming\UnityProjects\dialog-system" -runTests -testFilter "NPC.TestInteraction" -testResults "test-results.xml"
```

In-Editor: Use **Window > General > Test Runner** to run tests interactively.

## Code Style Guidelines

### File Structure
- One MonoBehaviour/ScriptableObject class per file
- File name must match class name exactly
- Place scripts in `Assets/Scripts/` with subdirectories for organization
- Editor scripts go in `Assets/Editor/` (automatically excluded from builds)

### Imports
- Use `using` statements (no fully qualified names)
- Order: System namespaces first, then Unity, then third-party, then project
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// Third-party (DOTween, etc.)
// Project imports (if any)
```

### Naming Conventions
- **Classes**: `PascalCase` (e.g., `DialogueBoxUI`, `CharacterSO`)
- **Public fields**: `PascalCase` (e.g., `public CharacterSO characterData;`)
- **Private fields**: `camelCase` (e.g., `private Vector2 movementInput;`)
- **[SerializeField] fields**: `camelCase` (e.g., `private float moveSpeed = 5f;`)
- **Methods**: `PascalCase` (e.g., `HandleMovement()`, `SetSelectedNPC()`)
- **Parameters**: `camelCase` (e.g., `OnNPCInteracted(NPC npc)`)
- **Local variables**: `camelCase`
- **Constants**: `PascalCase` or `UPPER_SNAKE_CASE`
- **ScriptableObject assets**: Use suffix pattern (e.g., `DialogueSO`, `CharacterSO`)

### Formatting
- **Braces**: Allman style (opening brace on new line)
```csharp
private void Start()
{
    // code
}
```
- **Indentation**: 4 spaces (Unity default)
- **Line length**: Keep under 120 characters when possible
- **Spacing**: Space after keywords (`if`, `while`, `for`), around operators
- **Empty lines**: One between methods, group related code with single blank lines

### Types
- Use `var` sparingly - prefer explicit types for clarity in Unity code
- Use `float` for Unity math (not `double`)
- Use `Vector2`, `Vector3`, `Quaternion` for spatial data
- Use `Coroutine` and `IEnumerator` for async/timed operations
- Use C# properties for public access: `public bool CurrentlyWriting { get; private set; }`

### Unity-Specific Patterns
- **Lifecycle methods**: `Awake()`, `Start()`, `Update()`, `OnDestroy()` - keep in this order
- **Singleton pattern**: Use lazy initialization with `Instance` property and `Awake()` check
```csharp
public static Player Instance { get; private set; }
private void Awake() {
    if (Instance != null && Instance != this) { Destroy(gameObject); return; }
    else { Instance = this; }
}
```
- **Serialization**: Use `[SerializeField]` for private fields exposed to Inspector
- **Menu items**: Use `[CreateAssetMenu]` for ScriptableObjects
```csharp
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueSO : ScriptableObject
```

### Error Handling
- Use `Debug.Log()`, `Debug.LogWarning()`, `Debug.LogError()` for debugging
- Check for null with `if (object != null)` before accessing
- Use `TryGetComponent(out T component)` instead of `GetComponent<T>()` when appropriate
- Avoid exceptions in Update loops - validate inputs early

### Events and Actions
- Use `System.Action` and `System.Action<T>` for simple events
- Subscribe in `Start()`, unsubscribe in `OnDestroy()` if needed
```csharp
public Action<NPC> NPCInteracted;
Player.Instance.NPCInteracted += OnNPCInteracted;
```

### Comments
- Avoid comments unless explaining non-obvious logic
- Use `//` for single-line comments, `/* */` for multi-line (rare)
- TODO comments: `// TODO: Implement dialogue branching`

### Git Ignore
- Never commit: `Library/`, `Temp/`, `Logs/`, `.vs/`, `*.csproj`, `*.sln`
- Always commit: `Assets/`, `Packages/`, `ProjectSettings/`

### Package Management
- Add packages via Unity Package Manager or edit `Packages/manifest.json`
- Third-party assets go in `Assets/Plugins/` (e.g., DOTween)
