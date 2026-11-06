using UnityEngine;

namespace Characters.Player.Climbing
{
    [CreateAssetMenu(fileName = "ClimbSettings", menuName = "ScriptableObject/Player/Climb Settings")]
    public class ClimbSettings : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Speed while moving along a wall (m/s).")]
        [SerializeField] private float climbSpeed = 2f;

        [Header("Detection")]
        [Tooltip("Only colliders on these layers can be climbed.")]
        [SerializeField] private LayerMask climbableLayers;

        public float ClimbSpeed => climbSpeed;
        public LayerMask ClimbableLayers => climbableLayers;
    }
}
