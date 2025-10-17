using System;

namespace Cooking
{
    public enum CookingMethod
    {
        None  = 0,
        Boil  = 1, // Olla
        Fry   = 2, // Sarten
        Roast = 3, // Horno
        Blend = 4,
        Chop  = 5, 
    }
    
    public enum Doneness
    {
        Raw      = 0,
        Rare     = 1,
        Medium   = 2,
        WellDone = 3,
        Burnt    = 4
    }

    [Serializable]
    public struct PreparationState
    {
        public CookingMethod method;

        public float turnsCooked;
        
        public Doneness Doneness
        {
            get
            {
                int idx = (int)Math.Floor(turnsCooked);
                if (idx < 0) idx = 0;
                if (idx > 4) idx = 4;
                return (Doneness)idx;
            }
        }
    }
}
