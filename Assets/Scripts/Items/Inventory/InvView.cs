using Items.Core;
using UnityEngine;

namespace Items.Inventory
{
    public class InvView : MonoBehaviour
    {
        [SerializeField] protected InvSlotUI[] slots;
        [SerializeField] protected InvSystem inventorySystem;
        
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
            inventorySystem.Subscribe(OnItemChanged);
        }

        private void OnItemChanged(int index, ItemAmount item)
        {
            if (index < 0 || index >= slots.Length) return;
            //slots[index].SetItem(item);
        }
    }
}