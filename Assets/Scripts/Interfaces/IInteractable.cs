using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
    {
        public void Interact(GameObject interactableObject);
        
        public void EnableInteract();
        public void DisableInteract();
    }
}