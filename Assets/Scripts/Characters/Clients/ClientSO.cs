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

        [FormerlySerializedAs("requestedPlate")] [Header("Order / Recipe")]
        public SoPlate requestedSoPlate;

        // Helper que devuelve el string empty si no hay lineas de dialogo.
        public string GetLine(DialogueCategory category) =>
            dialogueProfile?.PickRandom(category) ?? string.Empty;
    }
}
