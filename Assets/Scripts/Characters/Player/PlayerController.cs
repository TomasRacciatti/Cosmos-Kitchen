using System;
using Cinemachine;
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
        [Header("Player")]
        public float moveSpeed = 3.0f;
        public float sprintSpeed = 4.5f;
        
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;
        public float speedChangeRate = 10.0f;
        
        public float jumpHeight = 1.2f;
        public float gravity = -15.0f;
        
        public float jumpTimeout = 0.1f;
        public float fallTimeout = 0.15f;
        
        [Header("Player Grounded")]
        public bool grounded = true;
        public LayerMask groundLayers;

        [Header("Cinemachine")]
        public GameObject cinemachineCameraTarget1;
        public GameObject cinemachineCameraTarget2;
        public float topClamp = 85f;
        public float bottomClamp = -85f;
        
        public float cameraAngleOverride;
        
        public bool lockCameraPosition;
        
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
        [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
        
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

        private const float Threshold = 0.01f;
        
        private ECameraMode _cameraMode = ECameraMode.ThirdPerson;


        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerView = GetComponent<PlayerView>();
        }

        private void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget1.transform.rotation.eulerAngles.y;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - (-_characterController.radius),
                transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, _characterController.radius, groundLayers,
                QueryTriggerInteraction.Ignore);

            _playerView.Landed();
        }
        private void OnDrawGizmosSelected()
        {
            if (_characterController == null) return;

            // --- GroundedCheck ---
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - (-_characterController.radius),
                transform.position.z
            );

            bool groundedHit = Physics.CheckSphere(
                spherePosition,
                _characterController.radius,
                groundLayers,
                QueryTriggerInteraction.Ignore
            );

            Gizmos.color = groundedHit ? Color.blue : Color.cyan;
            Gizmos.DrawWireSphere(spherePosition, _characterController.radius);
        }

        
        private void CeilingCheck()
        {
            if (_verticalVelocity <= 0f) return;
            
            float headPositionY = transform.position.y + _characterController.height - _characterController.radius + 0.05f;
            
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                headPositionY,
                transform.position.z
            );

            if (Physics.CheckSphere(spherePosition, _characterController.radius, groundLayers, QueryTriggerInteraction.Ignore))
            {
                _verticalVelocity = 0f;
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                _cinemachineTargetYaw += _input.look.x;
                _cinemachineTargetPitch += _input.look.y;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget1.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
            cinemachineCameraTarget2.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
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
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
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
                // reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;
                
                _playerView.SetGrounded(true);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    _playerView.SetFalling(true);
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
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

        public void SwitchCameraMode() //rever
        {
            _cameraMode = _cameraMode != ECameraMode.ThirdPerson ? ECameraMode.ThirdPerson : ECameraMode.FirstPerson;
            switch (_cameraMode)
            {
                case ECameraMode.FirstPerson:
                    firstPersonCamera.Priority = 20;
                    Invoke(nameof(SetLockForwardTrue), 1.5f);
                    break;
                case ECameraMode.ThirdPerson:
                    firstPersonCamera.Priority = 0;
                    lockForwardFacing = false;
                    break;
            }
        }

        private void SetLockForwardTrue() //rever
        {
            lockForwardFacing = true;
        }
    }
    
    public enum ECameraMode
    {
        ThirdPerson,
        FirstPerson,
        Custom
    }
}