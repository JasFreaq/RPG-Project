using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        private Quest _quest;
        private List<string> _clearedObjectives = new List<string>();

        public QuestStatus(Quest quest)
        {
            _quest = quest;
        }

        public Quest Quest
        {
            get { return _quest; }
        }

        public int ClearedObjectivesCount
        {
            get { return _clearedObjectives.Count; }
        }

        public bool HasCleared(string objective)
        {
            return _clearedObjectives.Contains(objective);
        }
    }
}