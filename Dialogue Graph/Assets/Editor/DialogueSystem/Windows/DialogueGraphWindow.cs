using System.Collections;
using System.Collections.Generic;  
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace DialogueSystem.Windows
{

    using Utilities;
    public class DialogueGraphWindow : EditorWindow
    {
        private DialogueGraphView graphView;
        private readonly string defaultFileName = "DialogueFileName";
        private static TextField fileNameTextfield;
        private Button saveButton;
        [MenuItem("Window/Dialouge Graph")]

        public static void Open()
        {
            GetWindow<DialogueGraphWindow>("Dialogue Graph");
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddToolBar();

            AddStyles();
        }

        #region Elements Addition
        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextfield = DialogueElementUtility.CreateTextField(defaultFileName, "File Name:", callback => {
                fileNameTextfield.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });
            saveButton = DialogueElementUtility.CreateButton("Save", () => Save());
            Button loadButton = DialogueElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DialogueElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = DialogueElementUtility.CreateButton("Reset", () => ResetGraph());
            toolbar.Add(fileNameTextfield);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.AddStyleSheet("DialogueSystem/DialogueToolBarStyles.uss");
            rootVisualElement.Add(toolbar);
        }

        private void AddGraphView()
        {
            graphView = new DialogueGraphView(this);

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }  
        private void AddStyles()
        {
            rootVisualElement.AddStyleSheet("DialogueSystem/DialogueVariables.uss");
        }
        #endregion
        #region ToolBarActions
        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextfield.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name youve typed in is valid.", "Ok");
                return;
            }
            DialogueIOUtility.Initialize(graphView, fileNameTextfield.value);
            DialogueIOUtility.Save();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();
            UpdateFileName(defaultFileName);
        }


        private void Load()
        {
           string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Clear();

            DialogueIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            DialogueIOUtility.Load();
        }
        #endregion
        #region Utility Methods
        public static void UpdateFileName(string newFileName)
        {
            fileNameTextfield.value = newFileName;
        }
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion
    }
}

