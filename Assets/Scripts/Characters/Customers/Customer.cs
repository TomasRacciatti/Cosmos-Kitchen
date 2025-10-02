using System;
using Characters.Clients;
using Characters.Player;
using Cinemachine;
using DialogueEditor;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Customers
{
    public class Customer : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private NPCSpeaker npcSpeaker;
        private NPCConversation conversation;

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
        }

        public void LeaveInteraction()
        {
            PlayerInputs.SetCursor(false);
            GameManager.Player.SetMoveActive(true);
            GameManager.Player.SetThirdPersonCamera();
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
            //GameManager.Canvas
        }

        public Transform InteractionPoint => transform;
    }
}