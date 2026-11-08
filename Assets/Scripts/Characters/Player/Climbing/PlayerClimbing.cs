using System;
using Interfaces;
using UnityEngine;

namespace Characters.Player.Climbing
{
    // Based on the Madalaski BOTW climbing tutorial, adapted to CharacterController + Input System.
    // PlayerController steps aside while IsClimbing is true.
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
        private const float AnimatorStateEnterTimeout = 0.25f;
        private const float GroundProbeMargin = 0.1f;
        private const float StandUpProbeExtra = 0.5f;
        private const float LedgeProbeHeadroom = 0.5f;
        private const float LedgeRiseTolerance = 0.3f;
        private const float CapsuleFitMargin = 0.05f;

        // Waits for an Animator state we triggered to finish. The state is only entered a frame after the
        // trigger; the timeout keeps the player from getting stuck if it never is (e.g. the state was renamed).
        private struct AnimatorStateWait
        {
            private bool _seen;
            private float _startTime;

            public void Begin()
            {
                _seen = false;
                _startTime = Time.time;
            }

            public bool IsDone(bool isPlaying)
            {
                if (isPlaying)
                {
                    _seen = true;
                    return false;
                }
                return _seen || Time.time - _startTime > AnimatorStateEnterTimeout;
            }
        }

        // Cross-pattern probe corners, in (right, up) units of CrossProbeOffset.
        private static readonly Vector2[] ProbeCorners =
        {
            new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(-1f, -1f), new Vector2(1f, -1f)
        };

        private CharacterController _controller;
        private PlayerInputs _input;
        private IPlayerMotor _motor;
        private IClimbView _view;
        private IClimbGate[] _gates;
        private Transform _cameraTransform;

        private float _wallPushTimer;
        private Vector3 _lastWallNormal;
        private Vector3 _lastWallPoint;
        private bool _hasWallPoint;
        private float _regrabAllowedTime;
        private bool _climbStartPending;
        private AnimatorStateWait _climbStartWait;
        private AnimatorStateWait _ledgeClimbWait;

        // Forward into the surface, pitched on ramps/overhangs. The root itself only yaws,
        // because the CharacterController capsule can't tilt.
        private Quaternion _climbRotation = Quaternion.identity;
        private float _standingHeight;
        private float _slideSpeed;

        private Vector3 _contactNormal;
        private int _contactFrame = -1;

        public bool IsClimbing { get; private set; }
        public bool IsInClimbStart => _climbStartPending;

        // Out of stamina on a ramp: still on the surface, but dragged down it.
        public bool IsSliding { get; private set; }

        // Playing the climb-up onto a ledge: the player already stands on top, the animation catches up.
        public bool IsLedgeClimbing { get; private set; }

        // How far the player was moved onto the ledge when the climb-up started.
        public Vector3 LedgeClimbDelta { get; private set; }
        public float LedgeClimbProgress => IsLedgeClimbing && _view != null ? _view.LedgeClimbProgress : 0f;
        public Vector3 WallNormal => _lastWallNormal;
        public Quaternion ClimbRotation => _climbRotation;
        public Vector3 ClimbForward => _climbRotation * Vector3.forward;

        // Degrees from a vertical wall: negative = ramp (inclination), positive = overhang (declination).
        public float SurfaceTilt { get; private set; }

        // After edge blocking, so it's what the player actually does (x = right, y = up).
        public Vector2 ClimbInput { get; private set; }

        public bool CanClimb
        {
            get
            {
                for (int i = 0; i < _gates.Length; i++)
                {
                    if (!_gates[i].AllowsClimbing) return false;
                }
                return true;
            }
        }

        public ClimbSettings Settings => settings;

        public event Action ClimbStarted;
        public event Action ClimbEnded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
            _motor = GetComponent<IPlayerMotor>();
            _view = GetComponent<IClimbView>();
            _gates = GetComponents<IClimbGate>();
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

            if (IsLedgeClimbing)
            {
                if (_ledgeClimbWait.IsDone(_view.IsPlayingLedgeClimb)) StopClimbing(clearSurface: false);
                return;
            }

            if (_climbStartPending && _climbStartWait.IsDone(_view.IsPlayingClimbStart)) _climbStartPending = false;

            Vector2 input = _climbStartPending ? Vector2.zero : SquareToCircle(_input.move);
            if (IsSliding) input.y = 0f;
            float dt = Time.deltaTime;

            Vector3 origin = transform.TransformPoint(_controller.center);
            Vector3 forward = ClimbForward;

            if (!Physics.Raycast(origin, forward, out RaycastHit hit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore))
            {
                StopClimbing();
                return;
            }

