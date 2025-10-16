using Items.Core;
using UnityEngine;

// Owner: Ksa (UI/visuals)
// Purpose: UI-facing contract for choosing the correct sprite based on an item’s runtime preparation state.
// Notes: Implementations read (base SoItem + PreparationState) and return the best-fit sprite.
namespace Cooking.Ksa
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
