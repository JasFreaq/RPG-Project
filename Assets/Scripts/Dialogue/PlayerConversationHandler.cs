using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversationHandler : MonoBehaviour
    {
        private Dialogue _currentDialogue;
        
        private DialogueNode _currentDialogueNode;

        private AIConversationHandler _currentAIHandler;

        private Action _onConversationUpdated;

        private bool _isChoosing = false;

        public bool IsChoosing
        {
            get { return _isChoosing; }
        }
        
        public void StartDialogue(AIConversationHandler newAIHandler)
        {
            _currentDialogue = newAIHandler.Dialogue;

            _currentDialogueNode = _currentDialogue.DialogueNodes[0];
            TriggerEnterAction();

            _currentAIHandler = newAIHandler;

            _onConversationUpdated?.Invoke();
        }

        public void RegisterOnConversationUpdated(Action action)
        {
            _onConversationUpdated += action;
        }
        
        public void DeregisterOnConversationUpdated(Action action)
        {
            _onConversationUpdated -= action;
        }

        public bool IsDialogueActive()
        {
            return _currentDialogue != null;
        }

        public string GetSpeakerName()
        {
            return _currentDialogueNode.SpeakerName;
        }
        
        public string GetDialogueText()
        {
            return _currentDialogueNode.Text;
        }
        
        public bool IsPlayerSpeaking()
        {
            return _currentDialogueNode.IsPlayerSpeech;
        }

        public IReadOnlyList<DialogueNode> GetPlayerChildren()
        {
            return _currentDialogue.GetPlayerChildrenOfNode(_currentDialogueNode);
        }

        public void SelectChoice(int choiceIndex)
        {
            IReadOnlyList<DialogueNode> playerChildren = _currentDialogue.GetPlayerChildrenOfNode(_currentDialogueNode);
            
            _currentDialogueNode = playerChildren[choiceIndex];
            TriggerEnterAction();
            
            _isChoosing = false;
            Next();
        }

        public void Next()
        {
            IReadOnlyList<DialogueNode> playerChildren = _currentDialogue.GetPlayerChildrenOfNode(_currentDialogueNode);
            if (playerChildren.Count > 0)
            {
                _isChoosing = true;
                TriggerExitAction();
            }
            else
            {
                IReadOnlyList<DialogueNode> aIChildren = _currentDialogue.GetChildrenOfNode(_currentDialogueNode);
                
                TriggerExitAction();
                _currentDialogueNode = aIChildren[Random.Range(0, aIChildren.Count)];
                TriggerEnterAction();
            }

            _onConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return (_currentDialogueNode.ChildrenIDs.Count > 0);
        }

        void TriggerEnterAction()
        {
            if (_currentAIHandler && _currentDialogueNode.OnEnterActions.Count > 0)
            {
                _currentAIHandler.TriggerDialogueAction(_currentDialogueNode.OnEnterActions);
            }
        }

        void TriggerExitAction()
        {
            if (_currentAIHandler && _currentDialogueNode.OnExitActions.Count > 0)
            {
                _currentAIHandler.TriggerDialogueAction(_currentDialogueNode.OnExitActions);
            }
        }

        public void Quit()
        {
            _currentDialogue = null;

            TriggerExitAction();
            _currentDialogueNode = null;

            _currentAIHandler = null;

            _isChoosing = false;

            _onConversationUpdated?.Invoke();
        }
    }
}