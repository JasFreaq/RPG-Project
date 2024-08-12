using System.Collections.Generic;
using Campbell.InventorySystem;
using UnityEngine;

namespace Campbell.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Campbell/Quest")]
    public class Quest : ScriptableObject
    {
        [System.Serializable]
        public struct Objective
        {
            public string reference;
            public string description;
        }

        [System.Serializable]
        public struct Reward
        {
            public int number;
            public InventoryItem item;
        }

        [SerializeField] private string _questDescription;
        [SerializeField] private string _questGoal;
        [SerializeField] private List<Objective> _objectives = new List<Objective>();
        [SerializeField] private List<Reward> _rewards = new List<Reward>();

#if UNITY_EDITOR

        public string QuestDescription { set => _questDescription = value; }

        public string QuestGoal { set => _questGoal = value; }

        public void AddObjective(string reference, string description)
        {
            _objectives.Add(new Objective { reference = reference, description = description });
        }

        public void AddReward(int number, InventoryItem item)
        {
            _rewards.Add(new Reward { number = number, item = item });
        }
#endif

        public static Quest FindQuest(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }

            return null;
        }

        public IReadOnlyList<Objective> Objectives
        {
            get { return _objectives; }
        }

        public IReadOnlyList<Reward> Rewards
        {
            get { return _rewards; }
        }

        public bool ContainsObjective(string objectiveRef)
        {
            foreach (Objective objective in _objectives)
            {
                if (objective.reference == objectiveRef)
                    return true;
            }

            return false;
        }
    }
}
