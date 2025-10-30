using System;

namespace Cooking
{
    [Serializable]
    public struct StepRecord
    {
        public CookingMethod method;
        public int turns; // Cuanto se preparo 0-3, 4: quemado
    }
}
