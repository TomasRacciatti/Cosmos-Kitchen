using System;
using Items.Core;
using UnityEngine;

// Purpose: Asset that holds per-method, per-doneness sprite variants for a given SoItem.
// Notes: Keeping this separate from SoItem reduces merge conflicts; SoItem keeps its default sprite.
namespace Cooking
{
    [CreateAssetMenu(menuName = "ScriptableObject/Items/Item Visual Set", fileName = "SoItemVisualSet")]
    public class SoItemVisualSet : ScriptableObject
    { 
        [Header("Target Item")]
        [SerializeField] private SoItem item;
        [SerializeField] private Sprite defaultSprite;
    
        [Header("Variants by Method (index 0..4 = Raw..Burnt)")]
        [SerializeField] private MethodVariants[] variants;
    
        public SoItem Item => item;
        public Sprite DefaultSprite => defaultSprite;
    
        [Serializable]
        public class MethodVariants
        {
            public CookingMethod currentMethod;
            public bool usesPreviousMethod = false;
            public CookingMethod previousMethod;
            [Tooltip("0 = Rare, 1 = Medium, 2 = WellDone, 3 = Burnt")]
            public Sprite[] donenessSprites = new Sprite[4];
        }

        public bool TryGet(CookingMethod method, int donenessIndex, out Sprite sprite)
        {
            return TryGet(method, donenessIndex, CookingMethod.None, out sprite); 
        }

        public bool TryGet(CookingMethod currentMethod, int donenessIndex, CookingMethod previousMethod, out Sprite sprite)
        {
            sprite = null;
            if (variants == null) return false;
        
            if (donenessIndex <= 0) return false;
        
            int idx = donenessIndex - 1;
            if (idx < 0 || idx >= 4) return false;

            if (previousMethod != CookingMethod.None)
            {
                for (int i = 0; i < variants.Length; i++)
                {
                    var variant = variants[i];
                    if (variant.currentMethod != currentMethod) continue;
                    if (!variant.usesPreviousMethod) continue;
                    if (variant.previousMethod != previousMethod) continue;
                
                    var spriteArray = variant.donenessSprites;
                    if (spriteArray != null && idx < spriteArray.Length)
                    {
                        sprite = spriteArray[idx];
                        if (sprite != null) return true;
                    }
                }
            }

            for (int i = 0; i < variants.Length; i++)
            {
                var variant = variants[i];
                if (variant.currentMethod != currentMethod) continue;
                if (variant.usesPreviousMethod) continue;
            
                var spriteArray = variant.donenessSprites;
                if (spriteArray != null && idx < spriteArray.Length)
                {
                    sprite = spriteArray[idx];
                    if (sprite != null) return true;
                }
            }
        
            return false;
        }
    }
}

