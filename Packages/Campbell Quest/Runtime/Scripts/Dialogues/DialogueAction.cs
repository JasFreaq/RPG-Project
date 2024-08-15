using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Dialogues
{
    [System.Serializable]
    public enum DialogueAction
    {
        None,
        Attack,
        GiveQuest,
        CompleteObjective,
        CompleteQuest,
        EditInventory
    }

    [System.Serializable]
    public class DialogueActionData
    {
        public DialogueAction action;
        public string parameter = null;

        public override string ToString()
        {
            return $"{action.ToString()}({(string.IsNullOrWhiteSpace(parameter) ? "": parameter)})";
        }
    }
}