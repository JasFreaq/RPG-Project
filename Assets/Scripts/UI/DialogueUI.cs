using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        struct ChoiceBlock
        {
            public TextMeshProUGUI Text;
            public Button Button;
        }

        private PlayerConversationHandler _conversationHandler;

        [SerializeField] private TextMeshProUGUI _speakerNameText;

        [SerializeField] private GameObject _aIResponseBlock;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private Button _nextButton;

        [SerializeField] private GameObject _playerResponseBlock;
        [SerializeField] private GameObject _choicePrefab;
        
        [SerializeField] private Button _quitButton;

        private List<ChoiceBlock> _choiceBlocks = new List<ChoiceBlock>();

        private void Awake()
        {
            _conversationHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversationHandler>();
        }

        void Start()
        {
            if (_conversationHandler)
            {
                _conversationHandler.RegisterOnConversationUpdated(UpdateUI);

                _nextButton.onClick.AddListener(_conversationHandler.Next);
                
                for (int i = 0; i < 3; i++)
                {
                    AddChoice();
                }

                _quitButton.onClick.AddListener(_conversationHandler.Quit);

                UpdateUI();
            }
            else
                Debug.LogError("Either Player not found in Scene nor ConversationHandler not found within Player");
        }
        
        void UpdateUI()
        {
            if (_conversationHandler.IsDialogueActive())
            {
                if (_conversationHandler.IsChoosing)
                {
                    _speakerNameText.text = "Player";
                    SetChoices();

                    _playerResponseBlock.SetActive(true);
                    _aIResponseBlock.SetActive(false);
                }
                else
                {
                    _speakerNameText.text = _conversationHandler.GetSpeakerName();
                    _dialogueText.text = _conversationHandler.GetDialogueText();
                    _nextButton.gameObject.SetActive(_conversationHandler.HasNext());

                    _playerResponseBlock.SetActive(false);
                    _aIResponseBlock.SetActive(true);
                }
            }
            
            gameObject.SetActive(_conversationHandler.IsDialogueActive());
        }

        void SelectChoice(int index)
        {
            _conversationHandler.SelectChoice(index);
        }
        
        private void SetChoices()
        {
            foreach (ChoiceBlock choiceInstance in _choiceBlocks)
            {
                choiceInstance.Button.gameObject.SetActive(false);
                choiceInstance.Button.onClick.RemoveAllListeners();
            }

            IReadOnlyList<DialogueNode> playerChildren = _conversationHandler.GetPlayerChildren();

            for (int i = 0; i < playerChildren.Count; i++)
            {
                if (playerChildren[i] != null)
                {
                    if (!_choiceBlocks[i].Text)
                    {
                        AddChoice();
                    }

                    ChoiceBlock choiceInstance = _choiceBlocks[i];

                    choiceInstance.Text.text = playerChildren[i].Text;
                    int index = i;
                    choiceInstance.Button.onClick.AddListener(delegate{SelectChoice(index);});
                    choiceInstance.Button.gameObject.SetActive(true);

                    _choiceBlocks[i] = choiceInstance;
                }
            }
        }

        private void AddChoice()
        {
            GameObject choiceGameObject = Instantiate(_choicePrefab, _playerResponseBlock.transform);

            ChoiceBlock choiceInstance = new ChoiceBlock
            {
                Text = choiceGameObject.GetComponentInChildren<TextMeshProUGUI>(),
                Button = choiceGameObject.GetComponent<Button>()
            };

            _choiceBlocks.Add(choiceInstance);
        }
    }
}