using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;    
using UnityEngine;
namespace DialogueSystem.Windows
{
    using Data.Save;
    using Data.Error;
    using Elements;
    using Enumerations;
    using System.Collections.Generic;
    using Utilities;
    public class DialogueGraphView : GraphView
    {
        private DialogueSearchWindow searchWindow;
        private DialogueGraphWindow editorWindow;

        private SerializableDictionary<string, DialogueNodeErrorData> ungroupedNodes; 
        private SerializableDictionary<string, DSGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, DialogueNodeErrorData>> groupedNodes;

        private int namesErrorsAmount;
        public int NamesErrorsAmount {
            get { return namesErrorsAmount;}
            set { namesErrorsAmount = value;
                if (namesErrorsAmount == 0)
                {
                    //Enable Save Button
                    editorWindow.EnableSaving();
                }
                if (namesErrorsAmount == 1)
                {
                    //Disable Save Button
                    editorWindow.DisableSaving();
                }
            }
        }

        public DialogueGraphView(DialogueGraphWindow dialogueGraphWindow)
        {
            editorWindow = dialogueGraphWindow;

            ungroupedNodes = new SerializableDictionary<string, DialogueNodeErrorData>();
            groups = new SerializableDictionary<string, DSGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogueNodeErrorData>>();
            AddManipulators();
            AddSearchWindow();
            AddGridBackGround();
            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiablePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }
                if (startPort.node == port.node)
                {
                    return;
                }
                if (startPort.direction == port.direction)
                {
                    return;
                }
                compatiablePorts.Add(port);
            });

            return compatiablePorts;
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueSystemDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueSystemDialogueType.MultipleChoice));
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateGroupContextualMenu());

        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup",GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
                );

            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueSystemDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }



        public DialogueGroup CreateGroup(string title, Vector2 position)
        {
            DialogueGroup group = new DialogueGroup(title, position);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is DialogueNode))
                {
                    continue;
                }

                DialogueNode node = (DialogueNode)selectedElement;

                group.AddElement(node);
            }

            return group;
        }

        private void AddGroup(DialogueGroup group)
        {
            string groupName = group.title.ToLower();
            if (!groups.ContainsKey(groupName))
            {
                DSGroupErrorData groupErrorData = new DSGroupErrorData();
                groupErrorData.Groups.Add(group);
                groups.Add(groupName, groupErrorData);
                return;
            }

            List<DialogueGroup> groupsList = groups[groupName].Groups;
            groupsList.Add(group);

            Color errorColor = groups[groupName].ErrorData.color;
            group.SetErrorStyle(errorColor);

            if (groupsList.Count ==2)
            {
                NamesErrorsAmount++;
                groupsList[0].SetErrorStyle(errorColor);
            }

        }

        public DialogueNode CreateNode(string nodeName,DialogueSystemDialogueType dialogueType, Vector2 position, bool shouldDraw = true) 
        {
            Type nodeType = Type.GetType($"DialogueSystem.Elements.DialogueNode{dialogueType}");
            DialogueNode node =(DialogueNode) Activator.CreateInstance(nodeType);
            node.Initizalize(nodeName,this,position);

            if (shouldDraw)
            {
                node.Draw();
            }

            AddElement(node);
            AddUngroupedNode(node);

            return node;
        }

        public void AddUngroupedNode(DialogueNode node)
        {
            string nodeName = node.DialogueName.ToLower();
            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                DialogueNodeErrorData nodeErrorData = new DialogueNodeErrorData();

                nodeErrorData.Nodes.Add(node);
                ungroupedNodes.Add(nodeName, nodeErrorData);
                return;
            }

            List<DialogueNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;
            ungroupedNodesList.Add(node);
            Color errorColor = ungroupedNodes[nodeName].ErrorData.color;
            node.SetErrorStyle(errorColor);

            if (ungroupedNodesList.Count == 2)
            {
                NamesErrorsAmount++;
                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        public void RemoveUngroupedNodes(DialogueNode node)
        {

            string nodeName = node.DialogueName.ToLower();
            List<DialogueNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;
            ungroupedNodesList.Remove(node);
            node.ResetStyle();
            if (ungroupedNodesList.Count == 1)
            {
                NamesErrorsAmount--;
                ungroupedNodesList[0].ResetStyle();
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }

        private void AddGridBackGround()
        {
            GridBackground gridbackground = new GridBackground();
            gridbackground.StretchToParentSize();
            Insert(0, gridbackground);
        }

        public void AddStyles()
        {
            this.AddStyleSheet(
                "DialogueSystem/DialogueGraphViewStyles.uss", 
                "DialogueSystem/DSNodeStyles.uss"
                );
        }
        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DialogueSearchWindow>();
                searchWindow.Initialize(this);

            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow =false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DialogueGroup);
                Type edgeType = typeof(Edge);

                List<DialogueGroup> groupsToDelete = new List<DialogueGroup>();
                List<DialogueNode> nodesToDelete = new List<DialogueNode>();
                List<Edge> edgesToDelete = new List<Edge>();

                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is DialogueNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        Edge edge = (Edge)selectedElement;

                        edgesToDelete.Add(edge);

                        continue;
                    }

                    if (selectedElement.GetType() != groupType)
                    {
                        continue;
                    }

                    DialogueGroup group = (DialogueGroup)selectedElement;

                    groupsToDelete.Add(group);
                }

                foreach (DialogueGroup groupToDelete in groupsToDelete)
                {
                    List<DialogueNode> groupNodes = new List<DialogueNode>();

                    foreach (GraphElement groupElement in groupToDelete.containedElements)
                    {
                        if (!(groupElement is DialogueNode))
                        {
                            continue;
                        }

                        DialogueNode groupNode = (DialogueNode)groupElement;

                        groupNodes.Add(groupNode);
                    }

                    groupToDelete.RemoveElements(groupNodes);

                    RemoveGroup(groupToDelete);

                    RemoveElement(groupToDelete);
                }

                DeleteElements(edgesToDelete);

                foreach (DialogueNode nodeToDelete in nodesToDelete)
                {
                    if (nodeToDelete.Group != null)
                    {
                        nodeToDelete.Group.RemoveElement(nodeToDelete);
                    }

                    RemoveUngroupedNodes(nodeToDelete);

                    nodeToDelete.DisconnectAllPorts();

                    RemoveElement(nodeToDelete);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) => 
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DialogueNode))
                    {
                        continue;
                    }
                    DialogueGroup nodeGroup = (DialogueGroup)group;
                    DialogueNode node = (DialogueNode)element;

                    RemoveUngroupedNodes(node);
                    AddGroupedNodes(node, nodeGroup);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DialogueNode))
                    {
                        continue;
                    }

                    DialogueGroup dsGroup = (DialogueGroup)group;
                    DialogueNode node = (DialogueNode)element;

                    RemoveGroupedNodes(node, dsGroup);
                    AddUngroupedNode(node);
                }
            };
        }



        public void AddGroupedNodes(DialogueNode node, DialogueGroup group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, DialogueNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DialogueNodeErrorData nodeErrorData = new DialogueNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<DialogueNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.color;

            node.SetErrorStyle(errorColor);

            if (groupedNodesList.Count == 2)
            {
                ++namesErrorsAmount;

                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }     

        public void RemoveGroupedNodes(DialogueNode node, Group group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = null;

            List<DialogueNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);

            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                --NamesErrorsAmount;

                groupedNodesList[0].ResetStyle();

                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }


        private void RemoveGroup(DialogueGroup group)
        {
            string oldGroupName = group.oldTitle.ToLower();
            List<DialogueGroup> groupsList = groups[oldGroupName].Groups;

            groupsList.Remove(group);
            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                groupsList[0].ResetStyle();
                return;
            }
            if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }

        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DialogueGroup dsGroup = (DialogueGroup) group;

                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.oldTitle))
                    {
                        ++namesErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dsGroup.oldTitle))
                    {
                        --namesErrorsAmount;
                    }
                }

                RemoveGroup(dsGroup);

                dsGroup.oldTitle = dsGroup.title;

                AddGroup(dsGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) => 
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        DialogueNode nextNode = (DialogueNode)edge.input.node;
                        DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)edge.output.userData;

                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);
                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }
                        Edge edge = (Edge)element;
                        DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)edge.output.userData;
                        choiceData.NodeID = "";
                    }
                }
                return changes;
            };

        } 
        public void ClearGraph()
        {
            graphElements.ForEach(graphElements => RemoveElement(graphElements));
            groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();

            namesErrorsAmount = 0;
        }
    }


}

