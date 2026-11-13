using Managers;
using UnityEngine;

namespace Items.Inventory
{
    // Goes on the Player, next to its InvSystem: each scene has its own Player, so the contents are carried over on travel.
    [RequireComponent(typeof(InvSystem))]
    public class CarryOverInventory : MonoBehaviour
    {
        private InvSystem _inventory;
        private bool _subscribed;

        private void Awake()
        {
            _inventory = GetComponent<InvSystem>();
        }

        // Start, so InvSystem.Awake (which builds the slots) has already run.
        private void Start()
        {
            if (GameManager.Scenes != null)
            {
                GameManager.Scenes.LoadStarting += StoreItems;
                _subscribed = true;
            }

            var carried = GameManager.CarriedInventory;
            if (carried != null && carried.TryTake(out var items)) _inventory.LoadItems(items);
        }

        private void OnDestroy()
        {
            if (_subscribed && GameManager.Scenes != null) GameManager.Scenes.LoadStarting -= StoreItems;
        }

        private void StoreItems() => GameManager.CarriedInventory?.Store(_inventory.Items);
    }
}
