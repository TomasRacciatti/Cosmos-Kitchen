using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using Characters.Clients.ClientDialogue;
using Interfaces;
using Items.Core;
using Characters.Clients.Plates;

namespace Characters.Clients
{
    [RequireComponent(typeof(Collider))]
    public class ClientController  : MonoBehaviour, IClient, IInteractable
    {
        [Header("Data")]
        [SerializeField] private ClientSO clientData;
        
        [Header("Visuals")]
        [SerializeField] Animator animator;
        [SerializeField] GameObject signal;
        [SerializeField] Sprite questionSprite;

        private IDialogueService dialogue;
        private IOrderVerifier verifier;
        
        // State
        private bool hasSpoken;
        private bool deliveryComplete;
        private bool perfectDelivery;

        // ===== IClient (for Dialogue) =====
        public string Name  => clientData != null ? clientData.clientName : string.Empty;
        public Sprite Icon  => clientData != null ? clientData.clientIcon : null;
        public void RetryRequest() => Retry();
        

        // ===== IInteractable =====
        public Transform InteractionPoint => transform;
        public void EnableInteract()  { }
        public void DisableInteract() { }
    
        private void Awake()
        {
            dialogue = DialogueManager.Instance;
            verifier = new PlainOrderVerifier();
            if (signal) signal.SetActive(true);
            
            // Warnings de setteo
            if (clientData == null) Debug.LogWarning($"{name}: ClientSO not set.");
            if (clientData.requestedSoPlate == null) Debug.LogWarning($"{name}: requestedPlate not set.");
        }

        private void OnDisable()
        {
            if (dialogue != null && dialogue.IsOpen && dialogue.CurrentClient == this)
                dialogue.Close();
        }
        
        public void Interact(GameObject interactor)
        {
            if (dialogue == null) return;
            
            if (dialogue.IsOpen && dialogue.CurrentClient == this)
            {
                dialogue.Close();
            }
            else
            {
                OpenConversation();
            }
        }

        private void OpenConversation()
        {
            dialogue.StartConversation(this);
            
            if (deliveryComplete)
            {
                dialogue.SetLine(clientData.GetLine(DialogueCategory.Repeating));
                dialogue.EnableDelivery(false);
                dialogue.EnableRetry(true);
                dialogue.TogglePlateReceiver(false);
                return;
            }
            
            if (!hasSpoken)
            {
                SetSignalQuestion();
                hasSpoken = true;
            }

            dialogue.SetLine(clientData.GetLine(DialogueCategory.Asking));
            dialogue.EnableRetry(false);
            dialogue.EnableDelivery(true);
            dialogue.TogglePlateReceiver(true);
        }

        public void Deliver(ItemAmount delivered)
        {
            if (dialogue == null || clientData == null || clientData.requestedSoPlate == null)
                return;
            
            var outcome = verifier.Verify(clientData.requestedSoPlate, delivered);
            
            switch (outcome)
            {
                case OrderOutcome.Wrong:
                    deliveryComplete = false;
                    perfectDelivery  = false;
                    dialogue.SetLine(clientData.GetLine(DialogueCategory.Wrong));
                    dialogue.EnableDelivery(true);
                    dialogue.EnableRetry(false);
                    dialogue.TogglePlateReceiver(true);
                    SetSignalQuestion();
                    // animacion
                    break;

                case OrderOutcome.Delivered:
                    deliveryComplete = true;
                    perfectDelivery  = false;
                    dialogue.SetLine(clientData.GetLine(DialogueCategory.Delivered));
                    dialogue.EnableDelivery(false);
                    dialogue.EnableRetry(true);
                    dialogue.TogglePlateReceiver(false);
                    if (signal) signal.SetActive(false);
                    // animacion
                    break;

                case OrderOutcome.Perfect:
                    deliveryComplete = true;
                    perfectDelivery  = true;
                    dialogue.SetLine(clientData.GetLine(DialogueCategory.Perfect));
                    dialogue.EnableDelivery(false);
                    dialogue.EnableRetry(false);
                    dialogue.TogglePlateReceiver(false);
                    if (signal) signal.SetActive(false);
                    // animacion
                    break;
            }
        }
        
        public void Retry()
        {
            deliveryComplete = false;
            perfectDelivery  = false;

            if (!dialogue.IsOpen || dialogue.CurrentClient != this)
                dialogue.StartConversation(this);

            dialogue.SetLine(clientData.GetLine(DialogueCategory.Asking));
            dialogue.EnableRetry(false);
            dialogue.EnableDelivery(true);
            dialogue.TogglePlateReceiver(true);
            SetSignalQuestion();
        }
        
        private void SetSignalQuestion()
        {
            if (!signal) return;
            var img = signal.GetComponent<Image>();
            if (img && questionSprite) img.sprite = questionSprite;
            signal.SetActive(true);
        }
    }
}
