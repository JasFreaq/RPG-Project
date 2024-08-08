using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Core;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, IPredicateEvaluable, ISaveable
    {
        private List<QuestStatus> _questStatuses = new List<QuestStatus>();

        private Action _onQuestUpdate;

        private Inventory _inventory;
        private ItemDropper _itemDropper;

        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _itemDropper = GetComponent<ItemDropper>();
        }

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
            if (!HasQuest(quest))
            {
                _questStatuses.Add(new QuestStatus(quest));
                _onQuestUpdate?.Invoke();
            }
        }

        public void ClearQuest(Quest quest, string objectiveRef)
        {
            QuestStatus status = FindQuest(quest);
            if (status != null)
            {
                status.ClearObjective(objectiveRef);
                if (status.IsComplete())
                {
                    GiveReward(quest);
                }

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

        QuestStatus FindQuest(Quest quest)
        {
            foreach (QuestStatus status in _questStatuses)
            {
                if (status.Quest == quest)
                    return status;
            }

            return null;
        }

        bool HasQuest(Quest quest)
        {
            return FindQuest(quest) != null;
        }

        void GiveReward(Quest quest)
        {
            foreach (Quest.Reward reward in quest.Rewards)
            {
                if (!_inventory.AddToFirstEmptySlot(reward.item, reward.number))
                {
                    _itemDropper.DropItem(reward.item, reward.number);
                }
            }
        }

        public bool? CheckCondition(Condition.PredicateType predicate, string[] parameters)
        {
            switch (predicate)
            {
                case Condition.PredicateType.HasQuest:
                    if (parameters.Length == 1)
                    {
                        Quest quest = Quest.FindQuest(parameters[0]);
                        if (quest)
                        {
                            return HasQuest(quest);
                        }

                        Debug.LogError($"Quest of ID '{parameters[0]}' does not exist.");
                    }
                    else
                        Debug.LogError("Parameter list error.");

                    break;
                
                case Condition.PredicateType.CompletedObjective:
                    if (parameters.Length == 2)
                    {
                        Quest quest = Quest.FindQuest(parameters[0]);
                        if (quest)
                        {
                            return FindQuest(quest).HasCleared(parameters[1]);
                        }

                        Debug.LogError($"Quest of ID '{parameters[0]}' does not exist.");
                    }
                    else
                        Debug.LogError("Parameter list error.");

                    break;
                
                case Condition.PredicateType.CompletedQuest:
                    if (parameters.Length == 1)
                    {
                        Quest quest = Quest.FindQuest(parameters[0]);
                        if (quest)
                        {
                            return FindQuest(quest).IsComplete();
                        }

                        Debug.LogError($"Quest of ID '{parameters[0]}' does not exist.");
                    }
                    else
                        Debug.LogError("Parameter list error.");

                    break;
            }
            
            return null;
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in _questStatuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = (List<object>) state;

            _questStatuses.Clear();
            foreach (object obj in stateList)
            {
                _questStatuses.Add(new QuestStatus(obj));
            }
        }
    }
}