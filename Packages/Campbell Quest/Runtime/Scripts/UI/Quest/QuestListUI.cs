﻿using Campbell.Quests;
using UnityEngine;

namespace Campbell.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private QuestItemUI _questPrefab;
        [SerializeField] private Transform _questContainer;

        private QuestList _questList;

        private void Awake()
        {
            _questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }

        private void OnEnable()
        {
            if (_questList)
            {
                _questList.RegisterOnQuestUpdate(UpdateQuests);
            }
        }

        private void OnDisable()
        {
            if (_questList)
            {
                _questList.DeregisterOnQuestUpdate(UpdateQuests);
            }
        }

        private void UpdateQuests()
        {
            foreach (Transform child in _questContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (QuestStatus status in _questList.QuestStatuses)
            {
                QuestItemUI questItemInstance = Instantiate(_questPrefab, _questContainer);
                questItemInstance.Setup(status);
            }
        }
    }
}