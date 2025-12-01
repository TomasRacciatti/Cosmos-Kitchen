using System;
using System.Collections.Generic;
using Characters.Customers;
using Cinemachine;
using Interfaces;
using Items.Inventory;
using Managers;
using UnityEngine;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Characters.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputs))]
    [RequireComponent(typeof(PlayerView))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")] public float moveSpeed = 3.0f;
        public float sprintSpeed = 4.5f;

        [Range(0.0f, 0.3f)] public float rotationSmoothTime = 0.12f;
        public float speedChangeRate = 10.0f;

        public float jumpHeight = 1.2f;
        public float gravity = -15.0f;

        public float jumpTimeout = 0.1f;
        public float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [SerializeField] private bool grounded = true;
        public bool Grounded => grounded;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundedOffset = 0.1f;

        [Header("Cinemachine")] [SerializeField]
        private GameObject cinemachineCameraTarget1;

        [SerializeField] private GameObject cinemachineCameraTarget2;
        [SerializeField] private float topClamp = 85f;
        [SerializeField] private float bottomClamp = -85f;
        [SerializeField] private float cameraAngleOverride;
        [SerializeField] private bool lockCameraPosition;

        //[SerializeField] private float cameraRadius = 10f;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
        [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
        [SerializeField] private CinemachineVirtualCamera actualCamera;

        [SerializeField] public Camera MainCamera => mainCamera;
        [SerializeField] public CinemachineVirtualCamera FirstPersonCamera => firstPersonCamera;
        [SerializeField] public CinemachineVirtualCamera ThirdPersonCamera => thirdPersonCamera;
        [SerializeField] public CinemachineVirtualCamera ActualCamera => actualCamera;

        [Header("Movement Options")]
        [SerializeField] private bool lockForwardFacing;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private const float TerminalVelocity = 50.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private CharacterController _controller;
        private PlayerInputs _input;
        private CharacterController _characterController;
        private PlayerView _playerView;
        private InvSystem _inventory;

        private const float Threshold = 0.01f;

        public InvSystem Inventory => _inventory;
        public PlayerInputs Input => _input;

        private int _score = 0;

        public List<Customer> critics = new();

        public void AddScore(int score)
        {
            _score += score;
            if (_score > 3)
            {
                foreach (var critic in critics)
                {
                    critic.SetCriticSignal();
                }
            }
        }
        
        public int GetScore()
        {
            return _score;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerView = GetComponent<PlayerView>();
            _input = GetComponent<PlayerInputs>();
            _controller = GetComponent<CharacterController>();
            _inventory = GetComponent<InvSystem>();
            SetCamera(ThirdPersonCamera);
        }

        private void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget1.transform.rotation.eulerAngles.y;
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
            GameManager.Resume();
            GameManager.RegisterPlayer(this);
            Instantiate(PrefabsManager.Canvas, null, false);
        }

        private void Update()
        {
            GroundedCheck();
            CeilingCheck();
            VerticalMovement();
            HorizontalMovement();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + 
                _characterController.radius - groundedOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, _characterController.radius, groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void CeilingCheck()
        {
            if (_verticalVelocity <= 0f) return;

            float headPositionY = transform.position.y + _characterController.height - _characterController.radius +
                                  groundedOffset;

            Vector3 spherePosition = new Vector3(
                transform.position.x,
                headPositionY,
                transform.position.z
            );

            if (Physics.CheckSphere(spherePosition, _characterController.radius, groundLayers,
                    QueryTriggerInteraction.Ignore))
            {
                _verticalVelocity = 0f;
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                _cinemachineTargetYaw += _input.look.x;
                _cinemachineTargetPitch += _input.look.y;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget1.transform.rotation = Quaternion.Euler(
                _cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
            cinemachineCameraTarget2.transform.rotation = cinemachineCameraTarget1.transform.rotation;
        }

        private void HorizontalMovement()
        {
            float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                    Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            Vector3 targetDirection;

            if (!lockForwardFacing)
            {
                if (_input.move != Vector2.zero)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                        ref _rotationVelocity,
                        rotationSmoothTime);

                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

                targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            }
            else
            {
                float cameraYaw = mainCamera.transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0.0f, cameraYaw, 0.0f);

                targetDirection = mainCamera.transform.forward * inputDirection.z +
                                  mainCamera.transform.right * inputDirection.x;
                targetDirection.y = 0f;
                targetDirection.Normalize();
            }

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            _playerView.SetSpeed(_animationBlend);
        }

        private void VerticalMovement()
        {
            if (grounded)
            {
                _fallTimeoutDelta = fallTimeout;

                _playerView.SetGrounded(true);

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -1f;
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    _playerView.SetFalling(true);
                }
            }

            if (_verticalVelocity < TerminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        public void Jump()
        {
            if (_jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _playerView.Jumped();
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            _playerView.Footstep();
        }

        public void SetCamera(CinemachineVirtualCamera newCamera)
        {
            CancelInvoke(nameof(SetLockForwardTrue));
            if (actualCamera != null) actualCamera.Priority = 0;
            actualCamera = newCamera;
            if (actualCamera != null) actualCamera.Priority = 10;
            if (actualCamera == thirdPersonCamera)
            {
                lockForwardFacing = false;
                _input.active = true;
                return;
            }

            if (actualCamera == firstPersonCamera)
            {
                Invoke(nameof(SetLockForwardTrue), 0.8f);
                _input.active = true;
                return;
            }

            lockForwardFacing = false;
            _input.active = false;
        }

        public void SetThirdPersonCamera()
        {
            SetCamera(thirdPersonCamera);
        }
        
        public void SetFirstPersonCamera()
        {
            SetCamera(firstPersonCamera);
        }

        private void SetLockForwardTrue()
        {
            lockForwardFacing = true;
        }

        public void SetInputActive(bool value)
        {
            _input.active = value;
        }
        
        public void SetMoveActive(bool value)
        {
            _input.SetCanMove(value);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            _characterController.enabled = false;
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            _characterController.enabled = true;
        }

        public void SetPositionAndRotationWithCamera(Vector3 position, Quaternion rotation)
        {
            SetPositionAndRotation(position, rotation);
            
            float yaw = rotation.eulerAngles.y;
            _targetRotation = yaw;
            _cinemachineTargetYaw = yaw;
            
            cinemachineCameraTarget1.transform.rotation = Quaternion.Euler(
                _cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
            cinemachineCameraTarget2.transform.rotation = cinemachineCameraTarget1.transform.rotation;
        }
        
        public Vector3 GetThrowPosition => transform.position + 1f * transform.forward + transform.up;
    }
}