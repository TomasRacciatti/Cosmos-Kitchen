using System.Collections.Generic;
using UnityEngine;

namespace Characters.Clients.ClientDialogue
{
    [CreateAssetMenu(menuName = "ScriptableObject/Dialogue/DialogueList", fileName = "DialogueList_")]
    public sealed class DialogueListSO : ScriptableObject
    {
        public DialogueCategory category;
        public List<DialogueEntry> lines = new();
    }
}

