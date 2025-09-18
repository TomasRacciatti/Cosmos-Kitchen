using System.Collections.Generic;
using Items.Core;
using UnityEngine;
using Items.Core;

namespace Characters.Clients.Plates
{
    [CreateAssetMenu(menuName = "ScriptableObject/Items/Plate", fileName = "Plate")]
    public sealed class PlateSO : SoItem
    {
        [Header("Required Ingredients")]
        [SerializeField] private SoItem ingredientA;
        [SerializeField] private SoItem ingredientB;
        [SerializeField] private SoItem ingredientC;
        
        public SoItem IngredientA => ingredientA;
        public SoItem IngredientB => ingredientB;
        public SoItem IngredientC => ingredientC;
        
        public SoItem[] RequiredIngredients => new[] { ingredientA, ingredientB, ingredientC };
    }
}
