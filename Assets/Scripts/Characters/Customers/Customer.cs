using System;
using Characters.Clients;
using Characters.Player;
using Cinemachine;
using DialogueEditor;
using Interfaces;
using Items.Core;
using Managers;
using UnityEngine;

namespace Characters.Customers
{
    public class Customer : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private NPCSpeaker npcSpeaker;
        [SerializeField] private ClientSO soClient;
        [SerializeField] private CustomerSignal customerSignal;
        private NPCConversation conversation;
        private CustomerState state = CustomerState.Waiting;
        
        private enum CustomerState
        {
            Waiting,
            Ordered,
            Served
        }

        private void Awake()
        {
            conversation = GetComponent<NPCConversation>();
        }

        public void Interact(GameObject interactableObject)
        {
            ConversationManager.Instance.StartConversation(conversation);
            PlayerInputs.SetCursor(true);
            GameManager.Player.SetMoveActive(false);
            GameManager.Player.SetCamera(cinemachineVirtualCamera);
            GameManager.Player.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
            GameManager.Canvas.InvSlotUI.gameObject.SetActive(false);
            if (state == CustomerState.Ordered) ConversationManager.Instance.SetBool("Requested", true);
            if (state == CustomerState.Served) ConversationManager.Instance.SetBool("Served", true);
        }

        public void LeaveInteraction()
        {
            PlayerInputs.SetCursor(false);
            GameManager.Player.SetMoveActive(true);
            GameManager.Player.SetThirdPersonCamera();
            GameManager.Canvas.InvSlotUI.gameObject.SetActive(false);
            GameManager.Canvas.InvManager.ForceInventory(false);
        }

        public void EnableInteract()
        {
            
        }

        public void DisableInteract()
        {
            
        }

        public void Speak(string text)
        {
            SpeakerManager.Speak(text, npcSpeaker);
        }

        public void ShowSlot()
        {
            GameManager.Canvas.InvManager.ForceInventory(true);
            GameManager.Canvas.InvSlotUI.gameObject.SetActive(true);
        }

        public void SetRequested()
        {
            state = CustomerState.Ordered;
            customerSignal.SetSignal(1);
        }

        public void TestPlate()
        {
            var itemTested = GameManager.Canvas.InvSystem.Items[0];
            if (!itemTested.IsEmpty && soClient.requestedSoPlate == itemTested.SoItem)
            {
                ConversationManager.Instance.SetInt("Quality", itemTested.Rating);
                if (itemTested.Rating >= 3)
                {
                    customerSignal.SetSignal(-1);
                    state = CustomerState.Served;
                }
            }
            else
            {
                ConversationManager.Instance.SetInt("Quality", -1);
                customerSignal.SetSignal(1);
            }
            GameManager.Canvas.InvSystem.ClearSlot(0);
            GameManager.Canvas.InvSlotUI.gameObject.SetActive(false);
            GameManager.Canvas.InvManager.ForceInventory(false);
        }

        public Transform InteractionPoint => transform;
    }
}