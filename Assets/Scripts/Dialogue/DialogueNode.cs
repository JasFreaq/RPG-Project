using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        [SerializeField] private string _nodeId;

        [SerializeField] private string _text;

        [SerializeField] private string[] _childrenIds;

        /*[HideInInspector]*/ public Rect positionRect;

        public string NodeID
        {
            get { return _nodeId; }
            set { _nodeId = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}