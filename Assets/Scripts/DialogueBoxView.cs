using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DialogSystem.Dialogue
{
    public class DialogueBoxView : MonoBehaviour
    {
        [SerializeField] private Image nameBG;
        [SerializeField] private TextMeshProUGUI nameText;

        private void Start()
        {
            Hide();
        }

        public void Show(CharacterSO characterData)
        {
            gameObject.SetActive(true);

            nameBG.color = characterData.nameBGColor;
            nameText.color = characterData.nameTextColor;
            nameText.text = characterData.characterName;  
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
