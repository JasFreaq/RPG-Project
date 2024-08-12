using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Quests
{
    public class QuestClearer : MonoBehaviour
    {
        [SerializeField] private Quest _quest;
        [SerializeField] private string _objectiveReference;

        public void UpdateQuestCompletion()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.ClearQuest(_quest, _objectiveReference);
        }
    }
}