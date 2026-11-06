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
        private const float ConvexTolerance = 0.05f;
        private const float SameSurfaceDot = 0.99f; 
        private const float MinGrabFacingDot = 0.7f;
        private const float ClimbStartEnterTimeout = 0.25f;

        // Corners of the cross-pattern probe, in (right, up) units of CrossProbeOffset.
        private static readonly Vector2[] ProbeCorners =
        {
            new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(-1f, -1f), new Vector2(1f, -1f)
        };

        private CharacterController _controller;
        private PlayerInputs _input;
        private IPlayerMotor _motor;
        private IClimbView _view; 
        private Transform _cameraTransform;

        private float _wallPushTimer;
        private Vector3 _lastWallNormal;
        private float _regrabAllowedTime;
        private bool _climbStartPending;
        private bool _climbStartSeen;
        private float _climbStartTime;

        public bool IsClimbing { get; private set; }

        public event Action ClimbStarted;
        public event Action ClimbEnded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
            _motor = GetComponent<IPlayerMotor>();
            _view = GetComponent<IClimbView>();
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

            if (_climbStartPending) UpdateClimbStart();

            Vector2 input = _climbStartPending ? Vector2.zero : SquareToCircle(_input.move);
            float dt = Time.deltaTime;
            
            Vector3 origin = transform.TransformPoint(_controller.center);
            
            if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                StopClimbing();
                return;
            }
            
            // Everywhere else the straight-ahead hit is used as-is (same as the single-ray version).
            if (TryGetInsideCornerDirection(origin, hit, out Vector3 checkDirection) &&
                Physics.Raycast(origin, checkDirection, out RaycastHit averagedHit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                hit = averagedHit;
                DrawDebugRay(origin, checkDirection, Color.magenta);
            }
            DrawDebugRay(origin, transform.forward * hit.distance, Color.green);
            
            if (!IsClimbableAngle(hit.normal) || (input.y < -InputDeadZone && _motor.Grounded))
            {
                StopClimbing();
                return;
            }
            _lastWallNormal = hit.normal;
            
            Quaternion wallRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            
            Vector3 flatForward = Vector3.ProjectOnPlane(-hit.normal, Vector3.up);
            if (flatForward.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatForward),
                    settings.RotationSharpness * dt);
            }
            
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

            // Animate with the input that was actually applied (blocked edges = idle).
            if (_view != null) _view.SetClimbMove(input);
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
        
        // Releases climb input once the Climb Start animation is over (its exit-time transition has begun).
        private void UpdateClimbStart()
        {
            if (_view.IsPlayingClimbStart)
            {
                _climbStartSeen = true;
                return;
            }

            // The Animator enters the state one frame after the grab. If it never does
            // (e.g. state renamed), don't leave the player stuck.
            if (_climbStartSeen || Time.time - _climbStartTime > ClimbStartEnterTimeout)
            {
                _climbStartPending = false;
            }
        }

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
            if (_wallPushTimer >= settings.AutoGrabDelay) StartClimbing(_motor.Grounded);
        }

        private void OnJumpPressed()
        {
            if (IsClimbing)
            {
                JumpOff();
                return;
            }
            
            if (!_motor.Grounded && Time.time >= _regrabAllowedTime && HasGrabbableWallAhead(out _)) StartClimbing();
        }
        
        private Vector3 GetCameraRelativeMoveDirection()
        {
            if (_cameraTransform == null) _cameraTransform = Camera.main.transform;
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
        
        private bool IsClimbableAngle(Vector3 normal)
        {
            float angle = Vector3.Angle(normal, Vector3.up);
            return angle > _controller.slopeLimit && angle < 180f - _controller.slopeLimit;
        }
        
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

        /// <param name="fromGround">Grabbing from standing: plays Climb Start and holds input until it finishes.</param>
        public void StartClimbing(bool fromGround = false)
        {
            if (IsClimbing) return;
            IsClimbing = true;
            _wallPushTimer = 0f;
            _climbStartPending = fromGround && _view != null;
            _climbStartSeen = false;
            _climbStartTime = Time.time;
            ClimbStarted?.Invoke();
            if (_view != null) _view.ClimbStarted(fromGround);
        }

        public void StopClimbing()
        {
            if (!IsClimbing) return;
            IsClimbing = false;
            _climbStartPending = false;
            ClimbEnded?.Invoke();
            if (_view != null) _view.ClimbEnded();
        }
        
        private static Vector2 SquareToCircle(Vector2 input)
        {
            return input.sqrMagnitude >= 1f ? input.normalized : input;
        }
    }
}
