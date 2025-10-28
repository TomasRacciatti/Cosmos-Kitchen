using System;
using System.Collections.Generic;
using Items.Core;

namespace Cooking
{
    public static class ProcessAlignment
    {
        // Costos de los errores
        private const int MethodCost   = 2;
        private const int DonenessCost = 1;
        
        public static int ComputeCost(IReadOnlyList<ProcessStepReq> requiredSteps, 
                                      IReadOnlyList<StepRecord> actualSteps, bool enforceOrder)
        {
            int requiredStepCount = requiredSteps?.Count ?? 0;
            int actualStepCount   = actualSteps?.Count   ?? 0;
            
            if (requiredStepCount == 0) return 0;
            if (actualStepCount == 0)
            {
                int sumMissing = 0;
                for (int i = 0; i < requiredStepCount; i++)
                    sumMissing += MissingCost(requiredSteps[i]);
                return sumMissing;
            }
            
            var dp = new int[requiredStepCount + 1, actualStepCount + 1];
            
            dp[0,0] = 0;
            for (int i = 1; i <= requiredStepCount; i++)
                dp[i,0] = dp[i-1,0] + MissingCost(requiredSteps[i-1]);
            for (int j = 1; j <= actualStepCount; j++)
                dp[0,j] = dp[0,j-1] + ExtraCost(actualSteps[j-1]); 
            
            for (int i = 1; i <= requiredStepCount; i++)
            {
                for (int j = 1; j <= actualStepCount; j++)
                {
                    int substituteOrMatch = dp[i-1,j-1] + MatchCost(requiredSteps[i-1], actualSteps[j-1]);
                    int deleteRequired = dp[i-1,j] + MissingCost(requiredSteps[i-1]);
                    int insertActual = dp[i,j-1] + ExtraCost(actualSteps[j-1]);
                    
                    int best = substituteOrMatch;
                    if (deleteRequired < best) best = deleteRequired;
                    if (insertActual  < best) best = insertActual;
                    dp[i,j] = best;
                }
            }

            return dp[requiredStepCount, actualStepCount];
        }

        private static int MatchCost(in ProcessStepReq req, in StepRecord a)
        {
            if (req.method != a.method)
                return MethodCost;
            
            bool checkDoneness = req.useDoneness && CookingUtil.SupportsDoneness(req.method);
            if (!checkDoneness) return 0;
            
            var actualDoneness = CookingUtil.ToDonenessFromTurns(a.turns);
            if (actualDoneness == Doneness.Burnt)
            {
                return 3 * DonenessCost;
            }
            
            int actual = (int)actualDoneness;
            int target = (int)req.targetDoneness;
            int delta  = Math.Abs(actual - target) - req.tolerance;
            if (delta <= 0) return 0;

            return delta * DonenessCost;
        }
        
        private static int MissingCost(in ProcessStepReq req)
        {
            return MethodCost;
        }

        private static int ExtraCost(in StepRecord a)
        {
            return 1;
        }
    }
}
