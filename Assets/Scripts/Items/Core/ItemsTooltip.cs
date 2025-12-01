using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Items.Core
{
    public class ItemsTooltip : MonoBehaviour
    {
        private static ItemsTooltip _instance;
        
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text itemDescriptionText;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Image starIcon;
        
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private ItemAmount _itemAmount;
        
        private void Awake()
        {
            _instance = this;
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            Hide();
        }

        public static void Show(ItemAmount itemAmount)
        {
            if (ItemsDropper.IsActive) return;
            _instance._itemAmount = itemAmount;
            Updated();
        }

        public static void Updated()
        {
            if (_instance._itemAmount.IsEmpty)
            {
                Hide();
                return;
            }
            
            _instance.itemNameText.text = _instance._itemAmount.SoItem.ItemName;
            _instance.itemDescriptionText.text = _instance._itemAmount.GetProcessHistoryText();
            _instance.itemIcon.sprite = ItemSpriteResolver.Resolve(_instance._itemAmount, null);
            _instance.gameObject.SetActive(true);
            _instance.UpdatePositionAndPivot();

            bool isPlate = _instance._itemAmount.SoItem is SoPlate;
            _instance.starIcon.sprite = isPlate ? _instance._itemAmount.GetRatingSprite : null;
            _instance.starIcon.enabled = isPlate && _instance._itemAmount.GetRatingSprite != null;
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdatePositionAndPivot();
        }
        
        private void UpdatePositionAndPivot()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            AdjustPivotToStayOnScreen(mouseScreenPos);

            RectTransform parentRect = _rectTransform.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                mouseScreenPos,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out var anchoredPos);
            
            _rectTransform.anchoredPosition = anchoredPos;
        }
        
        private void AdjustPivotToStayOnScreen(Vector2 screenPos)
        {
            Vector2 pivot = new Vector2(
                screenPos.x / Screen.width < 0.5f ? 0f : 1f,
                screenPos.y / Screen.height < 0.5f ? 0f : 1f
            );

            _rectTransform.pivot = pivot;
        }
    }
}
