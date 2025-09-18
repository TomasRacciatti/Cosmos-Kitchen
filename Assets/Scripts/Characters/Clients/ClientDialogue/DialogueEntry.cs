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
    
    [CreateAssetMenu(menuName = "ScriptableObject/Dialogue/DialogueList", fileName = "DialogueList_")]
    public sealed class DialogueListSO : ScriptableObject
    {
        public DialogueCategory category;
        public List<DialogueEntry> lines = new();
    }
    
    [CreateAssetMenu(menuName = "ScriptableObject/Dialogue/ClientDialogueProfile", fileName = "ClientDialogue_")]
    public sealed class ClientDialogueProfileSO : ScriptableObject
    {
        [Header("General Dialogue Lists (Optional)")]
        public DialogueListSO delivered;
        public DialogueListSO perfect;
        public DialogueListSO repeating;
        public DialogueListSO wrong;

        [Header("Client-specific extra lines")]
        public List<DialogueEntry> extraAsking    = new();
        public List<DialogueEntry> extraDelivered = new();
        public List<DialogueEntry> extraPerfect   = new();
        public List<DialogueEntry> extraRepeating = new();
        public List<DialogueEntry> extraWrong     = new();

        public IReadOnlyList<DialogueEntry> GetAll(DialogueCategory category)
        {
            var result = new List<DialogueEntry>(8);

            DialogueListSO baseList = category switch
            {
                DialogueCategory.Delivered => delivered,
                DialogueCategory.Perfect   => perfect,
                DialogueCategory.Repeating => repeating,
                DialogueCategory.Wrong     => wrong,
                _ => null
            };
            if (baseList?.lines != null) result.AddRange(baseList.lines);

            List<DialogueEntry> extras = category switch
            {
                DialogueCategory.Asking    => extraAsking,
                DialogueCategory.Delivered => extraDelivered,
                DialogueCategory.Perfect   => extraPerfect,
                DialogueCategory.Repeating => extraRepeating,
                DialogueCategory.Wrong     => extraWrong,
                _ => null
            };
            if (extras != null) result.AddRange(extras);

            return result;
        }
        
        public string PickRandom(DialogueCategory category)
        {
            var list = GetAll(category);
            if (list == null || list.Count == 0) return null;

            int total = 0;
            for (int i = 0; i < list.Count; i++) total += Mathf.Max(1, list[i].weight);

            int roll = UnityEngine.Random.Range(0, total);
            for (int i = 0; i < list.Count; i++)
            {
                roll -= Mathf.Max(1, list[i].weight);
                if (roll < 0) return list[i].text;
            }
            return list[0].text;
        }
    }
}