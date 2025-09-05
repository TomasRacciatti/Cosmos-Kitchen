using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private PlayerViewer _playerViewer;

    [SerializeField]
    private Transform _cameraTransform;
    
    [SerializeField]
    float _speed = 5f;

    private Vector3 _movementDirection;
    private float _movementMagnitude;
    private Vector3 _movementSpeed;
    private float _gravity = -9.81f;
    [SerializeField]
    private float _gravityMultiplier;
    private float _gravityVelocity;
    
    [Header("SFX")]
    
    [SerializeField] AudioClip NormalStep;

    [SerializeField] AudioClip[] GrassFootsteps;
    [SerializeField] AudioClip[] GravelFootsteps;
    [SerializeField] AudioClip[] WoodFootsteps;

    void FootStepEvent(int whichFoot)
    {
        Debug.Log("Foot step: " + whichFoot);
        if (whichFoot == 0)
        {
            AudioManager.instance.PlaySFX(NormalStep);
        }
        else
        {
            AudioManager.instance.PlaySFX(NormalStep);
        }
    }
    
    private void Awake()
    {
        GetDependencies();
    }

    private void Start()
    {
        InputManager._instance.SetPlayerMovement(this);
    }

    void Update()
    {
        Input_();
    }

    private void FixedUpdate()
    {
        Gravity();
        Movement();
        Rotate();
    }

    private void Input_()
    {
        float _horizontalInput = Input.GetAxisRaw("Horizontal");
        float _verticalInput = Input.GetAxisRaw("Vertical");
        _movementDirection = new Vector3(_horizontalInput, 0, _verticalInput);
        _movementMagnitude = Mathf.Clamp01(_movementDirection.magnitude); 
        _movementDirection = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up) * _movementDirection;
        _movementDirection.Normalize();
        _movementMagnitude = _speed;
        _movementSpeed = _movementDirection * _movementMagnitude;
    }

    void Gravity()
    {
        if (_characterController.isGrounded && _gravityVelocity < 0f)
        {
            _gravityVelocity = -1f;
        }
        else
        {
            _gravityVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
        }
        _movementSpeed.y = _gravityVelocity;
    }

    void Movement()
    {
        _characterController.Move(_movementSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        if (_movementDirection != Vector3.zero)
        {
            _playerViewer.Rotate(_movementDirection);
            _playerViewer.Walk();
        }
        else
        {
            _playerViewer.Idle();
        }
    }

    void GetDependencies()
    {
        _characterController = GetComponent<CharacterController>();
        _playerViewer = GetComponent<PlayerViewer>();
    }
}
