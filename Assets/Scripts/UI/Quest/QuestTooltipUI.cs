using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Transform _objectivesContainer;
        [SerializeField] private ObjectiveUI _objectivePrefab;

        public void Setup(QuestStatus status)
        {
            _title.text = status.Quest.name;
            foreach (string objective in status.Quest.Objectives)
            {
                ObjectiveUI objectiveInstance = Instantiate(_objectivePrefab, _objectivesContainer);
                objectiveInstance.Setup(objective, status.HasCleared(objective));
            }
        }
    }
}