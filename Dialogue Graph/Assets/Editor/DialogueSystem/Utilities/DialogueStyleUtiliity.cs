using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace DialogueSystem.Utilities
{
    public static class DialogueStyleUtiliity
    {
        public static VisualElement AddClasses(this VisualElement element,params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }
            return element;
        }

        public static VisualElement AddStyleSheet(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName);
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }
}

