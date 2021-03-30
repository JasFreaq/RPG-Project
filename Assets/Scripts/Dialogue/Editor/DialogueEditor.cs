using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        public enum NodeEdge
        {
            None,
            Top,
            Right,
            Bottom,
            Left
        }

        private const int NODE_PADDING = 10;

        private Dialogue _selectedDialogue = null;
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private DialogueNode _createUsingNode = null;

        [NonSerialized] private DialogueNode _nodeBeingResized = null;
        [NonSerialized] private NodeEdge _resizeEdge = NodeEdge.None;
        [NonSerialized] private float _allowedResizeDistance = 10;
        
        [NonSerialized] private DialogueNode _nodeBeingDragged = null;
        [NonSerialized] private Vector2 _dragOffset = Vector2.zero;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenDialogue(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is Dialogue)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnDialogueSelected;
            OnDialogueSelected();

            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = (Texture2D)EditorGUIUtility.Load("node0");
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.padding = new RectOffset(NODE_PADDING, NODE_PADDING, NODE_PADDING, NODE_PADDING);
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }
        
        private void OnGUI()
        {
            if (_selectedDialogue)
            {
                ProcessEvents();

                foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
                {
                    DrawConnections(node);
                }

                foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
                {
                    DrawNode(node);
                }

                if (_createUsingNode != null)
                {
                    Undo.RecordObject(_selectedDialogue, "Dialogue Node Created");

                    _selectedDialogue.CreateNode(_createUsingNode);
                    _createUsingNode = null;
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Dialogue asset selected.");
            }
        }
        
        private void OnDisable()
        {
            Selection.selectionChanged -= OnDialogueSelected;
        }

        private void OnDialogueSelected()
        {
            Dialogue tempDialogue = Selection.activeObject as Dialogue;
            if (tempDialogue)
            {
                _selectedDialogue = tempDialogue;
                Repaint();
            }
        }

        private void ProcessEvents()
        {
            DialogueNode tempResizeNode = null;
            if (_nodeBeingResized == null)
            {
                tempResizeNode = GetNodeAndEdgeNearPoint(Event.current.mousePosition);
                SetResizeCursor();
            }
            
            if (Event.current.type == EventType.MouseDown)
            {
                if (_nodeBeingResized == null)
                {
                    _nodeBeingResized = tempResizeNode;
                }

                if (_nodeBeingDragged == null && _nodeBeingResized == null)
                {
                    _nodeBeingDragged = GetNodeAtPoint(Event.current.mousePosition);
                    if (_nodeBeingDragged != null)
                    {
                        _dragOffset = _nodeBeingDragged.positionRect.position - Event.current.mousePosition;
                    }
                }
            }
            else if (_nodeBeingResized != null)
            {
                SetResizeCursor();

                Undo.RecordObject(_selectedDialogue, "Dialogue Node Size Edit");

                Rect tempRect = _nodeBeingResized.positionRect;
                float adjustment;
                switch (_resizeEdge)
                {
                    case NodeEdge.Top:
                        _nodeBeingResized.positionRect.yMin = Event.current.mousePosition.y;
                        adjustment = _nodeBeingResized.positionRect.yMin - Event.current.mousePosition.y;
                        if (DialogueNode.MIN_HEIGHT > _nodeBeingResized.positionRect.height - adjustment)
                        {
                            _nodeBeingResized.positionRect.yMin = tempRect.yMin;
                            _nodeBeingResized.positionRect.height = DialogueNode.MIN_HEIGHT;
                        }
                        else
                        {
                            _nodeBeingResized.positionRect.height -= adjustment;
                        }

                        break;

                    case NodeEdge.Right:
                        adjustment = tempRect.xMax - Event.current.mousePosition.x;
                        _nodeBeingResized.positionRect.width = Mathf.Max(DialogueNode.MIN_WIDTH,
                            _nodeBeingResized.positionRect.width - adjustment);

                        break;

                    case NodeEdge.Bottom:
                        adjustment = tempRect.yMax - Event.current.mousePosition.y;
                        _nodeBeingResized.positionRect.height = Mathf.Max(DialogueNode.MIN_HEIGHT,
                            _nodeBeingResized.positionRect.height - adjustment);

                        break;

                    case NodeEdge.Left:
                        _nodeBeingResized.positionRect.xMin = Event.current.mousePosition.x;
                        adjustment = _nodeBeingResized.positionRect.xMin - Event.current.mousePosition.x;
                        if (DialogueNode.MIN_WIDTH > _nodeBeingResized.positionRect.width - adjustment)
                        {
                            _nodeBeingResized.positionRect.xMin = tempRect.xMin;
                            _nodeBeingResized.positionRect.width = DialogueNode.MIN_WIDTH;
                        }
                        else
                        {
                            _nodeBeingResized.positionRect.width -= adjustment;
                        }

                        break;
                }
            
                GUI.changed = true;

                if (Event.current.type == EventType.MouseUp)
                {
                    _nodeBeingResized = null;
                }
            }
            else if (_nodeBeingDragged != null)
            {
                Undo.RecordObject(_selectedDialogue, "Dialogue Node Pos Edit");

                _nodeBeingDragged.positionRect.position = Event.current.mousePosition + _dragOffset;
                GUI.changed = true;

                if (Event.current.type == EventType.MouseUp)
                {
                    _nodeBeingDragged = null;
                    _dragOffset = Vector2.zero;
                }
            }
        }

        private void SetResizeCursor()
        {
            switch (_resizeEdge)
            {
                case NodeEdge.Top:
                case NodeEdge.Bottom:
                    EditorGUIUtility.AddCursorRect(
                        new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 500, 500),
                        MouseCursor.ResizeVertical);
                    break;
                case NodeEdge.Left:
                case NodeEdge.Right:
                    EditorGUIUtility.AddCursorRect(
                        new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 500, 500),
                        MouseCursor.ResizeHorizontal);
                    break;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.positionRect, _nodeStyle);
            EditorGUI.BeginChangeCheck();
            
            string newText = EditorGUILayout.TextField(node.Text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Dialogue Text Edit");

                node.Text = newText;
            }

            if (GUILayout.Button("+"))
            {
                _createUsingNode = node;
            }

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector2(node.positionRect.xMax - NODE_PADDING / 2, node.positionRect.center.y);
            foreach (DialogueNode child in _selectedDialogue.GetChildren(node))
            {
                Vector3 endPos = new Vector2(child.positionRect.xMin + NODE_PADDING / 2, child.positionRect.center.y);
                
                Vector3 curveOffset = node.positionRect.xMax > child.positionRect.xMin
                    ? startPos - endPos
                    : endPos - startPos;
                curveOffset.x *= 0.75f;
                curveOffset.y = 0;

                Handles.DrawBezier(startPos, endPos, 
                    startPos + curveOffset, endPos - curveOffset, 
                    Color.white, null, 5f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
            {
                if (node.positionRect.Contains(point))
                    return node;
            }

            return null;
        }
        
        private DialogueNode GetNodeAndEdgeNearPoint(Vector2 point)
        {
            _resizeEdge = NodeEdge.None;
            foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
            {
                if (Mathf.Abs(node.positionRect.yMin + NODE_PADDING - point.y) <= _allowedResizeDistance)
                {
                    if (point.x > node.positionRect.xMin && point.x < node.positionRect.xMax)
                    {
                        _resizeEdge = NodeEdge.Top;
                        return node;
                    }
                }

                if (Mathf.Abs(node.positionRect.xMax - NODE_PADDING - point.x) <= _allowedResizeDistance)
                {
                    if (point.y > node.positionRect.yMin && point.y < node.positionRect.yMax)
                    {
                        _resizeEdge = NodeEdge.Right;
                        return node;
                    }
                }

                if (Mathf.Abs(node.positionRect.yMax - NODE_PADDING - point.y) <= _allowedResizeDistance)
                {
                    if (point.x > node.positionRect.xMin && point.x < node.positionRect.xMax)
                    {
                        _resizeEdge = NodeEdge.Bottom;
                        return node;
                    }
                }

                if (Mathf.Abs(node.positionRect.xMin + NODE_PADDING - point.x) <= _allowedResizeDistance)
                {
                    if (point.y > node.positionRect.yMin && point.y < node.positionRect.yMax)
                    {
                        _resizeEdge = NodeEdge.Left;
                        return node;
                    }
                }
            }

            return null;
        }
    }
}