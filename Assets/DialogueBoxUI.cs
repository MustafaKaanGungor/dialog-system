using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxUI : MonoBehaviour
{
    public static DialogueBoxUI instance {private set; get;}
    [SerializeField] private TextMeshProUGUI speechText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image nameBG;
    private DialogueSO currentDialog;
    private int dialogIndex = 0;
    private bool isShowingDialouge = false;

    private void Awake() {
        if(instance != null)
        {
            Destroy(this);
            return;
        } else
        {
            instance = this;
        }
    }

    private void Start() {
        Player.Instance.NPCInteracted += OnNPCInteracted;
        gameObject.SetActive(false);
    }

    private void OnNPCInteracted(NPC npc)
    {
        if(isShowingDialouge)
        {
            if(dialogIndex - 1 >= currentDialog.dialogueLines.Count)
            {
                isShowingDialouge = false;
                gameObject.SetActive(false);
                Player.Instance.StopInteraction();
            } else
            {
                NextDialogue();
            }
        } else
        {
            ShowDialogue(npc.characterData);
        }
        
    }

    public void ShowDialogue(CharacterSO characterData)
    {
        gameObject.SetActive(true);
        isShowingDialouge = true;
        nameBG.color = characterData.nameBGColor;
        nameText.color = characterData.nameTextColor;
        nameText.text = characterData.characterName;    
        
        currentDialog = characterData.dialogues[UnityEngine.Random.Range(0, characterData.dialogues.Count())];
        dialogIndex = 0;
        speechText.text = currentDialog.dialogueLines[0];
        speechText.maxVisibleCharacters = 0;
        StartCoroutine(WriteText(currentDialog.dialogueLines[0].Length));
    }

    private void NextDialogue()
    {
        StopAllCoroutines();
        dialogIndex++;
        speechText.text = currentDialog.dialogueLines[dialogIndex];
        speechText.maxVisibleCharacters = 0;
        StartCoroutine(WriteText(currentDialog.dialogueLines[dialogIndex].Length));
    }

    private IEnumerator WriteText(int remainingLetters)
    {
        speechText.maxVisibleCharacters = currentDialog.dialogueLines[dialogIndex].Length - remainingLetters;
        yield return new WaitForSeconds(0.02f);
        if(remainingLetters != 0)
        {
            StartCoroutine(WriteText(remainingLetters - 1));
        }
    }
}
