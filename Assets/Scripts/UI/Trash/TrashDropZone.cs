using System;
using Items;
using Items.Core;
using Items.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Trash
{
    public class TrashDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Scaled up while an inventory item is dragged over the can. Defaults to this RectTransform.")]
        [SerializeField] private RectTransform hoverTarget;
        [SerializeField, Min(1f)] private float hoverScale = 1.08f;

        private static readonly Vector3[] Corners = new Vector3[4];

        public event Action<TrashedItem> ItemTrashed;

        private void Awake()
        {
            if (hoverTarget == null) hoverTarget = (RectTransform)transform;
        }

        private void OnDisable()
        {
            SetHovered(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHovered(TryGetDraggedItem(eventData, out _));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHovered(false);
        }

        public void OnDrop(PointerEventData eventData)
        {
            SetHovered(false);
            if (!TryGetDraggedItem(eventData, out InvItemUI itemUI)) return;

            InvSlotUI slot = itemUI.SlotUI;
            if (slot == null || slot.InvView == null || slot.InvView.InventorySystem == null) return;

            ItemsDropper.Hide();
            ItemsTooltip.Hide();

            // Copy first: clearing the slot empties the ItemAmount instance the item UI still points to.
            var trashed = new ItemAmount(itemUI.ItemAmount);
            Sprite sprite = ItemSpriteResolver.Resolve(trashed, itemUI.Sprite);
            Vector2 screenSize = ScreenSizeOf((RectTransform)itemUI.transform, eventData.pressEventCamera);

            slot.InvView.InventorySystem.SetItemByIndex(slot.InvSlot, new ItemAmount());

            ItemTrashed?.Invoke(new TrashedItem(trashed, sprite, eventData.position, screenSize));
        }

        private static bool TryGetDraggedItem(PointerEventData eventData, out InvItemUI itemUI)
        {
            itemUI = null;
            if (!eventData.dragging || eventData.pointerDrag == null) return false;
            if (!eventData.pointerDrag.TryGetComponent(out itemUI)) return false;
            return itemUI.ItemAmount != null && !itemUI.ItemAmount.IsEmpty;
        }

        private static Vector2 ScreenSizeOf(RectTransform rect, Camera eventCamera)
        {
            rect.GetWorldCorners(Corners);
            Vector2 min = RectTransformUtility.WorldToScreenPoint(eventCamera, Corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(eventCamera, Corners[2]);
            return new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));
        }

        private void SetHovered(bool hovered)
        {
            if (hoverTarget == null) return;
            hoverTarget.localScale = Vector3.one * (hovered ? hoverScale : 1f);
        }
    }
}
