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
        [SerializeField] private Requirement[] requirements = new Requirement[3];
        
        public Requirement[] Requirements => requirements;
    }
    
    [Serializable]
    public struct Requirement
    {
        [Header("Identity (hard requirement)")]
        public SoItem baseItem; 
        public CookingMethod requiredMethod;

        [Header("Doneness (soft requirement)")]
        public bool useDoneness;
        public Doneness targetDoneness;
        [Tooltip("Tolerancia para evitar que cuente un error (0 = No hay tolerancia)")]
        public int tolerance;
    }
}
