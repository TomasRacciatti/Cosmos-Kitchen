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

        private PlayerClimbing _climbing;
        private CharacterController _controller;
        private Quaternion _baseLocalRotation;
        private Vector3 _baseLocalPosition;
        private Quaternion _lean = Quaternion.identity;

        private void Awake()
        {
            _climbing = GetComponent<PlayerClimbing>();
            _controller = GetComponent<CharacterController>();
            _baseLocalRotation = model.localRotation;
            _baseLocalPosition = model.localPosition;
        }

        private void Update()
        {
            // Climbing frame relative to the (yaw-only) root = the pitch the model should have.
            Quaternion target = _climbing.IsClimbing && !_climbing.IsLedgeClimbing
                ? Quaternion.Inverse(transform.rotation) * _climbing.ClimbRotation
                : Quaternion.identity;
            _lean = Quaternion.Slerp(_lean, target, leanSharpness * Time.deltaTime);

            // Rotate around the capsule center so the body stays at the wall distance.
            Vector3 pivot = _controller.center;
            model.localRotation = _lean * _baseLocalRotation;
            model.localPosition = pivot + _lean * (_baseLocalPosition - pivot);
        }
    }
}
