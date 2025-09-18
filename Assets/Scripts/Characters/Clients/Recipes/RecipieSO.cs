using System.Collections.Generic;
using UnityEngine;

namespace Characters.Clients.Recipes
{
    [CreateAssetMenu(menuName = "ScriptableObject/Recipe", fileName = "Recipe_")]
    public sealed class RecipeSO : ScriptableObject
    {
        [Header("Plate Identity")]
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        
        [Header("Required Ingredients")]
        public ScriptableIngredient ingredientA;
        public ScriptableIngredient ingredientB;
        public ScriptableIngredient ingredientC;
    }
}
