using System.Collections.Generic;
using Items.Core;
//using UnityEditorInternal;

namespace Cooking
{
    public static class ProcessingPrerequisiteChecker
    {
        public static bool CanProcess(ItemAmount item, CookingMethod methodToApply, out string failureReason)
        {
            failureReason = string.Empty;

            if (item == null || item.IsEmpty)
                return false;

            if (item.SoItem.Prerequisites == null || item.SoItem.Prerequisites.Length == 0)
                return true;

            foreach (var prereq in item.SoItem.Prerequisites)
            {
                if (prereq.methodToPerform != methodToApply)
                    continue;
                if (!HasCompletedPrerequisite(item, prereq.prerequisiteMethod))
                {
                    failureReason = $"Ingredient must be {MethodToString(prereq.prerequisiteMethod)} before {MethodToString(prereq.methodToPerform)}";
                    return false;
                }
            }
            return true;
        }

        public static bool HasCompletedPrerequisite(ItemAmount item, CookingMethod requiredMethod)
        {
            IReadOnlyList<StepRecord> history = item.ProcessHistory;

            foreach (var step in history)
            {
                if (step.method == requiredMethod)
                    return true;
            }
            
            return false;
        }

        // Pequeño helper para que el texto tenga mas sentido
        private static string MethodToString(CookingMethod method)
        {
            switch (method)
            {
                case CookingMethod.Blend:
                    return "BLENDED";
                case CookingMethod.Fry:
                    return "FRIED";
                case CookingMethod.Boil:
                    return "BOILED";
                case CookingMethod.Roast:
                    return "ROASTED";
                case CookingMethod.Chop:
                    return "CHOPPED";
                default:
                    return method.ToString();
            }
        }
    }
}
