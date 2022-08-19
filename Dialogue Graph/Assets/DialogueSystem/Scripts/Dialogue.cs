using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using System;
using UnityEngine.UI;
namespace DialogueSystem
{
    using ScriptableObjects;
    using Enumerations;
    using Data;
    using UnityEngine.Events;

    public class Dialogue : MonoBehaviour
    {

        //Dialogue Scriptable Objects
        [SerializeField] private DialogueContainerSO dialogueContainer;
        [SerializeField] private DialogueGroupSO dialogueGroup;
        [SerializeField] private DialogueSO dialogue;
        //Filters
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;
        //Indexs
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;


       
        [SerializeField] private TextMeshProUGUI textElement;
        [SerializeField] private Button buttonElementPrefab;
        [SerializeField] private  GameObject buttonArea;
        public void Start()
        {
            ShowDialogue();
            Debug.Log(dialogue.DialogueType);
        }

        public void ShowDialogue()
        {

            if (dialogue.DialogueType == DialogueSystemDialogueType.SingleChoice)
            {
                buttonArea.gameObject.SetActive(false);
            }
            textElement.text = dialogue.Text;


            if (dialogue.DialogueType == DialogueSystemDialogueType.MultipleChoice)
            {
                ShowButtons(dialogue);
            }
        }

        private void ShowButtons(DialogueSO dialogue)
        {
            buttonArea.gameObject.SetActive(true);
            List<Button> buttonsAdded = new List<Button>();

            foreach (var dialogueChoiceData in dialogue.Choices)
            {
                if (buttonElementPrefab == null)
                    return;

                Button button = Instantiate(buttonElementPrefab, buttonArea.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = dialogueChoiceData.Text;
                buttonsAdded.Add(button);
                button.onClick.AddListener(() => {
                    SetNextDialogue(dialogueChoiceData, buttonsAdded);
                });
            }
        }

        private void SetNextDialogue(DialogueChoiceData dialogueChoiceData, List<Button> activeButtons)
        {
            foreach (Button activeButton in activeButtons)
            {
                Debug.Log(" deleteding ");
                Destroy(activeButton.gameObject);
            }
            if (dialogueChoiceData.nextDialogue != null)
            {
                dialogue = dialogueChoiceData.nextDialogue;
                ShowDialogue();
            }
        }
    }
}
