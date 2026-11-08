using Interfaces;
using UnityEngine;

namespace Characters.Player.Climbing
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(PlayerClimbing))]
    [RequireComponent(typeof(CharacterController))]
    public class ClimbBodyLean : MonoBehaviour
    {
        [Tooltip("The character model to tilt. Must be a direct child of this object (e.g. Cosmos_RIG).")]
        [SerializeField] private Transform model;
        [Tooltip("How fast the model leans in and out (higher = snappier).")]
        [SerializeField] private float leanSharpness = 10f;
        [Tooltip("How fast the model catches up after the player is shifted to stand up (higher = snappier).")]
        [SerializeField] private float standUpSharpness = 8f;
        [Tooltip("Part of the climb-up clip (0..1) over which the body blends from where it really was onto the clip's own path.")]
        [Range(0.1f, 1f)] [SerializeField] private float ledgeWarpPortion = 0.8f;

        private PlayerClimbing _climbing;
        private CharacterController _controller;
        private IClimbView _view;
        private Quaternion _baseLocalRotation;
        private Vector3 _baseLocalPosition;
        private Quaternion _lean = Quaternion.identity;
        private Vector3 _standUpOffset; // world space
        private bool _wasLedgeClimbing;
        private Quaternion _ledgeStartLean;
        private Vector3 _ledgeStartOffset; // local space

        private void Awake()
        {
            _climbing = GetComponent<PlayerClimbing>();
            _controller = GetComponent<CharacterController>();
            _view = GetComponent<IClimbView>();
            _baseLocalRotation = model.localRotation;
            _baseLocalPosition = model.localPosition;
        }

        private void OnEnable()
        {
            _climbing.StandUpShifted += OnStandUpShifted;
        }

        private void OnDisable()
        {
            _climbing.StandUpShifted -= OnStandUpShifted;
        }

        private void OnStandUpShifted(Vector3 shift)
        {
            _standUpOffset -= shift;
        }

        private void Update()
        {
            Vector3 localOffset;
            if (_climbing.IsLedgeClimbing)
            {
                if (!_wasLedgeClimbing) BeginLedgeWarp();
                float progress = Mathf.Clamp01(_climbing.LedgeClimbProgress / ledgeWarpPortion);
                float blend = Mathf.SmoothStep(0f, 1f, progress);
                _lean = Quaternion.Slerp(_ledgeStartLean, Quaternion.identity, blend);
                localOffset = Vector3.Lerp(_ledgeStartOffset, Vector3.zero, blend);
            }
            else
            {
                // Climbing frame relative to the (yaw-only) root = the pitch the model should have.
                Quaternion target = _climbing.IsClimbing
                    ? Quaternion.Inverse(transform.rotation) * _climbing.ClimbRotation
                    : Quaternion.identity;
                _lean = Quaternion.Slerp(_lean, target, leanSharpness * Time.deltaTime);
                _standUpOffset = Vector3.Lerp(_standUpOffset, Vector3.zero, standUpSharpness * Time.deltaTime);
                localOffset = transform.InverseTransformDirection(_standUpOffset);
            }
            _wasLedgeClimbing = _climbing.IsLedgeClimbing;

            // Rotate around the capsule center so the body stays at the wall distance.
            Vector3 pivot = _controller.center;
            model.localRotation = _lean * _baseLocalRotation;
            model.localPosition = pivot + _lean * (_baseLocalPosition - pivot) + localOffset;
        }

        // The climb-up clip's first frame draws the body upright at a fixed spot relative to the stand point, but
        // the real body can be elsewhere (lower and leaning on a ramp). Offset the model so that first frame lands
        // exactly where the body was; Update then blends the offset and lean out as the clip plays.
        private void BeginLedgeWarp()
        {
            _ledgeStartLean = _lean;
            _ledgeStartOffset = Vector3.zero;
            _standUpOffset = Vector3.zero;
            if (_view == null || !_view.TryGetLedgeClimbStartBody(out Vector3 clipStartBody)) return;

            Vector3 pivot = _controller.center;
            Vector3 clipStartDrawn = pivot + _ledgeStartLean * (clipStartBody - pivot);
            Vector3 bodyBefore = transform.InverseTransformPoint(_climbing.LedgeStartBodyPosition);
            _ledgeStartOffset = bodyBefore - clipStartDrawn;
        }
    }
}
