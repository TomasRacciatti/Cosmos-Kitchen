using Items.Core;
using UnityEngine;

namespace UI.Trash
{
    public readonly struct TrashedItem
    {
        public readonly ItemAmount Item;
        public readonly Sprite Sprite;
        public readonly Vector2 ScreenPosition;
        public readonly Vector2 ScreenSize;

        public TrashedItem(ItemAmount item, Sprite sprite, Vector2 screenPosition, Vector2 screenSize)
        {
            Item = item;
            Sprite = sprite;
            ScreenPosition = screenPosition;
            ScreenSize = screenSize;
        }
    }
}
