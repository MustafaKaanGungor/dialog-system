using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DialogSystem.Dialogue
{
    public class TypewriterEffect : MonoBehaviour
    {
        public bool CurrentlyWriting {get; private set;} = false;
        public event Action Completed;

        [SerializeField] private float _charactersPerSecond = 20;
        [SerializeField] private float _punctuationDelay = 0.5f;

        private Coroutine typewriterCoroutine;
        private WaitForSeconds simpleDelay;
        private WaitForSeconds punctuationDelay;

        private List<TagCommand> tagCommands;

        public event Action<Emotion> EmotionTriggered;

        private void Awake() {
            simpleDelay = new WaitForSeconds(1 / _charactersPerSecond);
            punctuationDelay = new WaitForSeconds(_punctuationDelay);
        }

        public void StartTyping(TMP_Text text, List<TagCommand> commands)
        {
            if(typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }

            tagCommands = commands;
            simpleDelay = new WaitForSeconds(1 / _charactersPerSecond);
            punctuationDelay = new WaitForSeconds(_punctuationDelay);

            typewriterCoroutine = StartCoroutine(Typewrite(text));
        }

        public void Skip(TMP_Text text)
        {
            if(typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            text.maxVisibleCharacters = text.textInfo.characterCount;

            foreach (TagCommand item in tagCommands)
            {
                switch(item.TagName)
                {
                    case "emotion":
                        if(Enum.TryParse(item.Value, true, out Emotion emotion))
                        {
                            EmotionTriggered?.Invoke(emotion);
                        }
                    break;
                } 
            }

            CurrentlyWriting = false;
            typewriterCoroutine = null;
            Completed?.Invoke();
        }

        private IEnumerator Typewrite(TMP_Text text) 
        {
            text.ForceMeshUpdate();
            TMP_TextInfo textInfo = text.textInfo;
            CurrentlyWriting = true;
            int currentlyVisibleCharacterIndex = 0;

            while(currentlyVisibleCharacterIndex < textInfo.characterCount)
            {
                char character = textInfo.characterInfo[currentlyVisibleCharacterIndex].character;

                text.maxVisibleCharacters++;

                foreach (TagCommand item in tagCommands)
                {
                    if(item.CharIndex == currentlyVisibleCharacterIndex)
                    {
                        switch(item.TagName)
                        {
                            case "emotion":
                                if(Enum.TryParse(item.Value, true, out Emotion emotion))
                                {
                                    EmotionTriggered?.Invoke(emotion);
                                }
                            break;
                            case "speed":
                                if(float.TryParse(item.Value, out float speed))
                                {
                                    simpleDelay = new WaitForSeconds(1 / speed);
                                }
                            break;
                            case "pause":
                                if(float.TryParse(item.Value, out float amount))
                                {
                                    yield return new WaitForSeconds(amount);
                                }
                            break;
                            default:
                                Debug.LogError("invalid tag:"+ item.TagName);
                            break;
                        }

                        break;
                    }
                }

                if(character == '?' || character == '.' || character == ',' ||
                    character == ';' || character == '!' || character == '-')
                {
                    yield return punctuationDelay;
                } else
                {
                    yield return simpleDelay;
                }
                
                currentlyVisibleCharacterIndex++;
            }

            CurrentlyWriting = false;
            Completed?.Invoke();
        }
    }
}
