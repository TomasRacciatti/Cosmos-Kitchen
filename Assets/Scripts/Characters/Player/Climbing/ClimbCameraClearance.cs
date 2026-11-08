using UnityEngine;

namespace Characters.Player.Climbing
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(PlayerClimbing))]
    public class ClimbCameraClearance : MonoBehaviour
    {
        [Tooltip("Camera follow targets to push out (e.g. ThirdPersonCameraRoot). Must be direct children of this object.")]
        [SerializeField] private Transform[] cameraTargets;
        [Tooltip("How far the targets are pushed away from the climbed surface (m).")]
        [SerializeField] private float clearance = 0.5f;
        [Tooltip("How fast the targets move out and back (higher = snappier).")]
        [SerializeField] private float sharpness = 8f;

        private PlayerClimbing _climbing;
        private Vector3[] _baseLocalPositions;
        private Vector3 _offset;
        private bool _wasLedgeClimbing;
        private Vector3 _ledgeStartOffset;

        private void Awake()
        {
            _climbing = GetComponent<PlayerClimbing>();
            _baseLocalPositions = new Vector3[cameraTargets.Length];
            for (int i = 0; i < cameraTargets.Length; i++)
            {
                _baseLocalPositions[i] = cameraTargets[i].localPosition;
            }
        }

        private void OnEnable()
        {
            _climbing.StandUpShifted += OnStandUpShifted;
        }

        private void OnDisable()
        {
            _climbing.StandUpShifted -= OnStandUpShifted;
        }

        // Keep the targets in place when the player is shifted to stand up; Update eases them back.
        private void OnStandUpShifted(Vector3 shift)
        {
            _offset -= shift;
        }

        private void Update()
        {
            if (_climbing.IsLedgeClimbing)
            {
                _offset = GetLedgeClimbOffset();
            }
            else
            {
                Vector3 target = _climbing.IsClimbing ? -_climbing.ClimbForward * clearance : Vector3.zero;
                _offset = Vector3.Lerp(_offset, target, sharpness * Time.deltaTime);
            }
            _wasLedgeClimbing = _climbing.IsLedgeClimbing;

            Vector3 localOffset = transform.InverseTransformDirection(_offset);
            for (int i = 0; i < cameraTargets.Length; i++)
            {
                cameraTargets[i].localPosition = _baseLocalPositions[i] + localOffset;
            }
        }

        // The player is teleported onto the ledge when the climb-up starts. Keep the targets where they were
        // that frame, then carry them up with the clip's progress so the camera follows the body instead of
        // snapping ahead of it.
        private Vector3 GetLedgeClimbOffset()
        {
            if (!_wasLedgeClimbing) _ledgeStartOffset = _offset - _climbing.LedgeClimbDelta;
            float t = Mathf.SmoothStep(0f, 1f, _climbing.LedgeClimbProgress);
            return Vector3.Lerp(_ledgeStartOffset, Vector3.zero, t);
        }
    }
}
