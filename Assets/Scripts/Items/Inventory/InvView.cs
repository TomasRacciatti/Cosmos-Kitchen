using Items.Core;
using Managers;
using UnityEngine;

namespace Items.Inventory
{
    public class InvView : MonoBehaviour
    {
        [SerializeField] protected InvSlotUI[] slots;
        [SerializeField] protected InvSystem inventorySystem;
        [SerializeField] private bool usePlayerInventoryAsDefault = false;
        
        public InvSystem InventorySystem => inventorySystem;

        private void Awake()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Initialize(this, i);
            }
        }

        private void Start()
        {
            if (inventorySystem == null && usePlayerInventoryAsDefault) 
                inventorySystem = GameManager.Player.GetComponent<InvSystem>();

            if (inventorySystem != null) 
                SetInventory(inventorySystem);
        }
        
        public void SetInventory(InvSystem newInventory)
        {
            if (inventorySystem != null)
                inventorySystem.Unsubscribe(OnItemChanged);

            inventorySystem = newInventory;

            if (inventorySystem == null) return;

            inventorySystem.Subscribe(OnItemChanged);
            UpdateInventory();
        }

        private void OnItemChanged(int index, ItemAmount item)
        {
            if (index < 0 || index >= slots.Length) return;
            slots[index].SetItem(item);
        }

        private void UpdateInventory()
        {
            var items = inventorySystem.Items;

            for (int i = 0; i < slots.Length; i++)
            {
                if (i < items.Count)
                    slots[i].SetItem(items[i]);
                //else
                    //slots[i].Clear();
            }
        }
    }
}