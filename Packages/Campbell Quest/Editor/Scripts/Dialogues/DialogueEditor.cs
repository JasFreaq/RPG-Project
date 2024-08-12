using System;
using Campbell.Dialogues;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Campbell.Editor.Dialogues
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

        private const int NODE_PADDING = 15;
        private const int CANVAS_SIZE = 5000;
        private const int BG_SIZE = 50;

        private Dialogue _selectedDialogue = null;

        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private GUIStyle _playerNodeStyle;
        [NonSerialized] private GUIStyle _linkingNodeStyle;
        [NonSerialized] private DialogueNode _createUsingNode = null;
        [NonSerialized] private DialogueNode _linkingNode = null;
        [NonSerialized] private DialogueNode _deleteNode = null;

        [NonSerialized] private DialogueNode _nodeBeingResized = null;
        [NonSerialized] private NodeEdge _resizeEdge = NodeEdge.None;
        [NonSerialized] private float _allowedResizeDistance = 10;
        
        [NonSerialized] private DialogueNode _nodeBeingDragged = null;
        [NonSerialized] private Vector2 _dragOffset = Vector2.zero;

        [NonSerialized] private bool _scrollDragging = false;
        [NonSerialized] private Vector2 _scrollDragOffset = Vector2.zero;

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

            _nodeStyle = GetGUIStyle("node0");
            _playerNodeStyle = GetGUIStyle("node1");
            _linkingNodeStyle = GetGUIStyle("node2");
        }
        
        private void OnGUI()
        {
            if (_selectedDialogue)
            {
                ProcessEvents();

                _selectedDialogue.EditorScrollPosition = EditorGUILayout.BeginScrollView(_selectedDialogue.EditorScrollPosition);

                //Set Editor Background
                Rect canvasRect = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
                Texture2D background = Resources.Load<Texture2D>("campbellDialogueEditorBackground");
                Rect texCoords = new Rect(0, 0, CANVAS_SIZE / BG_SIZE, CANVAS_SIZE / BG_SIZE);
                GUI.DrawTextureWithTexCoords(canvasRect, background, texCoords);

                foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
                {
                    DrawConnections(node);
                }

                foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (_createUsingNode != null)
                {
                    _selectedDialogue.CreateNode(_createUsingNode);
                    _createUsingNode = null;
                }
                
                if (_deleteNode != null)
                {
                    _selectedDialogue.DeleteNode(_deleteNode);
                    _deleteNode = null;
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
                        _dragOffset = _nodeBeingDragged.PositionRect.position - Event.current.mousePosition;
                        Selection.activeObject = _nodeBeingDragged;
                    }
                    else
                    {
                        _scrollDragging = true;
                        _scrollDragOffset = Event.current.mousePosition + _selectedDialogue.EditorScrollPosition;
                        Selection.activeObject = _selectedDialogue;
                    }
                }
            }
            else if (_nodeBeingResized != null)
            {
                SetResizeCursor();
                
                Rect tempRect = new Rect(_nodeBeingResized.PositionRect);
                Vector2 point = Event.current.mousePosition + _selectedDialogue.EditorScrollPosition;
                float adjustment;
                switch (_resizeEdge)
                {
                    case NodeEdge.Top:
                        tempRect.yMin = point.y;
                        adjustment = tempRect.yMin - point.y;
                        if (DialogueNode.MIN_HEIGHT > tempRect.height - adjustment)
                        {
                            tempRect.yMin = _nodeBeingResized.PositionRect.yMin;
                            tempRect.height = DialogueNode.MIN_HEIGHT;
                        }
                        else
                        {
                            tempRect.height -= adjustment;
                        }

                        break;

                    case NodeEdge.Right:
                        adjustment = _nodeBeingResized.PositionRect.xMax - point.x;
                        tempRect.width = Mathf.Max(DialogueNode.MIN_WIDTH, tempRect.width - adjustment);

                        break;

                    case NodeEdge.Bottom:
                        adjustment = _nodeBeingResized.PositionRect.yMax - point.y;
                        tempRect.height = Mathf.Max(DialogueNode.MIN_HEIGHT, tempRect.height - adjustment);

                        break;

                    case NodeEdge.Left:
                        tempRect.xMin = point.x;
                        adjustment = tempRect.xMin - point.x;
                        if (DialogueNode.MIN_WIDTH > tempRect.width - adjustment)
                        {
                            tempRect.xMin = _nodeBeingResized.PositionRect.xMin;
                            tempRect.width = DialogueNode.MIN_WIDTH;
                        }
                        else
                        {
                            tempRect.width -= adjustment;
                        }

                        break;
                }

                _nodeBeingResized.PositionRect = tempRect;
                GUI.changed = true;

                if (Event.current.type == EventType.MouseUp)
                {
                    _nodeBeingResized = null;
                }
            }
            else if (_nodeBeingDragged != null)
            {
                Rect tempRect = new Rect(_nodeBeingDragged.PositionRect);
                tempRect.position = Event.current.mousePosition + _dragOffset;
                _nodeBeingDragged.PositionRect = tempRect;
                GUI.changed = true;

                if (Event.current.type == EventType.MouseUp)
                {
                    _nodeBeingDragged = null;
                    _dragOffset = Vector2.zero;
                }
            }
            else if (_scrollDragging)
            {
                _selectedDialogue.EditorScrollPosition = _scrollDragOffset - Event.current.mousePosition;
                GUI.changed = true;

                if (Event.current.type == EventType.MouseUp)
                {
                    _scrollDragging = false;
                    _scrollDragOffset = Vector2.zero;
                }
            }
        }
        
        private void DrawNode(DialogueNode node)
        {
            //Adjust Node Styles and BeginArea
            if (node == _linkingNode)
            {
                GUILayout.BeginArea(node.PositionRect, _linkingNodeStyle);
            }
            else if (node.IsPlayerSpeech)
            {
                GUILayout.BeginArea(node.PositionRect, _playerNodeStyle);
            }
            else
            {
                GUILayout.BeginArea(node.PositionRect, _nodeStyle);
            }

            //Text Field
            GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField);
            textFieldStyle.wordWrap = true;
            node.SetText(EditorGUILayout.TextArea(node.Text, textFieldStyle));
            
            //Horizontal Space for Adding, Deleting and Modifying Children of Node
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            node.SetIsPlayer(GUILayout.Toggle(node.IsPlayerSpeech, "IsPlayer"));
            GUILayout.Space(5);

            DrawModifyButtons(node);

            GUILayout.EndHorizontal();
            //End Horizontal Space

            GUILayout.Space(10);

            //Horizontal Space for Modifying Dialogue Action of Node
            GUILayout.BeginHorizontal();

            DrawDialogueActions(node);

            GUILayout.EndHorizontal();
            //End Horizontal Space

            GUILayout.EndArea();
        }

        private static void DrawDialogueActions(DialogueNode node)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("OnEnterActions");
            if (node.EditingOnEnterActions)
            {
                foreach (DialogueAction action in Enum.GetValues(typeof(DialogueAction)))
                {
                    node.ModifyOnEnterAction(action,
                        GUILayout.Toggle(node.OnEnterActionsContain(action), action.ToString()));
                }

                if (GUILayout.Button("Stop Edit"))
                {
                    node.EditingOnEnterActions = false;
                }
            }
            else
            {
                foreach (DialogueAction action in node.OnEnterActions)
                {
                    GUILayout.Label(action.ToString());
                }

                if (GUILayout.Button("Edit"))
                {
                    node.EditingOnEnterActions = true;
                }
            }
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Label("OnExitActions");
            if (node.EditingOnExitActions)
            {
                foreach (DialogueAction action in Enum.GetValues(typeof(DialogueAction)))
                {
                    node.ModifyOnExitAction(action,
                        GUILayout.Toggle(node.OnExitActionsContain(action), action.ToString()));
                }

                if (GUILayout.Button("Stop Edit"))
                {
                    node.EditingOnExitActions = false;
                }
            }
            else
            {
                foreach (DialogueAction action in node.OnExitActions)
                {
                    GUILayout.Label(action.ToString());
                }

                if (GUILayout.Button("Edit"))
                {
                    node.EditingOnExitActions = true;
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector2(node.PositionRect.xMax - NODE_PADDING / 2, node.PositionRect.center.y);
            foreach (DialogueNode child in _selectedDialogue.GetChildrenOfNode(node))
            {
                Vector3 endPos = new Vector2(child.PositionRect.xMin + NODE_PADDING / 2, child.PositionRect.center.y);
                
                Vector3 curveOffset = node.PositionRect.xMax > child.PositionRect.xMin
                    ? startPos - endPos
                    : endPos - startPos;
                curveOffset.x *= 0.75f;
                curveOffset.y = 0;

                Handles.DrawBezier(startPos, endPos, 
                    startPos + curveOffset, endPos - curveOffset, 
                    Color.white, null, 5f);
            }
        }

        private void DrawModifyButtons(DialogueNode node)
        {
            if (GUILayout.Button("+"))
            {
                _createUsingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("-"))
            {
                _deleteNode = node;
            }
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkingNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    _linkingNode = node;
                }
            }
            else
            {
                if (node == _linkingNode)
                {
                    if (GUILayout.Button("finish"))
                    {
                        _linkingNode = null;
                    }
                }
                else if (_linkingNode.ContainsChild(node.name))
                {
                    if (GUILayout.Button("unlink"))
                    {
                        _linkingNode.RemoveChild(node.name);
                    }
                }
                else
                {
                    if (GUILayout.Button("child"))
                    {
                        _linkingNode.AddChild(node.name);
                    }
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

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            point += _selectedDialogue.EditorScrollPosition;
            foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
            {
                if (node.PositionRect.Contains(point))
                    return node;
            }

            return null;
        }
        
        private DialogueNode GetNodeAndEdgeNearPoint(Vector2 point)
        {
            _resizeEdge = NodeEdge.None;
            point += _selectedDialogue.EditorScrollPosition;
            foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
            {
                if (Mathf.Abs(node.PositionRect.yMin - point.y) <= _allowedResizeDistance)
                {
                    if (point.x > node.PositionRect.xMin && point.x < node.PositionRect.xMax)
                    {
                        _resizeEdge = NodeEdge.Top;
                        return node;
                    }
                }

                if (Mathf.Abs(node.PositionRect.xMax - point.x) <= _allowedResizeDistance)
                {
                    if (point.y > node.PositionRect.yMin && point.y < node.PositionRect.yMax)
                    {
                        _resizeEdge = NodeEdge.Right;
                        return node;
                    }
                }

                if (Mathf.Abs(node.PositionRect.yMax - point.y) <= _allowedResizeDistance)
                {
                    if (point.x > node.PositionRect.xMin && point.x < node.PositionRect.xMax)
                    {
                        _resizeEdge = NodeEdge.Bottom;
                        return node;
                    }
                }

                if (Mathf.Abs(node.PositionRect.xMin - point.x) <= _allowedResizeDistance)
                {
                    if (point.y > node.PositionRect.yMin && point.y < node.PositionRect.yMax)
                    {
                        _resizeEdge = NodeEdge.Left;
                        return node;
                    }
                }
            }

            return null;
        }

        private GUIStyle GetGUIStyle(string texturePath)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = (Texture2D)EditorGUIUtility.Load(texturePath);
            style.padding = new RectOffset(NODE_PADDING, NODE_PADDING, NODE_PADDING, NODE_PADDING);
            style.border = new RectOffset(12, 12, 12, 12);

            return style;
        }
    }
}