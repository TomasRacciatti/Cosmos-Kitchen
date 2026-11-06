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
        [Tooltip("How far ahead (from the capsule center) the wall check ray reaches (m).")]
        [SerializeField] private float wallCheckDistance = 1f;
        [Tooltip("How far ahead of the capsule center (in the move direction) we check that the wall continues. " +
                 "If it doesn't, movement in that direction is blocked (m).")]
        [SerializeField] private float edgeCheckOffset = 0.3f;
        [Tooltip("Distance (right/left and up/down) from the capsule center to each of the 4 cross-pattern probe rays. " +
                 "Their averaged wall normals decide which way the player faces (m).")]
        [SerializeField] private float crossProbeOffset = 0.3f;

        [Header("Wall Alignment")]
        [Tooltip("Extra space kept between the capsule and the wall, on top of radius + skin width (m).")]
        [SerializeField] private float wallGap = 0.01f;
        [Tooltip("How fast the player is pulled to the correct distance from the wall.")]
        [SerializeField] private float snapSharpness = 10f;
        [Tooltip("How fast the player turns to face the wall.")]
        [SerializeField] private float rotationSharpness = 10f;

        public float ClimbSpeed => climbSpeed;
        public LayerMask ClimbableLayers => climbableLayers;
        public float WallCheckDistance => wallCheckDistance;
        public float EdgeCheckOffset => edgeCheckOffset;
        public float CrossProbeOffset => crossProbeOffset;
        public float WallGap => wallGap;
        public float SnapSharpness => snapSharpness;
        public float RotationSharpness => rotationSharpness;
    }
}
