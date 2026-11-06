using UnityEngine;

namespace Characters.Player.Climbing
{
    public static class TwoBoneIKSolver
    {
        private const float Epsilon = 0.0001f;
        
        public static void Solve(Transform upper, Transform lower, Transform end, Vector3 target, Vector3 bendHint)
        {
            Vector3 a = upper.position;
            Vector3 b = lower.position;
            Vector3 c = end.position;

            float upperLength = (b - a).magnitude;
            float lowerLength = (c - b).magnitude;
            if (upperLength < Epsilon || lowerLength < Epsilon) return;

            // Distance to the target, kept just inside reach so the limb never fully locks.
            float targetDistance = Mathf.Clamp((target - a).magnitude, Epsilon, upperLength + lowerLength - Epsilon);

            Vector3 ac = c - a;
            Vector3 ab = b - a;

            // Bend plane from the animated pose; fall back to the hint when the limb is straight.
            Vector3 bendAxis = Vector3.Cross(ac, ab);
            if (bendAxis.sqrMagnitude < Epsilon) bendAxis = Vector3.Cross(ac, bendHint);

            if (bendAxis.sqrMagnitude >= Epsilon)
            {
                bendAxis.Normalize();

                // Current and wanted angles at the root (between a→c and a→b) and at the joint (between b→a and b→c).
                float rootAngle = Vector3.Angle(ac, ab);
                float jointAngle = Vector3.Angle(a - b, c - b);
                float wantedRootAngle = LawOfCosines(lowerLength, upperLength, targetDistance);
                float wantedJointAngle = LawOfCosines(targetDistance, upperLength, lowerLength);

                // Joint first, then root: keeps the a→c direction while setting its length.
                lower.rotation = Quaternion.AngleAxis(wantedJointAngle - jointAngle, bendAxis) * lower.rotation;
                upper.rotation = Quaternion.AngleAxis(wantedRootAngle - rootAngle, bendAxis) * upper.rotation;
            }

            // Swing the whole limb so the end points at the target.
            upper.rotation = Quaternion.FromToRotation(end.position - a, target - a) * upper.rotation;
        }

        // Angle (degrees) opposite side `opposite` in a triangle with sides `opposite`, `sideA`, `sideB`.
        private static float LawOfCosines(float opposite, float sideA, float sideB)
        {
            float cos = (sideA * sideA + sideB * sideB - opposite * opposite) / (2f * sideA * sideB);
            return Mathf.Acos(Mathf.Clamp(cos, -1f, 1f)) * Mathf.Rad2Deg;
        }
    }
}
