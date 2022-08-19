using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Data.Save
{
    using Enumerations;

    [System.Serializable]
    public class DialogueNodeSaveData
    {
        [field: SerializeField]
        public string ID { get; set; }
        [field: SerializeField]
        public string Name { get; set; }
        [field: SerializeField]
        public string Text { get; set; }
        [field: SerializeField]
        public List<DialogueChoiceSaveData> Choices { get; set; }
        [field: SerializeField]
        public string GroupID { get; set; }
        [field: SerializeField]
        public DialogueSystemDialogueType DialogueType;
        [field: SerializeField]
        public Vector2 Position { get; set; }
    }
}

