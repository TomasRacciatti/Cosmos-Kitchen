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

            var reqs = plate.Requirements;
            if (reqs == null || reqs.Length == 0 || reqs.Any(r => r.baseItem == null))
            {
                failReason = "Plate has invalid requirements.";
                return false;
            }
            
            var used = new HashSet<int>();

            for (int r = 0; r < reqs.Length; r++)
            {
                var req = reqs[r];
                int bestIdx = -1;
                int bestDistance = int.MaxValue;
                
                bool methodHasDoneness = CookingUtil.SupportsDoneness(req.requiredMethod);
                bool checkDoneness = req.useDoneness && methodHasDoneness;

                for (int i = 0; i < inputs.Count; i++)
                {
                    if (used.Contains(i)) continue;

                    var it = inputs[i];
                    if (it.IsEmpty || it.SoItem != req.baseItem) continue;

                    var prep = it.Prep;

                    if (req.requiredMethod == CookingMethod.None) // Crudo
                        if (prep.method != CookingMethod.None) continue;
                    else
                        if (prep.method != req.requiredMethod) continue;

                    // Los quemados fallan el plato
                    if (prep.Doneness == Doneness.Burnt && checkDoneness)
                    {
                        failReason = $"Burnt {req.baseItem.ItemName} cannot be used for this plate.";
                        _lastMatch.Clear();
                        return false;
                    }

                    int dist = 0;
                    if (checkDoneness)
                        dist = Math.Abs((int)prep.Doneness - (int)req.targetDoneness);
                    
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestIdx = i;
                        if (bestDistance == 0 && checkDoneness) break;
                    }
                }
                
                if (bestIdx < 0)
                {
                    string methodText = req.requiredMethod == CookingMethod.None
                        ? "raw"
                        : req.requiredMethod.ToString();
                    failReason = $"Missing required ingredient: {req.baseItem.ItemName} ({methodText}).";
                    _lastMatch.Clear();
                    return false;
                }

                used.Add(bestIdx);
                _lastMatch.Add(bestIdx);
            }

            return true;
        }

        public int EvaluateDonenessMistakes(SoPlate plate, IReadOnlyList<ItemAmount> inputs)
        {
            if (_lastMatch.Count == 0)
            {
                string _;
                if (!ValidateIdentity(plate, inputs, out _))
                    return int.MaxValue;
            }

            int mistakes = 0;
            var reqs = plate.Requirements;

            for (int r = 0; r < reqs.Length; r++)
            {
                var req = reqs[r];
                
                bool methodHasDoneness = CookingUtil.SupportsDoneness(req.requiredMethod);
                bool checkDoneness = req.useDoneness && methodHasDoneness;
                if (!checkDoneness) continue;
                
                int idx = _lastMatch[r];
                var item = inputs[idx];
                
                int actual = (int)item.Prep.Doneness;
                int target = (int)req.targetDoneness;
                int delta = Math.Abs(actual - target) - req.tolerance;
                if (delta > 0) mistakes += delta;
            }

            return mistakes;
        }

        public int ComputeOutputRating(SoPlate plate, int baseRating, int mistakes)
        {
            int final = baseRating - mistakes;
            if (final < 1) final = 1;
            if (final > 3) final = 3;
            return final;
        }
    }
}
