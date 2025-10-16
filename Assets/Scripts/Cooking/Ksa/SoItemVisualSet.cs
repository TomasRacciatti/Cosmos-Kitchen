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
    public SoItem item;
    
    [Header("Variants by Method (index 0..4 = Raw..Burnt)")]
    public List<MethodVariants> variants = new List<MethodVariants>();
}

[Serializable]
public class MethodVariants
{
    public CookingMethod method;
    [Tooltip("0 = Raw, 1 = Rare, 2 = Medium, 3 = WellDone, 4 = Burnt")]
    public Sprite[] donenessSprites = new Sprite[5];
}
