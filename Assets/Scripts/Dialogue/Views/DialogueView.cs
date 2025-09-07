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
        
        [Header("Buttons")]
        [SerializeField] private UnityEvent onDeliverClicked;
        [SerializeField] private UnityEvent onRetryClicked;

        
        public void DeliverClicked() => onDeliverClicked?.Invoke();
        public void RetryClicked()   => onRetryClicked?.Invoke();


        /* Tal vez no es necesario
         
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
        */
        
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
            }
            else
            {
                dialogueService.Opened -= OnOpened;
                dialogueService.Closed -= OnClosed;
                dialogueService.LineChanged -= OnLineChanged;
                dialogueService.ClientChanged -= OnClientChanged;
                dialogueService.DeliveryToggled -= OnDeliveryToggled;
                dialogueService.RetryToggled -= OnRetryToggled;
            }
        }
        
        private void OnOpened() => panel?.SetActive(true);

        private void OnClosed()
        {
            panel?.SetActive(false);
            deliverButton?.SetActive(false);
            retryButton?.SetActive(false);
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
    }
    
}