            // The straight-ahead ray carries us around outside corners. The averaged cross-pattern ray is
            // only used at inside corners: averaging everywhere pulls us back onto the face we're leaving.
            if (TryGetInsideCornerDirection(origin, hit, out Vector3 checkDirection) &&
                Physics.Raycast(origin, checkDirection, out RaycastHit averagedHit, settings.WallCheckDistance,
                    settings.ClimbableLayers, QueryTriggerInteraction.Ignore) &&
                GetSurfaceTilt(averagedHit.normal) <= settings.MaxDeclination)
            {
                hit = averagedHit;
                DrawDebugRay(origin, checkDirection, Color.magenta);
            }
            DrawDebugRay(origin, forward * hit.distance, Color.green);

            if (GetSurfaceTilt(hit.normal) < -settings.MaxInclination)
            {
                StopClimbing(clearSurface: false);
                return;
            }

            if ((input.y < -InputDeadZone || IsSliding) && IsFeetOnGround(origin))
            {
                StopClimbing();
                return;
            }

            // Slid onto a surface too steep to keep sliding on: fall off.
            if (IsSliding && GetSurfaceTilt(hit.normal) > -settings.SlideMinInclination)
            {
                StopClimbing();
                return;
            }
            _lastWallNormal = hit.normal;
            _lastWallPoint = hit.point;
            _hasWallPoint = true;

            Quaternion surfaceRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            _climbRotation = Quaternion.Slerp(_climbRotation, surfaceRotation, settings.RotationSharpness * dt);
            forward = ClimbForward;
            SurfaceTilt = GetSurfaceTilt(-forward);
            ApplyRootYaw(dt);

            if (input.y > InputDeadZone && !IsSliding && TryGetLedgeStandPoint(out Vector3 standPoint))
            {
                StartLedgeClimb(standPoint);
                return;
            }

            float wallDistance = _controller.radius + _controller.skinWidth + settings.WallGap;
            Vector3 targetOrigin = hit.point + hit.normal * wallDistance;
            Vector3 snap = Vector3.Lerp(origin, targetOrigin, settings.SnapSharpness * dt) - origin;

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

            if (IsSliding)
            {
                // The ramp ends below us (a ledge): fall off it.
                if (!CanMoveToward(origin, -up, forward))
                {
                    StopClimbing();
                    return;
                }
                _slideSpeed = Mathf.MoveTowards(_slideSpeed, settings.SlideSpeed, settings.SlideAcceleration * dt);
            }

            float sideSpeed = IsSliding ? settings.SlideSideSpeed : settings.ClimbSpeed;
            Vector3 velocity = right * (input.x * sideSpeed) + up * (input.y * settings.ClimbSpeed - _slideSpeed);
            _controller.Move(snap + velocity * dt);

