using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class DialogueSO : ScriptableObject
    {
        [TextArea(3, 10)]
        public List<string> dialogueLines;
    }
}
