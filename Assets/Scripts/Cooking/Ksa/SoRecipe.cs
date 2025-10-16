using UnityEngine;
using System;
using Cooking;
using Items.Core;

namespace Stations.Serving
{
    // Owner: Ksa (serving/recipes)
    // Purpose: Optional data asset referenced by SoPlate to express per-ingredient method/doneness requirements.
    // Notes: If you don’t want a new asset, you can instead add Requirement[] directly to SoPlate with the same shape.
    [CreateAssetMenu(menuName = "ScriptableObject/Cooking/Recipe", fileName = "SoRecipe")]
    public class SoRecipe : ScriptableObject
    {
        public Requirement[] ingredients = Array.Empty<Requirement>();
    }

    [Serializable]
    public class Requirement
    {
        [Header("Identity (hard requirement)")]
        public SoItem baseItem;
        public CookingMethod requiredMethod = CookingMethod.None;
        
        [Header("Doneness (soft requirement)")]
        public Doneness? targetDoneness = null; // null = no requiere doneness (no creo que lo usemos siendo nulo)
        [Tooltip("Allowed +- turns away from target (0 = exact)")]
        public int tolerance = 0; // Si en el futuro quisieramos que hayan recetas faciles o gane un
                                  // perk que le permita tener errores, podemos tocar esto
    }
}