using System.Collections.Generic;
using System;
using System.Linq;
using Items.Core;
using UnityEngine;

namespace Cooking
{
    public class RecipeValidator : MonoBehaviour, IRecipeValidator
    {
        public IReadOnlyList<int> LastMatchedIndices => _lastMatch;
        private readonly List<int> _lastMatch = new List<int>(3);
        
        public bool ValidateIdentity(SoPlate plate, IReadOnlyList<ItemAmount> inputs, out string failReason)
        {
            _lastMatch.Clear();
            failReason = null;

            var proc = plate.IngredientsProcess;
            bool useProcess = proc != null && proc.Length == 3 && proc.Any(p => p != null && p.steps != null && p.steps.Count > 0);

            if (useProcess)
            {
                var used = new HashSet<int>();

                for (int r = 0; r < proc.Length; r++)
                {
                    var need = proc[r];
                    if (need == null || need.baseItem == null)
                    {
                        failReason = "Plate has invalid process requirements.";
                        _lastMatch.Clear();
                        return false;
                    }

                    int pick = -1;
                    for (int i = 0; i < inputs.Count; i++)
                    {
                        if (used.Contains(i)) continue;
                        var it = inputs[i];
                        if (it.IsEmpty || it.SoItem != need.baseItem) continue;
                        
                        if (it.Prep.Doneness == Doneness.Burnt)
                        {
                            failReason = $"{need.baseItem.ItemName} is burnt and cannot be used.";
                            _lastMatch.Clear();
                            return false;
                        }

                        pick = i;
                        break;
                    }

                    if (pick < 0)
                    {
                        failReason = $"Missing required ingredient: {need.baseItem.ItemName}.";
                        _lastMatch.Clear();
                        return false;
                    }

                    used.Add(pick);
                    _lastMatch.Add(pick);
                }

                return true;
            }
            
            failReason = "Plate has no process steps configured (ingredientsProcess is empty).";
            Debug.LogError(failReason);
            _lastMatch.Clear();
            return false;
        }

        public int EvaluateDonenessMistakes(SoPlate plate, IReadOnlyList<ItemAmount> inputs)
        {
            if (_lastMatch.Count == 0)
            {
                string _;
                if (!ValidateIdentity(plate, inputs, out _))
                    return int.MaxValue;
            }

            var proc = plate.IngredientsProcess;
            bool useProcess = proc != null && proc.Length == 3 && proc.Any(p => p != null && p.steps != null && p.steps.Count > 0);
            if (useProcess)
            {
                int total = 0;
                for (int r = 0; r < proc.Length; r++)
                {
                    var need = proc[r];
                    var idx = _lastMatch[r];
                    var it = inputs[idx];
                    
                    IReadOnlyList<StepRecord> actual = it.ProcessHistory;

                    total += ProcessAlignment.ComputeCost(need.steps, actual, need.enforceOrder);
                }
                return total;
            }
            
            return 0;
        }

        public int ComputeOutputRating(SoPlate plate, int mistakes)
        {
            int final;

            if (mistakes == 0)
                final = 3;
            else if (mistakes <= 1)
                final = 2;
            else if (mistakes <= 4)
                final = 1;
            else
            {
                // esto deberia devolver un plato default que sea basura
                final = 0;
            }
            return final;
        }
    }
}
