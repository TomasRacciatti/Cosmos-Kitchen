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
        [SerializeField] private Image starIcon;

        [Header("Amount Outline")]
        [SerializeField] private Color amountOutlineColor = Color.white;
        [Tooltip("TextMeshPro outline width (0-1). Keeps the stack number readable on dark sprites.")]
        [SerializeField, Range(0f, 1f)] private float amountOutlineWidth = 0.25f;

        private static Material _amountOutlineMaterial;

        //Getters
        public ItemAmount ItemAmount => itemAmount;
        public InvSlotUI SlotUI => _invSlotUI;
        public Sprite Sprite => image != null ? image.sprite : null;
        
        //Privates
        private InvSlotUI _invSlotUI;
        private Canvas _canvas;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.sortingOrder = 10;
            amountText.raycastTarget = false;
            ApplyAmountOutline();
        }

        // One shared material for every item UI, so slots keep batching together.
        private void ApplyAmountOutline()
        {
            if (_amountOutlineMaterial == null)
            {
                _amountOutlineMaterial = new Material(amountText.fontSharedMaterial);
                _amountOutlineMaterial.EnableKeyword(ShaderUtilities.Keyword_Outline);
                _amountOutlineMaterial.SetColor(ShaderUtilities.ID_OutlineColor, amountOutlineColor);
                _amountOutlineMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, amountOutlineWidth);
                ShaderUtilities.UpdateShaderRatios(_amountOutlineMaterial);
            }
            amountText.fontSharedMaterial = _amountOutlineMaterial;
        }

        public void SetSlotUI(InvSlotUI invSlotUI)
        {
            _invSlotUI = invSlotUI;
        }
        
        public void SetItem(ItemAmount newItemAmount)
        {
            Sprite previousSprite = image.sprite;
            itemAmount = newItemAmount;
            if (image)
            {
                image.enabled = true;
                image.sprite = Items.ItemSpriteResolver.Resolve(itemAmount, null);
            }
            RefreshAmount();

            bool isPlate = newItemAmount.SoItem is SoPlate;
            starIcon.sprite = isPlate ? newItemAmount.GetRatingSprite : null;
            starIcon.enabled = isPlate && newItemAmount.GetRatingSprite != null;
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
            ItemsTooltip.Hide();
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
            _canvas.sortingOrder = 10;
            transform.localPosition = Vector3.zero;
            ItemsDropper.Hide();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        _invSlotUI.InvView.InventorySystem.SplitItemStack(SlotUI.InvSlot);
                        ItemsTooltip.Updated();
                    }
                    else if (Input.GetKey(KeyCode.LeftShift))
                    {
                        _invSlotUI.InvView.InventorySystem.SplitOneItem(SlotUI.InvSlot);
                        ItemsTooltip.Updated();
                    }
                    break;
                case PointerEventData.InputButton.Right:
                    _invSlotUI.InvView.InventorySystem.TransferToOtherInventory(SlotUI.InvSlot);
                    ItemsTooltip.Updated();
                    break;
            }
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