using DialogSystem.Dialogue;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using Unity.GraphToolkit.Editor;

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

        if(GUILayout.Button("Create New Dialogue Graph", GUILayout.Height(30)))
        {
            string assetPath = $"Assets/Data/{characterSO.name}_Dia_{characterSO.dialogues.Length + 1}.dialoguegraph";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            GraphDatabase.CreateGraph<DialogueGraph>(assetPath);
            AssetDatabase.ImportAsset(assetPath);
            var runtimegraph = AssetDatabase.LoadAssetAtPath<RuntimeDialogueGraph>(assetPath);
            AssetDatabase.SaveAssets();

            System.Array.Resize(ref characterSO.dialogues, characterSO.dialogues.Length + 1);
            characterSO.dialogues[^1] = runtimegraph;

            EditorUtility.SetDirty(characterSO);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            AssetDatabase.Refresh();
        }
    }
}
