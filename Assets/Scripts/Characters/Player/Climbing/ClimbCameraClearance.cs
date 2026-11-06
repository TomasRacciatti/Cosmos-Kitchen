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

        private void Awake()
        {
            _climbing = GetComponent<PlayerClimbing>();
            _baseLocalPositions = new Vector3[cameraTargets.Length];
            for (int i = 0; i < cameraTargets.Length; i++)
            {
                _baseLocalPositions[i] = cameraTargets[i].localPosition;
            }
        }

        private void Update()
        {
            Vector3 target = _climbing.IsClimbing ? -_climbing.ClimbForward * clearance : Vector3.zero;
            _offset = Vector3.Lerp(_offset, target, sharpness * Time.deltaTime);

            Vector3 localOffset = transform.InverseTransformDirection(_offset);
            for (int i = 0; i < cameraTargets.Length; i++)
            {
                cameraTargets[i].localPosition = _baseLocalPositions[i] + localOffset;
            }
        }
    }
}
