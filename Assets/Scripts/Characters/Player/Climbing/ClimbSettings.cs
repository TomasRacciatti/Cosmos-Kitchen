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
        [Tooltip("How far ahead of the capsule center (in the move direction) we check that the wall continues. If it doesn't, movement in that direction is blocked (m).")]
        [SerializeField] private float edgeCheckOffset = 0.3f;
        [Tooltip("Distance (right/left and up/down) from the capsule center to each of the 4 cross-pattern probe rays. Their averaged wall normals decide which way the player faces (m).")]
        [SerializeField] private float crossProbeOffset = 0.3f;

        [Header("Surface Angles (measured from a vertical wall)")]
        [Tooltip("How far a surface can lean back like a ramp and still be climbed (degrees). Flatter surfaces are walked on.")]
        [Range(0f, 89f)] [SerializeField] private float maxInclination = 60f;
        [Tooltip("How far a surface can lean over the player (overhang) and still be climbed (degrees). Steeper overhangs block movement.")]
        [Range(0f, 89f)] [SerializeField] private float maxDeclination = 45f;
        [Tooltip("Ramps leaning back more than this (degrees from vertical) skip the Climb Start animation " +
                 "and go straight to climbing, even when grabbed from the ground.")]
        [Range(0f, 89f)] [SerializeField] private float climbStartMaxInclination = 30f;

        [Header("Stamina")]
        [Min(1f)] [SerializeField] private float maxStamina = 100f;
        [Tooltip("Stamina regained per second while not climbing.")]
        [Min(0f)] [SerializeField] private float staminaRegenRate = 25f;
        [Tooltip("Time after letting go before stamina starts regenerating (s).")]
        [Min(0f)] [SerializeField] private float staminaRegenDelay = 0.5f;
        [Tooltip("Walls can only be grabbed with at least this fraction of max stamina (0.15 = 15%).")]
        [Range(0f, 1f)] [SerializeField] private float minStaminaToGrab = 0.15f;
        [Tooltip("Stamina spent per second while climbing (moving) on a vertical wall.")]
        [Min(0f)] [SerializeField] private float climbMoveDrain = 15f;
        [Tooltip("Stamina spent per second while hanging still on a vertical wall.")]
        [Min(0f)] [SerializeField] private float climbIdleDrain = 3f;
        [Tooltip("Drain multiplier on a ramp at Max Inclination (blends from 1 on a vertical wall). Below 1 = cheaper.")]
        [Min(0f)] [SerializeField] private float inclineDrainMultiplier = 0.5f;
        [Tooltip("Drain multiplier on an overhang at Max Declination (blends from 1 on a vertical wall). Above 1 = costlier.")]
        [Min(0f)] [SerializeField] private float declineDrainMultiplier = 2f;

        [Header("Sliding (out of stamina on a ramp)")]
        [Tooltip("Running out of stamina on a ramp leaning back more than this (degrees from vertical) slides " +
                 "the player down it. On steeper walls and overhangs they jump off instead.")]
        [Range(0f, 89f)] [SerializeField] private float slideMinInclination = 10f;
        [Tooltip("Top speed while being dragged down the ramp (m/s).")]
        [Min(0f)] [SerializeField] private float slideSpeed = 3f;
        [Tooltip("How fast the slide reaches top speed (m/s²).")]
        [Min(0f)] [SerializeField] private float slideAcceleration = 6f;
        [Tooltip("Sideways speed while sliding (m/s). Up/down input is ignored.")]
        [Min(0f)] [SerializeField] private float slideSideSpeed = 1f;

        [Header("Wall Alignment")]
        [Tooltip("Extra space kept between the capsule and the wall, on top of radius + skin width (m).")]
        [SerializeField] private float wallGap = 0.01f;
        [Tooltip("How fast the player is pulled to the correct distance from the wall.")]
        [SerializeField] private float snapSharpness = 10f;
        [Tooltip("How fast the player turns to face the wall.")]
        [SerializeField] private float rotationSharpness = 10f;

        [Header("Transitions")]
        [Tooltip("How far ahead of the capsule center a wall can be to grab it, when walking or jumping (m).")]
        [SerializeField] private float grabDistance = 0.45f;
        [Tooltip("How long the player must keep walking into a wall before it's grabbed automatically (s).")]
        [SerializeField] private float autoGrabDelay = 0.1f;
        [Tooltip("Velocity when jumping off a wall: x = away from the wall, y = up (m/s). Tutorial: (2, 5).")]
        [SerializeField] private Vector2 jumpOffVelocity = new Vector2(2f, 5f);
        [Tooltip("After jumping off a wall, how long before a wall can be grabbed again (s).")]
        [SerializeField] private float regrabCooldown = 0.3f;

        public float ClimbSpeed => climbSpeed;
        public float GrabDistance => grabDistance;
        public float AutoGrabDelay => autoGrabDelay;
        public Vector2 JumpOffVelocity => jumpOffVelocity;
        public float RegrabCooldown => regrabCooldown;
        public LayerMask ClimbableLayers => climbableLayers;
        public float WallCheckDistance => wallCheckDistance;
        public float EdgeCheckOffset => edgeCheckOffset;
        public float CrossProbeOffset => crossProbeOffset;
        public float MaxInclination => maxInclination;
        public float MaxDeclination => maxDeclination;
        public float ClimbStartMaxInclination => climbStartMaxInclination;
        public float MaxStamina => maxStamina;
        public float StaminaRegenRate => staminaRegenRate;
        public float StaminaRegenDelay => staminaRegenDelay;
        public float MinStaminaToGrab => minStaminaToGrab;
        public float ClimbMoveDrain => climbMoveDrain;
        public float ClimbIdleDrain => climbIdleDrain;
        public float InclineDrainMultiplier => inclineDrainMultiplier;
        public float DeclineDrainMultiplier => declineDrainMultiplier;
        public float SlideMinInclination => slideMinInclination;
        public float SlideSpeed => slideSpeed;
        public float SlideAcceleration => slideAcceleration;
        public float SlideSideSpeed => slideSideSpeed;
        public float WallGap => wallGap;
        public float SnapSharpness => snapSharpness;
        public float RotationSharpness => rotationSharpness;
    }
}
