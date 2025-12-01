using System;
using Items.Inventory;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items.Core
{
    public class ItemPickupTrigger : MonoBehaviour
    {
        [SerializeField] private ItemPickup itemPickup;

        private void Awake()
        {
            if (itemPickup == null) itemPickup.GetComponentInParent<ItemPickup>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out InvSystem invSystem)) return;
            if (!invSystem.canGrab) return;
            itemPickup.Interact(invSystem.gameObject);
        }
    }
}