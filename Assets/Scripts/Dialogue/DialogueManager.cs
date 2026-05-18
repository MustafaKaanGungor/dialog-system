using TMPro;
using UnityEngine;
using DialogSystem.Core;
using DialogSystem.Gameplay;
using System.Collections.Generic;

namespace DialogSystem.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private enum DialogueState { Inactive, Typing, WaitingForInput }
        private Dictionary<string, RuntimeDialogueNode> nodeLookup = new Dictionary<string, RuntimeDialogueNode>();
        [SerializeField] private DialogueEventChannelSO dialogueEventChannel;
        [SerializeField] private DialogueBoxView dialogueBoxView;
        [SerializeField] private TypewriterEffect typewriterEffect;
        [SerializeField] private TextMeshProUGUI speechText;

        private DialogueState state = DialogueState.Inactive;
        private RuntimeDialogueGraph currentGraph;
        private RuntimeDialogueNode currentNode;

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
                    state = DialogueState.WaitingForInput;
                    break;

                case DialogueState.WaitingForInput:
                    if (currentNode.choices != null && currentNode.choices.Count > 0)
                    {
                        List<string> choices = new List<string>();
                        for (int i = 0; i < currentNode.choices.Count; i++)
                        {
                            choices.Add(currentNode.choices[i].ChoiceText);
                        }

                        dialogueBoxView.SetChoiceButtons(choices, choiceIndex =>
                        {
                            string destinationID = currentNode.choices[choiceIndex].DestinationID;
                            if (!string.IsNullOrEmpty(destinationID))
                            {
                                currentNode.NextNodeID = destinationID;
                                AdvanceLine();
                            }
                            else
                            {
                                EndDialogue();
                            }
                        });
                    }
                    else if (!string.IsNullOrEmpty(currentNode.NextNodeID))
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
                    state = DialogueState.WaitingForInput;
                    break;

                case DialogueState.WaitingForInput:
                    if (currentNode.choices != null && currentNode.choices.Count > 0)
                    {
                        List<string> choices = new List<string>();
                        for (int i = 0; i < currentNode.choices.Count; i++)
                        {
                            choices.Add(currentNode.choices[i].ChoiceText);
                        }

                        dialogueBoxView.SetChoiceButtons(choices, choiceIndex =>
                        {
                            string destinationID = currentNode.choices[choiceIndex].DestinationID;
                            if (!string.IsNullOrEmpty(destinationID))
                            {
                                currentNode.NextNodeID = destinationID;
                                AdvanceLine();
                            }
                            else
                            {
                                EndDialogue();
                            }
                        });
                    }
                    else if (!string.IsNullOrEmpty(currentNode.NextNodeID))
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
            currentGraph = character.dialogues[Random.Range(0, character.dialogues.Length)];
            foreach (var node in currentGraph.dialogueNodes)
            {
                nodeLookup[node.NodeID] = node;
            }

            if(string.IsNullOrEmpty(currentGraph.EntyNodeID))
            {
               Debug.LogError("Graph Empty");
               EndDialogue();
            }

            currentNode = nodeLookup[currentGraph.EntyNodeID];
            ParseResult parsedText = TagProcessor.Parse(currentNode.DialogueText);
            speechText.text = parsedText.CleanText;
            speechText.maxVisibleCharacters = 0;
            
            typewriterEffect.StartTyping(speechText, parsedText.Commands);

            state = DialogueState.Typing;
        }

        private void OnTypewriterCompleted()
        {
            state = DialogueState.WaitingForInput;
            if (currentNode.choices != null && currentNode.choices.Count > 0)
                    {
                        List<string> choices = new List<string>();
                        for (int i = 0; i < currentNode.choices.Count; i++)
                        {
                            choices.Add(currentNode.choices[i].ChoiceText);
                        }

                        dialogueBoxView.SetChoiceButtons(choices, choiceIndex =>
                        {
                            string destinationID = currentNode.choices[choiceIndex].DestinationID;
                            if (!string.IsNullOrEmpty(destinationID))
                            {
                                currentNode.NextNodeID = destinationID;
                                AdvanceLine();
                            }
                            else
                            {
                                EndDialogue();
                            }
                        });
                    }
        }

        private void AdvanceLine()
        {
            if(!nodeLookup.ContainsKey(currentNode.NextNodeID))
            {
                EndDialogue();
                return;
            }
            currentNode = nodeLookup[currentNode.NextNodeID];
            ParseResult parsedText = TagProcessor.Parse(currentNode.DialogueText);
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
