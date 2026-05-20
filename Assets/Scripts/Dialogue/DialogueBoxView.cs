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
        [SerializeField] private RectTransform choicePanel;
        [SerializeField] private CanvasGroup choiceCanvasGroup;
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
            choicePanel.gameObject.SetActive(true);

            choiceCanvasGroup.alpha = 0f;
            choicePanel.localScale = Vector3.zero;
            DOTween.Sequence()
                .Append(choiceCanvasGroup.DOFade(1f, 0.2f))
                .Join(choicePanel.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
            
            foreach(string choice in choices)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = choice;

                int index = choices.IndexOf(choice);
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => onChoiceSelected(index));
                button.onClick.AddListener(() => ClearChoiceButtons());
                button.onClick.AddListener(() => HideChoiceButtons());
            }
        }

        public void ClearChoiceButtons()
        {
            foreach(Transform child in choicePanel.transform)
            {
                Destroy(child.gameObject);
            }

        }

        private void HideChoiceButtons()
        {
            DOTween.Kill(choicePanel);
            DOTween.Kill(choiceCanvasGroup);
            choiceCanvasGroup.alpha = 1f;
            choicePanel.localScale = Vector3.one;
            DOTween.Sequence()
                .Append(choiceCanvasGroup.DOFade(0f, 0.15f))
                .Join(choicePanel.DOScale(0f, 0.2f).SetEase(Ease.InBack))
                .OnComplete(() => choicePanel.gameObject.SetActive(false));
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
