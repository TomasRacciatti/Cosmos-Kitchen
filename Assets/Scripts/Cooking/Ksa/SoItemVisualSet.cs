using System;
using System.Collections;
using System.Collections.Generic;
using Cooking;
using Items.Core;
using UnityEngine;

// Owner: Ksa (UI/visuals)
// Purpose: Asset that holds per-method, per-doneness sprite variants for a given SoItem.
// Notes: Keeping this separate from SoItem reduces merge conflicts; SoItem keeps its default sprite.
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
        public CookingMethod method;
        [Tooltip("0 = Raw, 1 = Rare, 2 = Medium, 3 = WellDone, 4 = Burnt")]
        public Sprite[] donenessSprites = new Sprite[5];
    }

    public bool TryGet(CookingMethod method, int donenessIndex, out Sprite sprite)
    {
        sprite = null;
        if (variants == null) return false;

        for (int i = 0; i < variants.Length; i++)
        {
            if (variants[i].method != method) continue;
            var spriteArr = variants[i].donenessSprites;
            if (spriteArr != null && donenessIndex >= 0 && donenessIndex < spriteArr.Length)
            {
                sprite = spriteArr[donenessIndex];
                return sprite != null;
            }
        }
        return false;
    }
}

