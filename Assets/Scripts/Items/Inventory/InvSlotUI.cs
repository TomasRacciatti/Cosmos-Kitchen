using Items.Core;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Items.Inventory
{
    public class InvSlotUI : MonoBehaviour, IDropHandler
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
        
        public void SetItem(ItemAmount itemAmount) //cambiar con object pool si despues se puede
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
                GameObject newItem = Instantiate(PrefabsManager.ItemPrefabUI, Vector3.zero, Quaternion.identity);
                itemUI = newItem.GetComponent<InvItemUI>();
                itemUI.SetSlotUI(this);
                newItem.transform.SetParent(transform, false);
                newItem.transform.localPosition = Vector3.zero;
                newItem.transform.localRotation = Quaternion.identity;
            }

            itemUI.SetItem(itemAmount);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            InvItemUI fromItemUI = eventData.pointerDrag.GetComponent<InvItemUI>();
            InvSlotUI fromSlotUI = fromItemUI.SlotUI;
            ItemsDropper.Hide();
            ItemsTooltip.Show(fromItemUI.ItemAmount);

            if (fromSlotUI == null) return;

            fromSlotUI.InvView.InventorySystem.TransferIndexToIndex(InvView.InventorySystem, fromSlotUI.InvSlot, InvSlot);
        }
    }
}