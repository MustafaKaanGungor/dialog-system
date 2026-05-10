using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public bool CurrentlyWriting {get; private set;} = false;
    public event Action Completed;

    [SerializeField] private float _charactersPerSecond = 20;
    [SerializeField] private float _punctuationDelay = 0.5f;

    private Coroutine typewriterCoroutine;
    private WaitForSeconds simpleDelay;
    private WaitForSeconds punctuationDelay;

    private void Awake() {
        simpleDelay = new WaitForSeconds(1 / _charactersPerSecond);
        punctuationDelay = new WaitForSeconds(_punctuationDelay);
    }

    public void StartTyping(TMP_Text text)
    {
        if(typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        typewriterCoroutine = StartCoroutine(Typewrite(text));
    }

    public void Skip(TMP_Text text)
    {
        if(typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }
        text.maxVisibleCharacters = text.textInfo.characterCount;
        CurrentlyWriting = false;
        typewriterCoroutine = null;
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
