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
        [Tooltip("0 = Rare, 1 = Medium, 2 = WellDone, 3 = Burnt")]
        public Sprite[] donenessSprites = new Sprite[4];
    }

    public bool TryGet(CookingMethod method, int donenessIndex, out Sprite sprite)
    {
        sprite = null;
        if (variants == null) return false;
        
        if (donenessIndex <= 0) return false;

        for (int i = 0; i < variants.Length; i++)
        {
            if (variants[i].method != method) continue;
            var spriteArr = variants[i].donenessSprites;
            int idx = donenessIndex - 1;
            if (spriteArr != null && idx >= 0 && idx < spriteArr.Length)
            {
                sprite = spriteArr[idx];
                return sprite != null;
            }
        }
        return false;
    }
}

