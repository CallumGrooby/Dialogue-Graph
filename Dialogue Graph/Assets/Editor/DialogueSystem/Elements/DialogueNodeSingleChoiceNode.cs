using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   using UnityEditor.Experimental.GraphView;
namespace DialogueSystem.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    public class DialogueNodeSingleChoice : DialogueNode
    {
        public override void Initizalize(string nodeName,DialogueGraphView dsGraphView, Vector2 postion)
        {
            base.Initizalize(nodeName,dsGraphView, postion);

            DialogueType = DialogueSystemDialogueType.SingleChoice;
            DialogueChoiceSaveData choiceSaveData = new DialogueChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceSaveData);
        }

        public override void Draw()
        {
            base.Draw();

            foreach (DialogueChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);
                choicePort.userData = choice;
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}

