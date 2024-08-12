using Campbell.Quests;
using TMPro;
using UnityEngine;

namespace Campbell.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _progress;

        private QuestStatus _status;

        public QuestStatus Status
        {
            get { return _status; }
        }

        public void Setup(QuestStatus status)
        {
            _status = status;

            _title.text = status.Quest.name;

            int cleared = status.ClearedObjectivesCount;
            int total = status.Quest.Objectives.Count;

            _progress.text = $"{cleared}/{total}";
        }
    }
}