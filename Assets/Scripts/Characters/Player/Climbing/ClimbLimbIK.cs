using System;
using UnityEngine;

namespace Characters.Player.Climbing
{
    [RequireComponent(typeof(PlayerClimbing))]
    public class ClimbLimbIK : MonoBehaviour
    {
        [Serializable]
        private class Limb
        {
            [Tooltip("upper_arm / thigh")] public Transform upper;
            [Tooltip("forearm / shin")] public Transform lower;
            [Tooltip("hand / foot")] public Transform end;

            [NonSerialized] public float Weight;
            [NonSerialized] public Vector3 Target;

            public bool IsValid => upper != null && lower != null && end != null;
        }

        [Header("Bones")]
        [Tooltip("Prefix the rig adds to every bone name (used by the 'Auto-assign Bones' menu).")]
        [SerializeField] private string bonePrefix = "B";
        [SerializeField] private Limb leftHand;
        [SerializeField] private Limb rightHand;
        [SerializeField] private Limb leftFoot;
        [SerializeField] private Limb rightFoot;

        [Header("Placement")]
        [Tooltip("Distance kept between each hand/foot bone and the wall surface (m). Tutorial: 0.1.")]
        [SerializeField] private float surfaceOffset = 0.08f;
        [Tooltip("How far behind the animated hand/foot (away from the wall) the ray starts (m).")]
        [SerializeField] private float castBack = 0.5f;
        [Tooltip("How far past the animated hand/foot (into the wall) the ray reaches (m).")]
        [SerializeField] private float castReach = 0.5f;
        [Tooltip("How fast the IK fades in and out (weight per second, 5 = 0.2 s).")]
        [SerializeField] private float blendSpeed = 5f;

        private PlayerClimbing _climbing;
        private Limb[] _limbs;
        private float _weight; 

        private void Awake()
        {
            _climbing = GetComponent<PlayerClimbing>();
            _limbs = new[] { leftHand, rightHand, leftFoot, rightFoot };

            foreach (Limb limb in _limbs)
            {
                if (!limb.IsValid) Debug.LogWarning($"{nameof(ClimbLimbIK)}: a limb has unassigned bones and will be skipped.", this);
            }
        }
        
        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            bool active = _climbing.IsClimbing && !_climbing.IsInClimbStart;
            _weight = Mathf.MoveTowards(_weight, active ? 1f : 0f, blendSpeed * dt);
            if (_weight <= 0f) return;
            
            Vector3 intoWall = _climbing.ClimbForward;
            Vector3 awayFromWall = -intoWall;
            LayerMask climbable = _climbing.Settings.ClimbableLayers;

            for (int i = 0; i < _limbs.Length; i++)
            {
                Limb limb = _limbs[i];
                if (!limb.IsValid) continue;

                Vector3 animated = limb.end.position;
                bool onWall = Physics.Raycast(animated - intoWall * castBack, intoWall, out RaycastHit hit,
                    castBack + castReach, climbable, QueryTriggerInteraction.Ignore);
                if (onWall) limb.Target = hit.point + awayFromWall * surfaceOffset;

                // A limb past the wall's edge fades back to its animated pose instead of popping.
                limb.Weight = Mathf.MoveTowards(limb.Weight, onWall ? 1f : 0f, blendSpeed * dt);
                float weight = limb.Weight * _weight;
                if (weight <= 0f) continue;

                // Elbows and knees point away from the wall if the limb is fully straight.
                TwoBoneIKSolver.Solve(limb.upper, limb.lower, limb.end,
                    Vector3.Lerp(animated, limb.Target, weight), awayFromWall);
            }
        }

#if UNITY_EDITOR
        // Editor helper: right-click the component header → "Auto-assign Bones".
        // Finds the bones by their Blender names (e.g. "Bupper_arm.L") anywhere under this object.
        [ContextMenu("Auto-assign Bones")]
        private void AutoAssignBones()
        {
            UnityEditor.Undo.RecordObject(this, "Auto-assign climb IK bones");
            leftHand = BuildLimb("upper_arm.L", "forearm.L", "hand.L");
            rightHand = BuildLimb("upper_arm.R", "forearm.R", "hand.R");
            leftFoot = BuildLimb("thigh.L", "shin.L", "foot.L");
            rightFoot = BuildLimb("thigh.R", "shin.R", "foot.R");
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private Limb BuildLimb(string upperName, string lowerName, string endName)
        {
            return new Limb { upper = FindBone(upperName), lower = FindBone(lowerName), end = FindBone(endName) };
        }

        private Transform FindBone(string boneName)
        {
            string fullName = bonePrefix + boneName;
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child.name == fullName) return child;
            }

            Debug.LogWarning($"{nameof(ClimbLimbIK)}: bone '{fullName}' not found under {name}.", this);
            return null;
        }
#endif
    }
}
