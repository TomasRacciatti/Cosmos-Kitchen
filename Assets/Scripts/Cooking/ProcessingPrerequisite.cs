using System;
using UnityEngine;

namespace Cooking
{
    [Serializable]
    public class ProcessingPrerequisite
    {
        [Tooltip("Cooking method that requires prerequisite")]
        public CookingMethod methodToPerform;
    
        [Tooltip("Method that must be completed before this method can be executed")]
        public CookingMethod prerequisiteMethod;
    }
}
