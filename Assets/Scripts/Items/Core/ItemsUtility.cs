using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items.Core
{
    public static class ItemsUtility
    {
        public static bool Stackable(params ItemAmount[] itemAmounts)
        {
            /* viejo sistema de stack
            return itemAmounts.Length > 0
                   && itemAmounts.All(i => !i.IsEmpty && i.SoItem == itemAmounts[0].SoItem);
            */
            
            // Guardrails
            if (itemAmounts == null || itemAmounts.Length == 0) return false;

            var first = itemAmounts[0];
            if (first.IsEmpty) return false;
            
            if (itemAmounts.Any(i => i.IsEmpty || i.SoItem != first.SoItem))
                return false;

            // Vamos por los items y revisamos los Preps
            
            // Chequeo con los direct processes
            var firstHistory = first.ProcessHistory; // El primero es nuestra ancla para ver si el resto apilan con el
            int firstHistoryCount = (firstHistory != null) ? firstHistory.Count : 0;
            
            foreach (var it in itemAmounts)
            {
                var restHistory = it.ProcessHistory;
                int restHistoryCount = (restHistory != null) ? restHistory.Count : 0;
                
                if (firstHistoryCount != restHistoryCount) return false;

                if (firstHistoryCount > 0)
                {
                    for (int i = 0; i < firstHistoryCount; i++)
                    {
                        if (firstHistory[i].method != restHistory[i].method) return false;
                        if (firstHistory[i].turns != restHistory[i].turns) return false;
                    }
                }
            }
            
            // Chequeo con timed processes
            var prepStateFirst = first.Prep; // El primero es nuestra ancla para ver si el resto apilan con el
            foreach (var it in itemAmounts)
            {
                var prepStateRest = it.Prep;
                
                bool firstHasPrep = prepStateFirst.method != Cooking.CookingMethod.None;
                bool restHavePrep = prepStateRest.method != Cooking.CookingMethod.None;

                if (firstHasPrep != restHavePrep) return false;
                if (!firstHasPrep) continue;

                if (prepStateFirst.method   != prepStateRest.method)   return false;
                if (prepStateFirst.Doneness != prepStateRest.Doneness) return false;
            }

            return true;
        }
    }
}

