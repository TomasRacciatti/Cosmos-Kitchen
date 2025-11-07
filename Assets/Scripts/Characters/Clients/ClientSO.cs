using UnityEngine;
using Characters.Clients.ClientDialogue;
using Characters.Clients.Plates;
using Items.Core;
using UnityEngine.Serialization;

namespace Characters.Clients
{
    [CreateAssetMenu(menuName = "ScriptableObject/Client", fileName = "Client_")]
    public class ClientSO : ScriptableObject
    {
        [Header("Client")]
        public string clientName;
        public Sprite clientIcon;
        public bool isCritic;

        [Header("Dialogue")]
        public ClientDialogueProfileSO dialogueProfile;

        [Header("Order / Recipe")]
        public SoPlate requestedSoPlate;
        public string clue1 = "Something with";
        public string clue2 = "";
        
        public string GetLine(DialogueCategory category) =>
            dialogueProfile?.PickRandom(category) ?? string.Empty;
    }
}
