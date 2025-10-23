namespace Cooking
{
    public static class CookingUtil
    {
        public static bool SupportsDoneness(CookingMethod method)
        {
            switch (method)
            {
                // Estaciones que usan Timed Station
                case CookingMethod.Boil:
                case CookingMethod.Fry:
                case CookingMethod.Roast:
                case CookingMethod.Blend:
                    return true;
                
                // Estaciones que usan Direct Station
                case CookingMethod.None:
                case CookingMethod.Chop:
                default:
                    return false;
            }
        }
    }
}
