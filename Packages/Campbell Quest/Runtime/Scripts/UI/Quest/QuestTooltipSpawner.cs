using Campbell.Quests;
using Campbell.Utils.UI.Tooltips;
using UnityEngine;

namespace Campbell.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus status = GetComponent<QuestItemUI>().Status;
            tooltip.GetComponent<QuestTooltipUI>().Setup(status);
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
}
