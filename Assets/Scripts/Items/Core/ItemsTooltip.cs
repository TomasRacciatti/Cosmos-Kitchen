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
            _instance.itemNameText.text = itemAmount.SoItem.ItemName;
            _instance.itemDescriptionText.text = itemAmount.GetProcessHistoryText();
            _instance.itemIcon.sprite = Items.ItemSpriteResolver.Resolve(itemAmount.SoItem, itemAmount.Prep);
            _instance.gameObject.SetActive(true);
            _instance.UpdatePositionAndPivot();
            _instance.starIcon.sprite = itemAmount.GetRatingSprite;
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
