using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Data
{
    using ScriptableObjects;
    [System.Serializable]
    public class DialogueChoiceData 
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DialogueSO nextDialogue { get; set; }

    }
}

