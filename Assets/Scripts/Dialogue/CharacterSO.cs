using UnityEngine;

namespace DialogSystem.Dialogue
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Dialogue System/Character")]
    public class CharacterSO : ScriptableObject
    {
        public string characterName;
        public Color nameBGColor = Color.white;
        public Color nameTextColor = Color.black;
        public RuntimeDialogueGraph[] dialogues;

        private void OnValidate()
        {
            if(dialogues == null || dialogues.Length == 0)
            {
                Debug.LogError("Character has no dialogues assigned", this);
            }
        }
    }
}
