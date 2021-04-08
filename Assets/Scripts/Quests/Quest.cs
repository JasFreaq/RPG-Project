using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "RPG/Quest")]
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

        [SerializeField] private List<Objective> _objectives = new List<Objective>();
        [SerializeField] private List<Reward> _rewards = new List<Reward>();

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
