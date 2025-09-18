using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Clients.ClientDialogue
{
    public enum DialogueCategory { Asking, Delivered, Perfect, Repeating, Wrong }
    
    [Serializable]
    public sealed class DialogueEntry
    {
        [TextArea] public string text;
        [Range(1, 10)] public int weight = 1; // Por si queremos que algunos textos tengan mas peso que otros
    }
    
}