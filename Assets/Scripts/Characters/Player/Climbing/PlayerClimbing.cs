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
        [Tooltip("Draw the climbing rays in the Scene view (green = straight-ahead, magenta = inside-corner ray, cyan = probe used, yellow = probe ignored (outside corner), red = probe miss).")]
        [SerializeField] private bool drawDebugRays;

        private const float InputDeadZone = 0.01f;
        private const float ConvexTolerance = 0.05f; // small bumps (< 5 cm) still count as the same surface
        private const float SameSurfaceDot = 0.99f;  // normals within ~8° count as the same surface

        // Corners of the cross-pattern probe, in (right, up) units of CrossProbeOffset.
        private static readonly Vector2[] ProbeCorners =
        {
            new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(-1f, -1f), new Vector2(1f, -1f)
        };

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

            // Straight-ahead ray is the anchor (this is what carries us around outside corners).
            if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                StopClimbing();
                return;
            }

            // Only at an inside corner: re-check along the averaged cross-pattern direction to turn into it.
            // Everywhere else the straight-ahead hit is used as-is (same as the single-ray version).
            if (TryGetInsideCornerDirection(origin, hit, out Vector3 checkDirection) &&
                Physics.Raycast(origin, checkDirection, out RaycastHit averagedHit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                hit = averagedHit;
                DrawDebugRay(origin, checkDirection, Color.magenta);
            }
            DrawDebugRay(origin, transform.forward * hit.distance, Color.green);

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

        // 4 rays from the corners around the capsule center, cast forward.
        // Returns true if any probe finds a surface angled toward us (inside corner), with the averaged
        // direction into the wall. Surfaces bending away (outside corners) are ignored.
        private bool TryGetInsideCornerDirection(Vector3 origin, RaycastHit frontHit, out Vector3 direction)
        {
            Vector3 right = transform.right * settings.CrossProbeOffset;
            Vector3 up = transform.up * settings.CrossProbeOffset;
            Vector3 forward = transform.forward;

            bool foundInsideCorner = false;
            Vector3 normalSum = frontHit.normal;
            for (int i = 0; i < ProbeCorners.Length; i++)
            {
                Vector3 probeOrigin = origin + right * ProbeCorners[i].x + up * ProbeCorners[i].y;
                if (!Physics.Raycast(probeOrigin, forward, out RaycastHit probeHit, settings.WallCheckDistance,
                        settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
                {
                    DrawDebugRay(probeOrigin, forward * settings.WallCheckDistance, Color.red);
                    continue;
                }

                // Height of the probe hit above the front surface's plane:
                // behind it = the wall bends away (outside corner), in front of it = it bends toward us (inside corner).
                float depth = Vector3.Dot(frontHit.normal, probeHit.point - frontHit.point);
                if (depth < -ConvexTolerance)
                {
                    DrawDebugRay(probeOrigin, forward * probeHit.distance, Color.yellow);
                    continue;
                }

                normalSum += probeHit.normal;
                if (depth > ConvexTolerance && Vector3.Dot(probeHit.normal, frontHit.normal) < SameSurfaceDot)
                {
                    foundInsideCorner = true;
                }
                DrawDebugRay(probeOrigin, forward * probeHit.distance, Color.cyan);
            }

            direction = normalSum.sqrMagnitude > 0.0001f ? -normalSum.normalized : forward;
            return foundInsideCorner;
        }

        // True if the wall still continues a little ahead in the move direction.
        private bool HasWallAhead(Vector3 origin, Vector3 moveDirection, Vector3 wallForward)
        {
            Vector3 probeOrigin = origin + moveDirection * settings.EdgeCheckOffset;
            return Physics.Raycast(probeOrigin, wallForward, settings.WallCheckDistance,
                settings.ClimbableLayers, QueryTriggerInteraction.Ignore);
        }

        private void DrawDebugRay(Vector3 start, Vector3 direction, Color color)
        {
            if (drawDebugRays) Debug.DrawRay(start, direction, color);
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
