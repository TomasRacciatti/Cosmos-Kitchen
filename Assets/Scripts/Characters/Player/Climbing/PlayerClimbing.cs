using System;
using Interfaces;
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
        private const float MinGrabFacingDot = 0.7f; // must face the wall within ~45° to grab it

        // Corners of the cross-pattern probe, in (right, up) units of CrossProbeOffset.
        private static readonly Vector2[] ProbeCorners =
        {
            new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(-1f, -1f), new Vector2(1f, -1f)
        };

        private CharacterController _controller;
        private PlayerInputs _input;
        private IPlayerMotor _motor;
        private Transform _cameraTransform;

        private float _wallPushTimer;   // how long we've been walking into a grabbable wall
        private Vector3 _lastWallNormal; // used for the jump-off direction
        private float _regrabAllowedTime; // no grabbing before this time (set when jumping off)

        public bool IsClimbing { get; private set; }

        public event Action ClimbStarted;
        public event Action ClimbEnded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
            _motor = GetComponent<IPlayerMotor>();
        }

        private void OnEnable()
        {
            _input.JumpPressed += OnJumpPressed;
        }

        private void OnDisable()
        {
            _input.JumpPressed -= OnJumpPressed;
        }

        private void Update()
        {
            if (!IsClimbing)
            {
                UpdateAutoGrab(Time.deltaTime);
                return;
            }

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

            // Surface got flat enough to walk on (e.g. a ramp at the top) → walk.
            // Climbing down onto the ground → walk (tutorial: ground raycast switches to WALKING).
            if (!IsClimbableAngle(hit.normal) || (input.y < -InputDeadZone && _motor.Grounded))
            {
                StopClimbing();
                return;
            }
            _lastWallNormal = hit.normal;

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

        // Walking (or jumping) into a climbable wall for a moment grabs it.
        private void UpdateAutoGrab(float dt)
        {
            bool pushingIntoWall = Time.time >= _regrabAllowedTime &&
                                   _input.move.sqrMagnitude > InputDeadZone * InputDeadZone &&
                                   HasGrabbableWallAhead(out Vector3 wallNormal) &&
                                   Vector3.Dot(GetCameraRelativeMoveDirection(), -wallNormal) >= MinGrabFacingDot;
            if (!pushingIntoWall)
            {
                _wallPushTimer = 0f;
                return;
            }

            _wallPushTimer += dt;
            if (_wallPushTimer >= settings.AutoGrabDelay) StartClimbing();
        }

        private void OnJumpPressed()
        {
            if (IsClimbing)
            {
                JumpOff();
                return;
            }

            // Tutorial: jump while falling next to a wall grabs it (instant, no push delay needed).
            if (!_motor.Grounded && Time.time >= _regrabAllowedTime && HasGrabbableWallAhead(out _)) StartClimbing();
        }

        // Same convention as walking: input is relative to the camera's yaw.
        private Vector3 GetCameraRelativeMoveDirection()
        {
            if (_cameraTransform == null) _cameraTransform = Camera.main.transform; // cached on first use
            Vector3 move = new Vector3(_input.move.x, 0f, _input.move.y).normalized;
            return Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f) * move;
        }

        private void JumpOff()
        {
            StopClimbing();
            _regrabAllowedTime = Time.time + settings.RegrabCooldown;
            Vector3 away = Vector3.ProjectOnPlane(_lastWallNormal, Vector3.up).normalized;
            Vector2 jumpOff = settings.JumpOffVelocity;
            _motor.Launch(away * jumpOff.x + Vector3.up * jumpOff.y);
        }

        // A climbable wall close in front, that we're roughly facing.
        private bool HasGrabbableWallAhead(out Vector3 wallNormal)
        {
            wallNormal = Vector3.zero;
            Vector3 origin = transform.TransformPoint(_controller.center);
            if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, settings.GrabDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            wallNormal = hit.normal;
            return IsClimbableAngle(hit.normal) && Vector3.Dot(transform.forward, -hit.normal) >= MinGrabFacingDot;
        }

        // Steeper than what the CharacterController can walk on, but not a ceiling.
        private bool IsClimbableAngle(Vector3 normal)
        {
            float angle = Vector3.Angle(normal, Vector3.up);
            return angle > _controller.slopeLimit && angle < 180f - _controller.slopeLimit;
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
            _wallPushTimer = 0f;
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
