using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Core
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
                    CancelCurrentAction();
                }
            }
            _action = newAction;
        }

        public void CancelCurrentAction()
        {
            if(_action!=null)
                _action.CancelAction();
        }

        private void Kill()
        {
            this.enabled = false;
        }
    }
}