using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxUI : MonoBehaviour
{
    [SerializeField] private Image nameBG;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI speechText;
    
    private DialogueSO currentDialog;
    private int dialogIndex = 0;
    private bool isShowingDialouge = false;

    private Coroutine typewriterCoroutine;
    private int currentlyVisibleCharacterIndex = 0;
    private WaitForSeconds simpleDelay;
    private WaitForSeconds punctuationDelay;
    [SerializeField] private float _charactersPerSecond = 20;
    [SerializeField] private float _punctuationDelay = 0.5f;

    public bool CurrentlyWriting {get; private set;} = false;

    private void Awake() {
        simpleDelay = new WaitForSeconds(1 / _charactersPerSecond);
        punctuationDelay = new WaitForSeconds(_punctuationDelay);
    }

    private void Start() {
        Player.Instance.NPCInteracted += OnNPCInteracted;
        Hide();
    }

    private void OnNPCInteracted(NPC npc)
    {
        if(isShowingDialouge)
        {
            if(CurrentlyWriting)
            {
                Debug.Log("skipiing");
                Skip();
            }
            else if(dialogIndex + 1 >= currentDialog.dialogueLines.Count())
            {
                Player.Instance.StopInteraction();
                Hide();
                
            } else
            {
                Debug.Log("next");
                NextDialogue();
            }
        } else
        {
            SetDialogue(npc.characterData);
        }
    }

    public void SetDialogue(CharacterSO characterData)
    {
        if(typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        Show();
        nameBG.color = characterData.nameBGColor;
        nameText.color = characterData.nameTextColor;
        nameText.text = characterData.characterName;    
        
        currentDialog = characterData.dialogues[UnityEngine.Random.Range(0, characterData.dialogues.Count())];
        dialogIndex = 0;
        speechText.text = currentDialog.dialogueLines[0];
        speechText.maxVisibleCharacters = 0;
        currentlyVisibleCharacterIndex = 0;
        typewriterCoroutine = StartCoroutine(TypeWriter());
    }

    private void NextDialogue()
    {
        dialogIndex++;
        speechText.text = currentDialog.dialogueLines[dialogIndex];
        speechText.maxVisibleCharacters = 0;
        currentlyVisibleCharacterIndex = 0;
        StartCoroutine(TypeWriter());
    }

    private IEnumerator TypeWriter()
    {
        speechText.ForceMeshUpdate();
        TMP_TextInfo textInfo = speechText.textInfo;
        CurrentlyWriting = true;

        while(currentlyVisibleCharacterIndex < textInfo.characterCount)
        {
            char character = textInfo.characterInfo[currentlyVisibleCharacterIndex].character;

            speechText.maxVisibleCharacters++;

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
    }

    private void Skip()
    {
        StopCoroutine(typewriterCoroutine);
        speechText.maxVisibleCharacters = speechText.textInfo.characterCount;
        CurrentlyWriting = false;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        isShowingDialouge = false;
    }

    private void Show()
    {
        gameObject.SetActive(true);
        isShowingDialouge = true;
    }
}
