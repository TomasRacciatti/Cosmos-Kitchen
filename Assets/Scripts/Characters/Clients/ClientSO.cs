using UnityEngine;
using Characters.Clients.ClientDialogue;
using Characters.Clients.Recipes;

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
        public RecipeSO requestedRecipe;

        // Helper que devuelve el string empty si no hay lineas de dialogo.
        public string GetLine(DialogueCategory category) =>
            dialogueProfile?.PickRandom(category) ?? string.Empty;
    }
}
