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
        
        public void SetItem(ItemAmount itemAmount)
        {
            InvItemUI itemUI = GetComponentInChildren<InvItemUI>();
            if (itemAmount.IsEmpty)
            {
                if (itemUI != null)
                {
                    ObjectPool.ReturnObjectToPool(gameObject);
                }
                return;
            }

            if (itemUI == null)
            {
                GameObject newItem = ObjectPool.SpawnObject(PrefabsManager.ItemPrefabUI, Vector3.zero, Quaternion.identity);
                itemUI = newItem.GetComponent<InvItemUI>();
                //falta asignar padre
            }

            itemUI.SetItem(itemAmount);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            InvItemUI fromItemUI = eventData.pointerDrag.GetComponent<InvItemUI>();
            InvSlotUI fromSlotUI = fromItemUI.GetComponentInParent<InvSlotUI>();
            ItemsDropper.Hide();
            ItemsTooltip.Show(fromItemUI.ItemAmount);

            if (fromSlotUI == null) return;

            //fromSlotUI.InventoryUI.InventorySystem.TransferIndexToIndex(InventoryUI.InventorySystem, fromSlotUI.SlotIndex,SlotIndex);
        }
    }
}