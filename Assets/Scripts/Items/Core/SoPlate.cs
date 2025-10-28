using System;
using Stations.Serving;
using Cooking;
using UnityEngine;

namespace Items.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Items/Plate", fileName = "Plate")]
    public sealed class SoPlate : SoItem
    {
        [Header("Required Ingredients")]
        [SerializeField] private IngredientProcessRecipe[] ingredientProcess = new IngredientProcessRecipe[3];
        public IngredientProcessRecipe[] IngredientsProcess => ingredientProcess;
    }
}
