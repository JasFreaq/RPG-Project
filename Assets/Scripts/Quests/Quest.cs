using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "RPG/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<string> _objectives = new List<string>();

        public IReadOnlyList<string> Objectives
        {
            get { return _objectives; }
        }
    }
}
