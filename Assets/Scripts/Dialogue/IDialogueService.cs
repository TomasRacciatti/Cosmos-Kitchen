using System;
using UnityEngine;

namespace Dialogue
{
    public interface IDialogueService
    {
        bool IsOpen { get; }
        IClient CurrentClient { get; }
    
        event Action Opened;
        event Action Closed;
        event Action<string> LineChanged;
        event Action<ClientInfo> ClientChanged;
        event Action<bool> DeliveryToggled;
        event Action<bool> RetryToggled;
        event Action<bool> PlateReceiverToggled;
        event Action<string> Notified;
    
        void StartConversation(IClient client);
        void Close();
        void SetLine(string line);
        void EnableDelivery(bool enable);
        void EnableRetry(bool enable);
        void TogglePlateReceiver(bool show);
        void Notify(string notification);
    }
    
    public readonly struct ClientInfo
    {
        public readonly string Name;
        public readonly Sprite Icon;
        public ClientInfo(string name, Sprite icon) { Name = name; Icon = icon; }
    }
    
    public interface IClient
    {
        string Name { get; }
        Sprite Icon { get; }
        void RetryRequest();
    }
}
