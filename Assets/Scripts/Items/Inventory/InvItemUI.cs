using Items.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Items.Inventory
{
    public class InvItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //Variables
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private ItemAmount itemAmount;
        
        //Getters
        public ItemAmount ItemAmount => itemAmount;
        public InvSlotUI SlotUI => _invSlotUI;
        
        //Privates
        private InvSlotUI _invSlotUI;
        private Canvas _canvas;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.sortingOrder = 10;
        }
        
        public void SetItem(ItemAmount newItemAmount)
        {
            itemAmount = newItemAmount;
            image.sprite = itemAmount.SoItem.Image;
            RefreshAmount();
        }
        
        private void RefreshAmount()
        {
            amountText.SetText(itemAmount.Amount.ToString());
            amountText.gameObject.SetActive(itemAmount.Amount > 1);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            image.raycastTarget = false;
            _canvas.sortingOrder = 15;
            ItemsDropper.Show();
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera, out Vector2 localPoint);
            transform.localPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            image.raycastTarget = true;
            _canvas.sortingOrder = 5;
            transform.localPosition = Vector3.zero;
            ItemsDropper.Hide();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    break;
                case PointerEventData.InputButton.Right:
                    SplitItem();
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
            }
        }

        private void SplitItem()
        {
            InvSlotUI slotUI = GetComponentInParent<InvSlotUI>();
            //slotUI.InventoryUI.InventorySystem.SplitItemStack(slotUI.SlotIndex);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            ItemsTooltip.Show(itemAmount);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ItemsTooltip.Hide();
        }
    }
}