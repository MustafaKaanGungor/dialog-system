using TMPro;
using UnityEngine;
using DialogSystem.Core;
using DialogSystem.Gameplay;

namespace DialogSystem.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private enum DialogueState { Inactive, Typing, WaitingForInput }

        [SerializeField] private DialogueEventChannelSO dialogueEventChannel;
        [SerializeField] private DialogueBoxView dialogueBoxView;
        [SerializeField] private TypewriterEffect typewriterEffect;
        [SerializeField] private TextMeshProUGUI speechText;

        private DialogueState state = DialogueState.Inactive;
        private DialogueSO currentDialogue;
        private int lineIndex = 0;
        private NPC currentNPC;

        private void OnEnable()
        {
            dialogueEventChannel.OnNPCInteracted += OnNPCInteracted;
            dialogueEventChannel.OnContinueInteraction += OnContinueInteraction;
            dialogueEventChannel.OnStoppedInteraction += OnStoppedInteraction;
            typewriterEffect.Completed += OnTypewriterCompleted;
            typewriterEffect.EmotionTriggered += OnEmotionTriggered;
        }

        private void OnDisable()
        {
            dialogueEventChannel.OnNPCInteracted -= OnNPCInteracted;
            dialogueEventChannel.OnContinueInteraction -= OnContinueInteraction;
            dialogueEventChannel.OnStoppedInteraction -= OnStoppedInteraction;
            typewriterEffect.Completed -= OnTypewriterCompleted;
            typewriterEffect.EmotionTriggered -= OnEmotionTriggered;
        }

        private void OnNPCInteracted(IInteractable interactable)
        {
            NPC npc = (NPC)interactable;

            switch (state)
            {
                case DialogueState.Inactive:
                    StartDialogue(npc);
                    break;

                case DialogueState.Typing:
                    typewriterEffect.Skip(speechText);
                    break;

                case DialogueState.WaitingForInput:
                    if (lineIndex + 1 < currentDialogue.dialogueLines.Count)
                        AdvanceLine();
                    else
                        EndDialogue();
                    break;
            }
        }

        private void OnContinueInteraction(IInteractable interactable)
        {
            switch (state)
            {
                case DialogueState.Typing:
                    typewriterEffect.Skip(speechText);
                    break;

                case DialogueState.WaitingForInput:
                    if (lineIndex + 1 < currentDialogue.dialogueLines.Count)
                        AdvanceLine();
                    else
                        EndDialogue();
                    break;
            }
        }

        private void StartDialogue(NPC npc)
        {
            currentNPC = npc;
            CharacterSO character = npc.characterData;
            dialogueBoxView.Show(character);
            currentDialogue = character.dialogues[Random.Range(0, character.dialogues.Length)];
            lineIndex = 0;

            ParseResult parsedText = TagProcessor.Parse(currentDialogue.dialogueLines[0]);
            speechText.text = parsedText.CleanText;
            speechText.maxVisibleCharacters = 0;
            
            typewriterEffect.StartTyping(speechText, parsedText.Commands);

            state = DialogueState.Typing;
        }

        private void OnTypewriterCompleted()
        {
            state = DialogueState.WaitingForInput;
        }

        private void AdvanceLine()
        {
            lineIndex++;
            ParseResult parsedText = TagProcessor.Parse(currentDialogue.dialogueLines[lineIndex]);
            speechText.text = parsedText.CleanText;
            speechText.maxVisibleCharacters = 0;
            typewriterEffect.StartTyping(speechText, parsedText.Commands);
            state = DialogueState.Typing;
        }

        private void EndDialogue()
        {
            NPC npcToEnd = currentNPC;
            dialogueBoxView.Hide();
            state = DialogueState.Inactive;
            currentNPC = null;
            dialogueEventChannel.RaiseStoppedInteraction(npcToEnd);
        }

        private void OnStoppedInteraction(IInteractable interactable)
        {
            if (state == DialogueState.Inactive)
                return;

            dialogueBoxView.Hide();
            state = DialogueState.Inactive;
            currentNPC = null;
        }

        private void OnEmotionTriggered(Emotion emotion)
        {
            dialogueEventChannel.RaiseEmotionTriggered(currentNPC, emotion);
        }
    }

        
}
