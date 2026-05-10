using UnityEngine;

namespace DialogSystem.Dialogue
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Dialogue System/Character")]
    public class CharacterSO : ScriptableObject
    {
        public string characterName;
        public Color nameBGColor = Color.white;
        public Color nameTextColor = Color.black;
        public DialogueSO[] dialogues;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                Debug.LogWarning($"[{name}] Character name is empty.", this);
            }
            if (dialogues == null || dialogues.Length == 0)
            {
                Debug.LogWarning($"[{name}] No dialogues assigned.", this);
                return;
            }
            for (int i = 0; i < dialogues.Length; i++)
            {
                if (dialogues[i] == null)
                {
                    Debug.LogWarning($"[{name}] Dialogue slot {i + 1} is missing (null).", this);
                }
            }
        }
    }
}
