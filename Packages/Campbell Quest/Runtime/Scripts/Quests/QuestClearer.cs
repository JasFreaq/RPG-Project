using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Quests
{
    public class QuestClearer : MonoBehaviour
    {
        [SerializeField] private Quest _quest;

#if UNITY_EDITOR
        public Quest Quest { set => _quest = value; }
#endif

        public void CompleteQuestObjective(string objectiveReference)
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.ClearQuest(_quest, objectiveReference);
        }
        
        public void CompleteQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.ClearQuest(_quest);
        }
    }
}