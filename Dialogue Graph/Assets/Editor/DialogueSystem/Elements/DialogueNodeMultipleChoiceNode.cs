using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;  
using UnityEngine.UIElements;

namespace DialogueSystem.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DialogueNodeMultipleChoice : DialogueNode
    {
        public override void Initizalize(string nodeName, DialogueGraphView dsGraphView, Vector2 postion)
        {
            base.Initizalize(nodeName, dsGraphView, postion);

            DialogueType = DialogueSystemDialogueType.MultipleChoice;

            DialogueChoiceSaveData choiceSaveData = new DialogueChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choiceSaveData);
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */

            Button addChoiceButton = DialogueElementUtility.CreateButton("Add Choice", () =>
            {
                DialogueChoiceSaveData choiceData = new DialogueChoiceSaveData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");

            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */

            foreach (DialogueChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)userData;

            Button deleteChoiceButton = DialogueElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);

                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = DialogueElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }
    }
}
