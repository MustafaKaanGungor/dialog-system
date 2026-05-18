using DialogSystem.Dialogue;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(CharacterSO))]
public class CharacterSOEditor : Editor
{
    private CharacterSO characterSO;

    private void OnEnable()
    {
        characterSO = (CharacterSO)target;
    }

    public override void OnInspectorGUI()
    {
        DrawValidationBanner();
        DrawDefaultInspector();
        DrawQuickCreateButton();
    }

    private void DrawValidationBanner()
    {
        if (string.IsNullOrWhiteSpace(characterSO.characterName))
        {
            EditorGUILayout.HelpBox("Character name is empty.", MessageType.Error);
        }

        if (characterSO.dialogues == null || characterSO.dialogues.Length == 0)
        {
            EditorGUILayout.HelpBox("No dialogues assigned.", MessageType.Error);
        } else
        {
            bool hasNull = false;
            for (int i = 0; i < characterSO.dialogues.Length; i++)
            {
                if (characterSO.dialogues[i] == null)
                {
                    hasNull = true;
                    break;
                }
            }

            if(hasNull)
            {
                EditorGUILayout.HelpBox("Some dialogue slots are missing", MessageType.Warning);
            } 
        }
    }

    private void DrawQuickCreateButton()
    {
        EditorGUILayout.Space(5);

        if(GUILayout.Button("Create New Dialogue", GUILayout.Height(30)))
        {
            RuntimeDialogueGraph newDialogue = CreateInstance<RuntimeDialogueGraph>();


            string assetPath = $"Assets/Data/{characterSO.name}_Dia_{characterSO.dialogues.Length + 1}.asset";

            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            AssetDatabase.CreateAsset(newDialogue, assetPath);
            AssetDatabase.SaveAssets();

            System.Array.Resize(ref characterSO.dialogues, characterSO.dialogues.Length + 1);
            characterSO.dialogues[^1] = newDialogue;

            EditorUtility.SetDirty(characterSO);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            AssetDatabase.Refresh();
        }
    }
}
