using Items.Core;
using UnityEngine;

namespace Cooking
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
    }
}
