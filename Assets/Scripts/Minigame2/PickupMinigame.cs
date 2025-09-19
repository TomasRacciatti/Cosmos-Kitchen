using Items.Inventory;
using UnityEngine;

namespace Minigame2
{
    public class PickupMinigame : MinigameTrigger
    {
        protected override void EnterMiniGame()
        {
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