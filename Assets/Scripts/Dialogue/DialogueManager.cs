using System;
using UnityEngine;

namespace Dialogue
{
    [DefaultExecutionOrder(-100)] // Queremos que esto se corra antes que los views
    public class DialogueManager : MonoBehaviour, IDialogueService
    {
        public static DialogueManager instance { get; private set; } // Legacy. Esto lo ponemos para que no se rompa el codigo viejo
    
        public static IDialogueService Instance => instance; // Eventualmente solo vamos a querer usar este
    
        public bool IsOpen { get; private set; }
        public IClient CurrentClient { get; private set; }

    
        public event Action Opened;
        public event Action Closed;
        public event Action<string> LineChanged;
        public event Action<ClientInfo> ClientChanged;
        public event Action<bool> DeliveryToggled;
        public event Action<bool> RetryToggled;
        public event Action<bool> PlateReceiverToggled;
        public event Action<bool> InteractionPromptToggled;
        public event Action<string> Notified;

        private void Awake()
        {
            if (instance == null) 
                instance = this;
            else
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void StartConversation(IClient client)
        {
            CurrentClient = client;
            IsOpen = true;
            ClientChanged?.Invoke(new ClientInfo(client?.Name ?? string.Empty, client?.Icon));
            InteractionPromptToggled?.Invoke(false); // Legacy
            Opened?.Invoke();
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            Closed?.Invoke();
            DeliveryToggled?.Invoke(false);
            RetryToggled?.Invoke(false);
            PlateReceiverToggled?.Invoke(false);
            InteractionPromptToggled?.Invoke(false);
            CurrentClient = null;
        }
    
        public void SetLine(string line) => LineChanged?.Invoke(line ?? string.Empty);
        public void EnableDelivery(bool enable) => DeliveryToggled?.Invoke(enable);
        public void EnableRetry(bool enable) => RetryToggled?.Invoke(enable);
        public void TogglePlateReceiver(bool show) => PlateReceiverToggled?.Invoke(show);
        public void Notify(string notification) => Notified?.Invoke(notification ?? string.Empty);


        #region Legacy. Borrar cuando no genere errores
    
        // M (Model) -> Este codigo
        // V (View) -> DialogueView, InteractionPromptView, etc son todos los scripts que manejan los distintos elementos del canvas
        // C (Controller) -> Van a ser las condiciones que llaman a las funciones del M para activar al V.
    
        // === Legacy methods. Borrar esto cuando no tengamos mas las refs. ===
        [Obsolete("Use InteractionPromptToggled event via a view")]
        public void ShowInteraction() => InteractionPromptToggled?.Invoke(true);

        [Obsolete("Use InteractionPromptToggled event via a view")]
        public void HideInteraction() => InteractionPromptToggled?.Invoke(false);

        [Obsolete("Controller should call StartConversation(IClient)")]
        public void SetClient(ClientScript c)
        {
            CurrentClient = c;
            ClientChanged?.Invoke(new ClientInfo(c?.ReturnClientName() ?? "", c?.ReturnClientIcon()));
        }

        [Obsolete("Use SetLine(string)")]
        public void ChangeDialogue(string t) => SetLine(t);

        [Obsolete("Use EnableDelivery(true) + TogglePlateReceiver(true)")]
        public void ShowDeliveryButton() { EnableDelivery(true); TogglePlateReceiver(true); }

        [Obsolete("Use EnableDelivery(false)")]
        public void HideDeliveryButton() => EnableDelivery(false);

        [Obsolete("Use EnableRetry(true)")]
        public void ShowRetryButton() => EnableRetry(true);

        [Obsolete("Use TogglePlateReceiver(true/false)")]
        public void OpenPlateReceiver() => TogglePlateReceiver(true);

        [Obsolete("Use TogglePlateReceiver(false) + EnableDelivery(false)")]
        public void ClosePlateReceiver() { TogglePlateReceiver(false); EnableDelivery(false); }

        [Obsolete("Use Close()")]
        public void CloseDialogue() => Close();

        [Obsolete("Controller should own this flow; here it's a no-op")]
        public void SwitchDialogue()
        {
            if (IsOpen) Close();
            else if (CurrentClient != null) StartConversation(CurrentClient);
        }

        [Obsolete("Model no longer fetches plates")]
        public void GetPlate() { }

        [Obsolete("Outcome should be handled in gameplay logic")]
        public void CorrectPlate() { }
    
        #endregion
    }
}
