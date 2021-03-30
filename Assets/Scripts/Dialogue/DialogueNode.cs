using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public static float MIN_WIDTH = 200;
        public static float MIN_HEIGHT = 100;

        [SerializeField] private string _nodeId;

        [SerializeField] private string _text;

        [SerializeField] private List<string> _childrenIds = new List<string>();

        [HideInInspector]
        public Rect positionRect = new Rect(100, 100, MIN_WIDTH, MIN_HEIGHT);

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

        public List<string> ChildrenIDs
        {
            get { return _childrenIds; }
        }
    }
}