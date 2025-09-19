using Items.Inventory;
using UnityEngine;

namespace Minigame2
{
    public class PickupMinigame : MinigameTrigger
    {
        public override void Interact(GameObject interactableObject)
        {
            if (!Cooldown.IsReady) return;
            RewardPlayer(3);
            StartCooldown();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<InvSystem>(out InvSystem invSystem))
            {
                Interact(invSystem.gameObject);
            }
        }
    }
}