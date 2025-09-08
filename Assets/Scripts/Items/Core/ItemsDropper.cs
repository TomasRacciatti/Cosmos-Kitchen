using Items.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Items.Core
{
    public class ItemsDropper : MonoBehaviour, IDropHandler
    {
        private static ItemsDropper _instance;
        
        public static bool IsActive => _instance.gameObject.activeSelf;

        private void Awake()
        {
            _instance = this;
            Hide();
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            InvItemUI fromItemUI = eventData.pointerDrag.GetComponent<InvItemUI>();
            if (fromItemUI == null) return;
            InvSlotUI fromSlotUI = fromItemUI.SlotUI;
            Drop(fromItemUI.ItemAmount);
            Hide();
        }

        public static void Drop(ItemAmount itemAmount)
        {
            //fromSlotUI.InvView.InventorySystem.SetItemByIndex(fromSlotUI.InvSlot, new ItemAmount());
            /*
            GameObject itemObject = Instantiate(_instance.itemPrefab, GameManager.Player.transform.position + 1.2f * Vector3.up,
                Quaternion.identity);
            itemObject.GetComponent<ItemPrefab>().SetItemAmount(new ItemAmount(itemAmount));
            
            Rigidbody rb = itemObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((GameManager.Player.transform.forward + 0.8f * Vector3.up).normalized * 3.5f, ForceMode.Impulse);
            }*/
        }
    }
}
