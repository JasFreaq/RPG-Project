using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
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
            foreach (QuestStatus status in _questList.QuestStatuses)
            {
                QuestItemUI questItemInstance = Instantiate(_questPrefab, _questContainer);
                questItemInstance.Setup(status);
            }
        }
    }
}