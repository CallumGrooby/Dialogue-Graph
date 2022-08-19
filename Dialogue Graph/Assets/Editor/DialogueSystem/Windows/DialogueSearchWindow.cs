using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
namespace DialogueSystem.Windows
{
    using Elements;
    using Enumerations;
    public class DialogueSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DialogueGraphView dsGraphView)
        {
            graphView = dsGraphView;
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("CreateElement")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                level = 2,
                userData = DialogueSystemDialogueType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                 {
                    level = 2,
                    userData = DialogueSystemDialogueType.MultipleChoice
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon)){ 
                    level = 2,
                    userData = new Group()
                }

            };
            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            switch (SearchTreeEntry.userData)
            {
                case DialogueSystemDialogueType.SingleChoice:
                    {
                        DialogueNodeSingleChoice singleChoiceNode = (DialogueNodeSingleChoice)graphView.CreateNode("DialogueName", DialogueSystemDialogueType.SingleChoice, localMousePosition);
                        graphView.AddElement(singleChoiceNode);
                        return true;
                    }

                case DialogueSystemDialogueType.MultipleChoice:
                    {
                        DialogueNodeMultipleChoice multipleChoiceNode = (DialogueNodeMultipleChoice) graphView.CreateNode("DialogueName", DialogueSystemDialogueType.MultipleChoice, localMousePosition);
                        graphView.AddElement(multipleChoiceNode);
                        return true;
                    }

                case Group _:
                    {
                        graphView.CreateGroup("DialogueGroup", localMousePosition);
                        return true;
                    }

                default:
                     return false;
            }
        }
    }

}
