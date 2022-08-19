using System.Collections.Generic;

namespace DialogueSystem.Data.Error
{
    using Elements;

    public class DialogueNodeErrorData
    {
        public DialogueErrorData ErrorData { get; set; }
        public List<DialogueNode> Nodes { get; set; }

        public DialogueNodeErrorData()
        {
            ErrorData = new DialogueErrorData();
            Nodes = new List<DialogueNode>();
        }
    }
}
