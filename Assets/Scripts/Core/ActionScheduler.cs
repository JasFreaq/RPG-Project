using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction _action;
        public void StartAction(IAction newAction)
        {
            if (_action != null)
            {
                if (newAction == _action)
                    return;
                else
                {
                    _action.Cancel();                    
                }
            }
            _action = newAction;
        }
    }
}