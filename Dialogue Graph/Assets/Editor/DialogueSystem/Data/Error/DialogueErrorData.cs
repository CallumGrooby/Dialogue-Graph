using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace DialogueSystem.Data.Error
{
    public class DialogueErrorData
    {
        public Color color { get; set; }

        public DialogueErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            color = new Color32(
                (byte)Random.Range(65, 256),
                (byte)Random.Range(50, 256),
                (byte)Random.Range(50, 256),
                255
                );
        }
    }
}

