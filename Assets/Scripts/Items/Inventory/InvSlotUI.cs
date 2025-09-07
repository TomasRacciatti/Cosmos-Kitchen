using Items.Core;
using UnityEngine;

namespace Items.Inventory
{
    public class InvSlotUI : MonoBehaviour
    {
        private InvView invView;
        private int invSlot;
        
        public InvView InvView => invView;
        public int InvSlot => invSlot;
        
        public void Initialize(InvView inventoryView, int inventorySlot)
        {
            invView = inventoryView;
            invSlot = inventorySlot;
        }
        
        /*
        public void SetItem(ItemAmount itemAmount)
        {
            InvItemUI itemUI = GetComponentInChildren<InvItemUI>();
            if (itemAmount.IsEmpty)
            {
                if (itemUI != null)
                {
                    Destroy(itemUI.gameObject);
                }

                return;
            }

            if (itemUI == null)
            {
                GameObject newItem = Instantiate(GameManager.Canvas.inventoryManager.itemSlotPrefab, transform);
                itemUI = newItem.GetComponent<InvItemUI>();
            }

            itemUI.SetItem(itemAmount);
        }*/
    }
}