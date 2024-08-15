using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest _quest;

#if UNITY_EDITOR
        public Quest Quest { set => _quest = value; }
#endif

        public void GiveQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.AddQuest(_quest);
        }
    }
}