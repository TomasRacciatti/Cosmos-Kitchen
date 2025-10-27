using UnityEngine;
using System;
using System.Collections.Generic;
using Items.Core;

namespace Cooking
{
    [Serializable]
    public class IngredientProcessRecipe
    {
        [Header("Identity (Hard Req)")] 
        public SoItem baseItem;
        
        [Header("Steps (Soft Req)")] 
        public List<ProcessStepReq> steps = new();
        public bool enforceOrder = true;
    }

    [Serializable]
    public class ProcessStepReq
    {
        [Header("Target Step")] 
        public CookingMethod method;

        [Header("Doneness")] 
        public bool useDoneness = false;
        public Doneness targetDoneness = Doneness.Medium;
        public int tolerance = 0;

        [Header("Mistake cost")]
        public int methodCost = 2;
        public int donenessCost = 1;
    }
}
