using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        [System.Serializable]
        struct QuestStatusRecord
        {
            public string questName;
            public List<string> clearedObjectiveRefs;

            public QuestStatusRecord(string name, List<string> objectiveRefs)
            {
                questName = name;
                clearedObjectiveRefs = objectiveRefs;
            }
        }

        private Quest _quest;
        private List<string> _clearedObjectives = new List<string>();

        public QuestStatus(Quest quest)
        {
            _quest = quest;
        }
        
        public QuestStatus(object obj)
        {
            QuestStatusRecord statusRecord = (QuestStatusRecord) obj;
            
            _quest = Quest.FindQuest(statusRecord.questName);
            _clearedObjectives = statusRecord.clearedObjectiveRefs;
        }

        public Quest Quest
        {
            get { return _quest; }
        }

        public void ClearObjective(string objectiveRef)
        {
            if (_quest.ContainsObjective(objectiveRef))
            {
                _clearedObjectives.Add(objectiveRef);
            }
        }

        public int ClearedObjectivesCount
        {
            get { return _clearedObjectives.Count; }
        }

        public bool HasCleared(string objectiveRef)
        {
            return _clearedObjectives.Contains(objectiveRef);
        }

        public bool IsComplete()
        {
            foreach (Quest.Objective objective in _quest.Objectives)
            {
                if (!_clearedObjectives.Contains(objective.reference))
                    return false;
            }

            return true;
        }

        public object CaptureState()
        {
            return new QuestStatusRecord(_quest.name, _clearedObjectives);
        }
    }
}