            ClimbInput = input;
            if (_view == null) return;
            _view.SetClimbMove(IsSliding ? Vector2.zero : input);
            if (IsSliding) _view.SetSlideEffect(true, hit.point);
        }

        // Catches climbable surfaces the forward ray can't see, like a ramp under our feet.
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (IsClimbing || (settings.ClimbableLayers.value & (1 << hit.gameObject.layer)) == 0) return;
            if (!IsClimbableTilt(GetSurfaceTilt(hit.normal))) return;
            _contactNormal = hit.normal;
            _contactFrame = Time.frameCount;
        }

        // Tutorial's cross pattern: 4 forward rays around the center. Probe hits behind the front
        // surface's plane are an outside corner (ignored); hits clearly in front of it with a different
        // normal are an inside corner. Returns whether one was found, and the averaged direction into it.
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

        // The climb-up clip ends standing at its origin; its offset (measured from the clips) is how far that is
        // from where the player stands while climbing. So the stand point is that far ahead/up from the feet,
        // on the ledge top, reached once the top is no higher above the feet than the clip rises.
        private bool TryGetLedgeStandPoint(out Vector3 standPoint)
        {
            standPoint = default;
            if (_view == null || !_view.TryGetLedgeClimbOffset(out Vector3 localOffset) || localOffset.y <= 0f)
            {
                return false;
            }

            float clipRise = localOffset.y;
            Vector3 feet = transform.position;
            Vector3 horizontal = transform.TransformDirection(new Vector3(localOffset.x, 0f, localOffset.z));
            Vector3 probeStart = feet + horizontal + Vector3.up * (clipRise + LedgeProbeHeadroom);
            // A ray starting inside a taller wall doesn't hit it, so a wall without a ledge here finds nothing.
            if (!Physics.Raycast(probeStart, Vector3.down, out RaycastHit hit, LedgeProbeHeadroom + LedgeRiseTolerance,
                    _motor.GroundLayers, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            float rise = hit.point.y - feet.y;
            if (rise > clipRise || GetSurfaceTilt(hit.normal) >= -settings.MaxInclination) return false;

            standPoint = hit.point + Vector3.up * _controller.skinWidth;
            float radius = _controller.radius;
            Vector3 bottom = standPoint + Vector3.up * (radius + CapsuleFitMargin);
            Vector3 top = standPoint + Vector3.up * (_standingHeight - radius);
            return !Physics.CheckCapsule(bottom, top, radius, _motor.GroundLayers | settings.ClimbableLayers,
                QueryTriggerInteraction.Ignore);
        }

        // Moves the player to the stand point and starts the clip in the same frame: its first frame draws the
        // body back where it was, and the animation then carries it up onto the ledge.
        private void StartLedgeClimb(Vector3 standPoint)
        {
            IsLedgeClimbing = true;
            ClimbInput = Vector2.zero;
            _ledgeClimbWait.Begin();

            _controller.height = _standingHeight;
            LedgeClimbDelta = standPoint - transform.position;
            _controller.enabled = false; // required to teleport a CharacterController
            transform.position = standPoint;
            _controller.enabled = true;

            _view.SetClimbMove(Vector2.zero);
            _view.PlayLedgeClimb();
        }

        private void UpdateAutoGrab(float dt)
        {
            Vector3 surfaceNormal = Vector3.zero;
            bool pushingIntoWall = Time.time >= _regrabAllowedTime &&
                                   CanClimb &&
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
            Vector3 origin = transform.TransformPoint(_controller.center);
            bool playClimbStart = IsFeetOnGround(origin) &&
                                  GetSurfaceTilt(surfaceNormal) >= -settings.ClimbStartMaxInclination;
            StartClimbing(surfaceNormal, playClimbStart);
        }

        private void OnJumpPressed()
        {
            if (IsClimbing)
            {
                if (!IsSliding && !IsLedgeClimbing) JumpOff();
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

        public void JumpOff()
        {
            if (!IsClimbing) return;
            StopClimbing();
            _regrabAllowedTime = Time.time + settings.RegrabCooldown;
            Vector3 away = -FlatInto(_lastWallNormal);
            Vector2 jumpOff = settings.JumpOffVelocity;
            _motor.Launch(away * jumpOff.x + Vector3.up * jumpOff.y);
        }

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

            // One frame of slack: PlayerController may move (and report contacts) before or after our Update.
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

        private static float GetSurfaceTilt(Vector3 normal)
        {
            return Vector3.Angle(normal, Vector3.up) - 90f;
        }

        private static Vector3 FlatInto(Vector3 normal)
        {
            return Vector3.ProjectOnPlane(-normal, Vector3.up).normalized;
        }
        
        private bool CanMoveToward(Vector3 origin, Vector3 moveDirection, Vector3 intoSurface)
        {
            Vector3 probeOrigin = origin + moveDirection * settings.EdgeCheckOffset;
            return Physics.Raycast(probeOrigin, intoSurface, out RaycastHit hit, settings.WallCheckDistance,
                       settings.ClimbableLayers, QueryTriggerInteraction.Ignore) &&
                   GetSurfaceTilt(hit.normal) <= settings.MaxDeclination;
        }

        private bool IsFeetOnGround(Vector3 origin)
        {
            float reach = _controller.center.y + GroundProbeMargin;
            return Physics.Raycast(origin, Vector3.down, out RaycastHit hit, reach, _motor.GroundLayers,
                       QueryTriggerInteraction.Ignore) &&
                   GetSurfaceTilt(hit.normal) < -settings.MaxInclination;
        }

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

        public void StartClimbing(Vector3 surfaceNormal, bool fromGround)
        {
            if (IsClimbing || !CanClimb) return;
            IsClimbing = true;
            ClimbInput = Vector2.zero;
            _wallPushTimer = 0f;
            _climbStartPending = fromGround && _view != null;
            _climbStartWait.Begin();

            // Face the surface immediately so the first wall ray finds it, even on shallow ramps
            _climbRotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);
            _lastWallNormal = surfaceNormal;
            _hasWallPoint = false;
            SurfaceTilt = GetSurfaceTilt(surfaceNormal);

            // Sphere around the center: an upright capsule can't lie against a ramp or tuck under an overhang.
            _controller.height = _controller.radius * 2f;

            ClimbStarted?.Invoke();
            if (_view != null) _view.ClimbStarted(fromGround);
        }

        public void StartSliding()
        {
            if (!IsClimbing || IsSliding) return;
            IsSliding = true;
            _slideSpeed = 0f;
        }

        public void StopClimbing() => StopClimbing(true);

        private void StopClimbing(bool clearSurface)
        {
            if (!IsClimbing) return;
            IsClimbing = false;
            _climbStartPending = false;
            SurfaceTilt = 0f;
            ClimbInput = Vector2.zero;
            if (IsSliding && _view != null) _view.SetSlideEffect(false, Vector3.zero);
            IsSliding = false;
            IsLedgeClimbing = false;
            _slideSpeed = 0f;
            RestoreStandingCapsule(clearSurface);
            ClimbEnded?.Invoke();
            if (_view != null) _view.ClimbEnded();
        }
        
        private void RestoreStandingCapsule(bool clearSurface)
        {
            _controller.height = _standingHeight;

            Vector3 correction = clearSurface ? GetSurfaceClearance() : Vector3.zero;
            correction += Vector3.up * GetGroundLift(correction);
            if (correction.sqrMagnitude <= 0f) return;

            _controller.enabled = false; 
            transform.position += correction;
            _controller.enabled = true;
        }
        
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
