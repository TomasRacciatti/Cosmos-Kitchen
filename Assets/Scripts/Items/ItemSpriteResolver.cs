using Items.Core;
using UnityEngine;
using Cooking;

namespace Items
{
    public class ItemSpriteResolver : MonoBehaviour
    {
        public static Sprite Resolve(SoItem item, PreparationState prep)
        {
            if (item == null) return null;
            
            var set = item.VisualSet;
            if (set != null)
            {
                if (prep.method != CookingMethod.None)
                {
                    int doneness = Mathf.Clamp((int)prep.Doneness, 0, 4);
                    if (set.TryGet(prep.method, doneness, out var variant))
                        return variant;
                }
                
                if (set.DefaultSprite != null)
                    return set.DefaultSprite;
            }
            
            return null;
        }
        
        public static Sprite Resolve(ItemAmount itemAmount, Sprite currentSprite)
        {
            if (itemAmount == null || itemAmount.SoItem == null) return null;

            var so   = itemAmount.SoItem;
            var prep = itemAmount.Prep;
            var set  = so.VisualSet;
            
            if (prep.method == CookingMethod.None || prep.Doneness == Doneness.Raw)
            {
                if (currentSprite != null) return currentSprite;
                
                var hist = itemAmount.ProcessHistory;
                if (hist != null && hist.Count > 0)
                {
                    var last = hist[hist.Count - 1];
                    if (set != null)
                    {
                        int di = Mathf.Clamp(last.turns, 1, 4);
                        if (set.TryGet(last.method, di, out var fromHistory))
                            return fromHistory;
                    }
                }
                return (set != null) ? set.DefaultSprite : null;
            }
            
            if (set != null)
            {
                if (set.TryGet(prep.method, (int)prep.Doneness, out var variant))
                    return variant;

                if (currentSprite != null) return currentSprite;
                if (set.DefaultSprite != null) return set.DefaultSprite;
            }
            return currentSprite;
        }
    }
}
