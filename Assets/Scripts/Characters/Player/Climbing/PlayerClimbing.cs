using System;
using Interfaces;
using UnityEngine;

namespace Characters.Player.Climbing
{
    /// <summary>
    /// Climbing movement on walls, ramps (inclination) and overhangs (declination).
    /// Owns the CharacterController while climbing;
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
        private const float GroundProbeMargin = 0.1f;  // extra reach below the feet when looking for ground
        private const float StandUpProbeExtra = 0.5f;  // how far below the feet we look for ground when standing up

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
        private Vector3 _lastWallPoint;
        private bool _hasWallPoint;
        private float _regrabAllowedTime;
        private bool _climbStartPending;
        private bool _climbStartSeen;
        private float _climbStartTime;
        
        private Quaternion _climbRotation = Quaternion.identity;
        private float _standingHeight;
        
        private Vector3 _contactNormal;
        private int _contactFrame = -1;

        public bool IsClimbing { get; private set; }
        
        public bool IsInClimbStart => _climbStartPending;
        
        public Vector3 WallNormal => _lastWallNormal;
        
        public Quaternion ClimbRotation => _climbRotation;
        public Vector3 ClimbForward => _climbRotation * Vector3.forward;
        
        public float SurfaceTilt { get; private set; }

        public ClimbSettings Settings => settings;

        public event Action ClimbStarted;
        public event Action ClimbEnded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
            _motor = GetComponent<IPlayerMotor>();
            _view = GetComponent<IClimbView>();
            _standingHeight = _controller.height;
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
            Vector3 forward = ClimbForward;

            if (!Physics.Raycast(origin, forward, out RaycastHit hit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                StopClimbing();
                return;
            }

            // Everywhere else the straight-ahead hit is used as-is (same as the single-ray version).
            if (TryGetInsideCornerDirection(origin, hit, out Vector3 checkDirection) &&
                Physics.Raycast(origin, checkDirection, out RaycastHit averagedHit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore) &&
                GetSurfaceTilt(averagedHit.normal) <= settings.MaxDeclination)
            {
                hit = averagedHit;
                DrawDebugRay(origin, checkDirection, Color.magenta);
            }
            DrawDebugRay(origin, forward * hit.distance, Color.green);

            // Surface got too flat to climb (e.g. top of a ramp) → stand on it.
            if (GetSurfaceTilt(hit.normal) < -settings.MaxInclination)
            {
                StopClimbing(clearSurface: false);
                return;
            }

            // Climbing down onto walkable ground → walk (tutorial: ground raycast switches to WALKING).
            if (input.y < -InputDeadZone && IsFeetOnGround(origin))
            {
                StopClimbing();
                return;
            }
            _lastWallNormal = hit.normal;
            _lastWallPoint = hit.point;
            _hasWallPoint = true;

            // Turn the climbing frame to face the surface (tutorial: transform.forward = -hit.normal), smoothed.
            Quaternion surfaceRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            _climbRotation = Quaternion.Slerp(_climbRotation, surfaceRotation, settings.RotationSharpness * dt);
            forward = ClimbForward;
            SurfaceTilt = GetSurfaceTilt(-forward);
            ApplyRootYaw(dt);

            float wallDistance = _controller.radius + _controller.skinWidth + settings.WallGap;
            Vector3 targetOrigin = hit.point + hit.normal * wallDistance;
            Vector3 snap = Vector3.Lerp(origin, targetOrigin, settings.SnapSharpness * dt) - origin;

            // Block movement toward an edge (>90° corner) or onto an overhang steeper than allowed.
            Vector3 right = _climbRotation * Vector3.right;
            Vector3 up = _climbRotation * Vector3.up;
            if (Mathf.Abs(input.x) > InputDeadZone && !CanMoveToward(origin, right * Mathf.Sign(input.x), forward))
            {
                input.x = 0f;
            }
            if (Mathf.Abs(input.y) > InputDeadZone && !CanMoveToward(origin, up * Mathf.Sign(input.y), forward))
            {
                input.y = 0f;
            }

            Vector3 velocity = (right * input.x + up * input.y) * settings.ClimbSpeed;
            _controller.Move(snap + velocity * dt);

            // Animate with the input that was actually applied (blocked edges = idle).
            if (_view != null) _view.SetClimbMove(input);
        }

