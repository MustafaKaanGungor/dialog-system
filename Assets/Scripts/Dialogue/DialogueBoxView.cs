using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace DialogSystem.Dialogue
{
    public class DialogueBoxView : MonoBehaviour
    {
        [SerializeField] private Image nameBG;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panel;
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private GameObject choiceButtonPrefab;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show(CharacterSO characterData)
        {
            gameObject.SetActive(true);

            canvasGroup.alpha = 0f;
            panel.localScale = Vector3.zero;
            DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 0.2f))
                .Join(panel.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
                    
            nameBG.color = characterData.nameBGColor;
            nameText.color = characterData.nameTextColor;
            nameText.text = characterData.characterName;  
        }

        public void SetChoiceButtons(List<string> choices, Action<int> onChoiceSelected)
        {
            ClearChoiceButtons();
            choicePanel.SetActive(true);

            foreach(string choice in choices)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = choice;

                int index = choices.IndexOf(choice);
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => onChoiceSelected(index));
                button.onClick.AddListener(() => ClearChoiceButtons());
            }
        }

        public void ClearChoiceButtons()
        {
            foreach(Transform child in choicePanel.transform)
            {
                Destroy(child.gameObject);
            }

            choicePanel.SetActive(false);
        }

        public void Hide()
        {
            DOTween.Kill(panel);
            DOTween.Kill(canvasGroup);
            canvasGroup.alpha = 1f;
            panel.localScale = Vector3.one;
            DOTween.Sequence()
                .Append(canvasGroup.DOFade(0f, 0.15f))
                .Join(panel.DOScale(0f, 0.2f).SetEase(Ease.InBack))
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
