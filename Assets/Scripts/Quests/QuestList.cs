using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
    {
        private List<QuestStatus> _questStatuses = new List<QuestStatus>();

        private Action _onQuestUpdate;

        private void Start()
        {
            _onQuestUpdate?.Invoke();
        }

        public IReadOnlyList<QuestStatus> QuestStatuses
        {
            get { return _questStatuses; }
        }

        public void AddQuest(Quest quest)
        {
            if (!ContainsQuest(quest))
            {
                _questStatuses.Add(new QuestStatus(quest));
                _onQuestUpdate?.Invoke();
            }
        }

        public void RegisterOnQuestUpdate(Action action)
        {
            _onQuestUpdate += action;
        }
        
        public void DeregisterOnQuestUpdate(Action action)
        {
            _onQuestUpdate -= action;
        }

        bool ContainsQuest(Quest quest)
        {
            foreach (QuestStatus status in _questStatuses)
            {
                if (status.Quest == quest)
                    return true;
            }

            return false;
        }
    }
}