        // Remember climbable surfaces we bump into (walls, but also ramps under our feet).
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (IsClimbing || (settings.ClimbableLayers.value & (1 << hit.gameObject.layer)) == 0) return;
            if (!IsClimbableTilt(GetSurfaceTilt(hit.normal))) return; // e.g. the flat top of a climbable block
            _contactNormal = hit.normal;
            _contactFrame = Time.frameCount;
        }

        // 4 rays from the corners around the capsule center, cast forward.
        // Returns true if any probe finds a surface angled toward us (inside corner), with the averaged
        // direction into the wall. Surfaces bending away (outside corners) are ignored.
        private bool TryGetInsideCornerDirection(Vector3 origin, RaycastHit frontHit, out Vector3 direction)
        {
            Vector3 right = _climbRotation * Vector3.right * settings.CrossProbeOffset;
            Vector3 up = _climbRotation * Vector3.up * settings.CrossProbeOffset;
            Vector3 forward = ClimbForward;

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
            Vector3 surfaceNormal = Vector3.zero;
            bool pushingIntoWall = Time.time >= _regrabAllowedTime &&
                                   _input.move.sqrMagnitude > InputDeadZone * InputDeadZone &&
                                   TryGetGrabbableSurface(out surfaceNormal) &&
                                   Vector3.Dot(GetCameraRelativeMoveDirection(), FlatInto(surfaceNormal)) >= MinGrabFacingDot;
            if (!pushingIntoWall)
            {
                _wallPushTimer = 0f;
                return;
            }

            _wallPushTimer += dt;
            if (_wallPushTimer < settings.AutoGrabDelay) return;

            // Climb Start only when grabbing from standing, and not on shallow ramps (you just step onto those).
            bool playClimbStart = _motor.Grounded &&
                                  GetSurfaceTilt(surfaceNormal) >= -settings.ClimbStartMaxInclination;
            StartClimbing(surfaceNormal, playClimbStart);
        }

        private void OnJumpPressed()
        {
            if (IsClimbing)
            {
                JumpOff();
                return;
            }

            if (!_motor.Grounded && Time.time >= _regrabAllowedTime && TryGetGrabbableSurface(out Vector3 normal))
            {
                StartClimbing(normal, false);
            }
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
            Vector3 away = -FlatInto(_lastWallNormal);
            Vector2 jumpOff = settings.JumpOffVelocity;
            _motor.Launch(away * jumpOff.x + Vector3.up * jumpOff.y);
        }

        // A climbable surface we're roughly facing: a wall close in front of the chest,
        // or a surface the CharacterController just touched (e.g. a ramp under our feet).
        private bool TryGetGrabbableSurface(out Vector3 surfaceNormal)
        {
            Vector3 origin = transform.TransformPoint(_controller.center);
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, settings.GrabDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore) &&
                IsGrabbable(hit.normal))
            {
                surfaceNormal = hit.normal;
                return true;
            }

            // Touched this frame or the previous one (PlayerController may move before or after us).
            surfaceNormal = _contactNormal;
            return Time.frameCount - _contactFrame <= 1 && IsGrabbable(_contactNormal);
        }

        private bool IsGrabbable(Vector3 normal)
        {
            return IsClimbableTilt(GetSurfaceTilt(normal)) &&
                   Vector3.Dot(transform.forward, FlatInto(normal)) >= MinGrabFacingDot;
        }

        private bool IsClimbableTilt(float tilt)
        {
            return tilt >= -settings.MaxInclination && tilt <= settings.MaxDeclination;
        }

        // Degrees from a vertical wall: negative = ramp-like (inclination), positive = overhang (declination).
        private static float GetSurfaceTilt(Vector3 normal)
        {
            return Vector3.Angle(normal, Vector3.up) - 90f;
        }

        // Horizontal direction into the surface.
        private static Vector3 FlatInto(Vector3 normal)
        {
            return Vector3.ProjectOnPlane(-normal, Vector3.up).normalized;
        }

        // True if the surface continues a little ahead in the move direction and isn't an overhang beyond the max.
        private bool CanMoveToward(Vector3 origin, Vector3 moveDirection, Vector3 intoSurface)
        {
            Vector3 probeOrigin = origin + moveDirection * settings.EdgeCheckOffset;
            return Physics.Raycast(probeOrigin, intoSurface, out RaycastHit hit, settings.WallCheckDistance,
                       settings.ClimbableLayers, QueryTriggerInteraction.Ignore) &&
                   GetSurfaceTilt(hit.normal) <= settings.MaxDeclination;
        }

        // Walkable ground right under where our feet would be standing.
        private bool IsFeetOnGround(Vector3 origin)
        {
            float reach = _controller.center.y + GroundProbeMargin;
            return Physics.Raycast(origin, Vector3.down, out RaycastHit hit, reach, _motor.GroundLayers,
                       QueryTriggerInteraction.Ignore) &&
                   GetSurfaceTilt(hit.normal) < -settings.MaxInclination;
        }

        // The root follows the climbing frame's heading only (smoothed, like before).
        private void ApplyRootYaw(float dt)
        {
            Vector3 flatForward = Vector3.ProjectOnPlane(ClimbForward, Vector3.up);
            if (flatForward.sqrMagnitude < 0.0001f) return;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatForward),
                settings.RotationSharpness * dt);
        }

        private void DrawDebugRay(Vector3 start, Vector3 direction, Color color)
        {
            if (drawDebugRays) Debug.DrawRay(start, direction, color);
        }

        /// <param name="surfaceNormal">Normal of the surface being grabbed (sets the initial climbing frame).</param>
        /// <param name="fromGround">Grabbing from standing: plays Climb Start and holds input until it finishes.</param>
        public void StartClimbing(Vector3 surfaceNormal, bool fromGround)
        {
            if (IsClimbing) return;
            IsClimbing = true;
            _wallPushTimer = 0f;
            _climbStartPending = fromGround && _view != null;
            _climbStartSeen = false;
            _climbStartTime = Time.time;

            // Face the surface right away so the first wall ray finds it, even on shallow ramps.
            // (The model's lean is smoothed separately.)
            _climbRotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);
            _lastWallNormal = surfaceNormal;
            _hasWallPoint = false;
            SurfaceTilt = GetSurfaceTilt(surfaceNormal);

            // Shrink the capsule to a sphere around its center: an upright capsule can't lie against
            // a ramp or tuck under an overhang.
            _controller.height = _controller.radius * 2f;

            ClimbStarted?.Invoke();
            if (_view != null) _view.ClimbStarted(fromGround);
        }

        public void StopClimbing() => StopClimbing(true);

        private void StopClimbing(bool clearSurface)
        {
            if (!IsClimbing) return;
            IsClimbing = false;
            _climbStartPending = false;
            SurfaceTilt = 0f;
            RestoreStandingCapsule(clearSurface);
            ClimbEnded?.Invoke();
            if (_view != null) _view.ClimbEnded();
        }

        // Back to the full capsule, placed where it fits. Otherwise the CharacterController shoves itself
        // out of the geometry in one frame, and PlayerController reads that shove as walking speed.
        /// <param name="clearSurface">False when stepping onto the top of a ramp: the ramp's plane doesn't
        /// continue past its edge, so pushing out of it would only shove the player backward.</param>
        private void RestoreStandingCapsule(bool clearSurface)
        {
            _controller.height = _standingHeight;

            Vector3 correction = clearSurface ? GetSurfaceClearance() : Vector3.zero;
            correction += Vector3.up * GetGroundLift(correction);
            if (correction.sqrMagnitude <= 0f) return;

            // CharacterController must be disabled to teleport it.
            _controller.enabled = false;
            transform.position += correction;
            _controller.enabled = true;
        }

        // On anything but a vertical wall, the top or bottom of the full capsule would stick into the
        // climbed surface (it leans toward one end). Push out along the surface normal until both ends clear it.
        private Vector3 GetSurfaceClearance()
        {
            if (!_hasWallPoint) return Vector3.zero;

            Vector3 center = transform.TransformPoint(_controller.center);
            float halfSegment = Mathf.Max(0f, _controller.height * 0.5f - _controller.radius);
            float clearance = _controller.radius + _controller.skinWidth;

            float topDepth = clearance - Vector3.Dot(_lastWallNormal, center + Vector3.up * halfSegment - _lastWallPoint);
            float bottomDepth = clearance - Vector3.Dot(_lastWallNormal, center - Vector3.up * halfSegment - _lastWallPoint);
            float push = Mathf.Max(0f, Mathf.Max(topDepth, bottomDepth));
            return _lastWallNormal * push;
        }

        // On ramps the feet can end up below the surface we stand up on: lift onto it.
        private float GetGroundLift(Vector3 offset)
        {
            Vector3 center = transform.TransformPoint(_controller.center) + offset;
            float reach = _controller.center.y + StandUpProbeExtra;
            if (!Physics.Raycast(center, Vector3.down, out RaycastHit hit, reach, _motor.GroundLayers,
                    QueryTriggerInteraction.Ignore))
            {
                return 0f;
            }

            float feetY = transform.position.y + offset.y;
            return Mathf.Max(0f, hit.point.y + _controller.skinWidth - feetY);
        }

        private static Vector2 SquareToCircle(Vector2 input)
        {
            return input.sqrMagnitude >= 1f ? input.normalized : input;
        }
    }
}
