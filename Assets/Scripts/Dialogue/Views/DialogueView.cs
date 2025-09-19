using System;
using Characters.Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Dialogue.Views
{
    public class DialogueView : ServiceView
    {
        [Header("Panel & Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI dialogueTMP;
        [SerializeField] private TextMeshProUGUI clientNameTMP;
        [SerializeField] private Image clientPortrait;
        [SerializeField] private GameObject deliverButton;
        [SerializeField] private GameObject retryButton;
        [SerializeField] private GameObject plateReceiver;
        
        [Header("Buttons")]
        [SerializeField] private UnityEvent onDeliverClicked = new UnityEvent();
        [SerializeField] private UnityEvent onRetryClicked = new UnityEvent();

        
        public void DeliverClicked() => onDeliverClicked?.Invoke();
        public void RetryClicked()   => onRetryClicked?.Invoke();


        private void Awake()
        {
            panel?.SetActive(false);
            deliverButton?.SetActive(false);
            retryButton?.SetActive(false);
            plateReceiver?.SetActive(false);
            if (dialogueTMP) dialogueTMP.text = "";
            if (clientNameTMP) clientNameTMP.text = "";
            if (clientPortrait) clientPortrait.sprite = null;
        }
        
        
        //Por si el UI se corre antes que el service, esto lo va a rescatar
        protected override void OnEnable() 
        {
            base.OnEnable();
            
            if (dialogueService == null) return;
            if (dialogueService.IsOpen)
                OnOpened();
            
            var c = dialogueService.CurrentClient;
            if (c != null) 
                OnClientChanged(new ClientInfo(c.Name, c.Icon));
        }
        
        protected override void Subscribe(bool on)
        {
            if (on)
            {
                dialogueService.Opened += OnOpened;
                dialogueService.Closed += OnClosed;
                dialogueService.LineChanged += OnLineChanged;
                dialogueService.ClientChanged += OnClientChanged;
                dialogueService.DeliveryToggled += OnDeliveryToggled;
                dialogueService.RetryToggled += OnRetryToggled;
                dialogueService.PlateReceiverToggled += OnPlateReceiverToggled;
            }
            else
            {
                dialogueService.Opened -= OnOpened;
                dialogueService.Closed -= OnClosed;
                dialogueService.LineChanged -= OnLineChanged;
                dialogueService.ClientChanged -= OnClientChanged;
                dialogueService.DeliveryToggled -= OnDeliveryToggled;
                dialogueService.RetryToggled -= OnRetryToggled;
                dialogueService.PlateReceiverToggled -= OnPlateReceiverToggled;
            }
        }
        
        private void OnOpened() 
        {
            panel?.SetActive(true);
            plateReceiver?.SetActive(true);
            PlayerInputs.SetCursor(true);
        }

        private void OnClosed()
        {
            panel?.SetActive(false);
            deliverButton?.SetActive(false);
            retryButton?.SetActive(false);
            plateReceiver?.SetActive(false);
            PlayerInputs.SetCursor(false);
        }

        private void OnLineChanged(string line)
        {
            if (dialogueTMP)
                dialogueTMP.text = line ?? "Error. No dialogue set";
        }

        private void OnClientChanged(ClientInfo info)
        {
            if (clientNameTMP)
                clientNameTMP.text = info.Name ?? "John Doe";
            if (clientPortrait)
                clientPortrait.sprite = info.Icon;
        }

        void OnDeliveryToggled(bool on) => deliverButton?.SetActive(on);
        void OnRetryToggled(bool on)    => retryButton?.SetActive(on);
        private void OnPlateReceiverToggled(bool on) => plateReceiver?.SetActive(on);
    }
}
