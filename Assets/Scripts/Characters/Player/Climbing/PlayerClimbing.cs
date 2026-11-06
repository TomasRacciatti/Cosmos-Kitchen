using System;
using UnityEngine;

namespace Characters.Player.Climbing
{
    /// <summary>
    /// Climbing movement. Owns the CharacterController while climbing;
    /// PlayerController steps aside whenever <see cref="IsClimbing"/> is true.
    /// Based on the Madalaski BOTW climbing tutorial, adapted to CharacterController + Input System.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputs))]
    public class PlayerClimbing : MonoBehaviour
    {
        [SerializeField] private ClimbSettings settings;

        private const float InputDeadZone = 0.01f;

        private CharacterController _controller;
        private PlayerInputs _input;

        public bool IsClimbing { get; private set; }

        public event Action ClimbStarted;
        public event Action ClimbEnded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
        }

        private void Update()
        {
            if (!IsClimbing) return;

            Vector2 input = SquareToCircle(_input.move);
            float dt = Time.deltaTime;

            // Cast from the capsule center (the root pivot is at the feet).
            Vector3 origin = transform.TransformPoint(_controller.center);
            if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                StopClimbing();
                return;
            }

            // Wall-aligned axes: forward into the wall, up along its surface.
            Quaternion wallRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);

            // Face the wall. Only yaw on the root: the CharacterController capsule can't tilt.
            Vector3 flatForward = Vector3.ProjectOnPlane(-hit.normal, Vector3.up);
            if (flatForward.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatForward),
                    settings.RotationSharpness * dt);
            }

            // Pull the capsule to a fixed distance from the wall (tutorial: hit.point + hit.normal * offset).
            float wallDistance = _controller.radius + _controller.skinWidth + settings.WallGap;
            Vector3 targetOrigin = hit.point + hit.normal * wallDistance;
            Vector3 snap = Vector3.Lerp(origin, targetOrigin, settings.SnapSharpness * dt) - origin;

            // Block movement toward an edge (>90° corner) 
            Vector3 wallForward = -hit.normal;
            if (Mathf.Abs(input.x) > InputDeadZone &&
                !HasWallAhead(origin, wallRotation * Vector3.right * Mathf.Sign(input.x), wallForward))
            {
                input.x = 0f;
            }
            if (Mathf.Abs(input.y) > InputDeadZone &&
                !HasWallAhead(origin, wallRotation * Vector3.up * Mathf.Sign(input.y), wallForward))
            {
                input.y = 0f;
            }

            Vector3 velocity = wallRotation * new Vector3(input.x, input.y, 0f) * settings.ClimbSpeed;
            _controller.Move(snap + velocity * dt);
        }

        // True if the wall still continues a little ahead in the move direction.
        private bool HasWallAhead(Vector3 origin, Vector3 moveDirection, Vector3 wallForward)
        {
            Vector3 probeOrigin = origin + moveDirection * settings.EdgeCheckOffset;
            return Physics.Raycast(probeOrigin, wallForward, settings.WallCheckDistance,
                settings.ClimbableLayers, QueryTriggerInteraction.Ignore);
        }

        public void StartClimbing()
        {
            if (IsClimbing) return;
            IsClimbing = true;
            ClimbStarted?.Invoke();
        }

        public void StopClimbing()
        {
            if (!IsClimbing) return;
            IsClimbing = false;
            ClimbEnded?.Invoke();
        }

        // Keeps diagonal input from being faster than straight input.
        private static Vector2 SquareToCircle(Vector2 input)
        {
            return input.sqrMagnitude >= 1f ? input.normalized : input;
        }
    }
}
