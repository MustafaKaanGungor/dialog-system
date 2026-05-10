using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class DialogueSO : ScriptableObject
    {
        [TextArea(3, 10)]
        public List<string> dialogueLines;

        private void OnValidate()
        {
            if(dialogueLines == null || dialogueLines.Count == 0)
            {
                Debug.LogWarning($"[{name}] Dialogue has no lines assigned.", this);
                return;
            }

            for (int i = 0; i < dialogueLines.Count; i++)
            {
                if(string.IsNullOrWhiteSpace(dialogueLines[i]))
                {
                    Debug.LogWarning($"[{name}] line is empty", this);
                } else
                {
                    dialogueLines[i] = dialogueLines[i].Trim();
                }
            }
        }
    }
}
