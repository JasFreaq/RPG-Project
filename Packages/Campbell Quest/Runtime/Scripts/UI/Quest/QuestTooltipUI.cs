using System.Collections.Generic;
using Campbell.Quests;
using TMPro;
using UnityEngine;

namespace Campbell.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Transform _objectivesContainer;
        [SerializeField] private ObjectiveUI _objectivePrefab;
        [SerializeField] private TextMeshProUGUI _rewards;

        public void Setup(QuestStatus status)
        {
            _title.text = status.Quest.name;
            foreach (Quest.Objective objective in status.Quest.Objectives)
            {
                ObjectiveUI objectiveInstance = Instantiate(_objectivePrefab, _objectivesContainer);
                objectiveInstance.Setup(objective.description, status.HasCleared(objective.reference));
            }

            _rewards.text = GetRewardsString(status);
        }

        private string GetRewardsString(QuestStatus status)
        {
            string rewardsString = "";
            IReadOnlyList<Quest.Reward> rewards = status.Quest.Rewards;
            for (int i = 0, n = rewards.Count; i < n; i++)
            {
                rewardsString += $"- {rewards[i].item.GetDisplayName()} x{rewards[i].number}";

                if (i != n - 1)
                    rewardsString += "\n";
            }

            return rewardsString;
        }
    }
}