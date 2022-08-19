using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;


namespace DialogueSystem.Data.Error
{
    using Elements;

    public class DSGroupErrorData
    {
        public DialogueErrorData ErrorData { get; set; }
        public List<DialogueGroup> Groups { get; set; }
        public DSGroupErrorData()
        {
            ErrorData = new DialogueErrorData();
            Groups = new List<DialogueGroup>();
        }

    }
}
