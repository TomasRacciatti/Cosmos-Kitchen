using System;
using Items.Inventory;
using UnityEngine;

namespace MiniGames
{
    public class TriggerMiniGame : MiniGame
    {
        public override void Interact(GameObject interactableObject)
        {
            if (!Cooldown.IsReady) return;
            
            RewardPlayer();
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