using UnityEngine;

namespace Items.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Items/Plate", fileName = "Plate")]
    public sealed class SoPlate : SoItem
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
