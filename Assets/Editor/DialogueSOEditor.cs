using DialogSystem.Dialogue;
using UnityEditor;

[CustomEditor(typeof(DialogueSO))]
public class DialogueSOEditor : Editor
{
    private DialogueSO dialogue;

    void OnEnable()
    {
        dialogue = (DialogueSO)target;
    }

    public override void OnInspectorGUI()
    {
        DrawValidationBanner();
        DrawDefaultInspector();
    }

    private void DrawValidationBanner()
    {
        if (dialogue.dialogueLines == null || dialogue.dialogueLines.Count == 0)
        {
            EditorGUILayout.HelpBox("No dialogue lines assigned.", MessageType.Error);
        }
        else
        {
            bool hasEmpty = false;
            for (int i = 0; i < dialogue.dialogueLines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(dialogue.dialogueLines[i]))
                {
                    hasEmpty = true;
                    break;
                }
            }
            if (hasEmpty)
                EditorGUILayout.HelpBox("Contains empty lines.", MessageType.Warning);
        }
    }
}
