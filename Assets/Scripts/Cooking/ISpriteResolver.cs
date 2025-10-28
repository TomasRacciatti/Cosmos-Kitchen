using Items.Core;
using UnityEngine;

namespace Cooking
{
    public interface ISpriteResolver
    {
        /// <summary>
        /// Returns the sprite that should represent this item instance in UI.
        /// If prepState is null or no variant exists, return the SoItem’s default sprite.
        /// </summary>
        Sprite ResolveSprite(SoItem baseItem, PreparationState? prepState);
    }
}
