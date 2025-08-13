using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerViewer : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform _modelTransform;
    [SerializeField] CinemachineBrain _cinemachineBrain;
    [SerializeField] float _rotationSpeed = 5f;

    private void Start()
    {
        InputManager._instance.SetPlayerViewer(this);
        InputManager._instance.SetThirdPersonCamera(_cinemachineBrain);
    }
    public void Walk()
    {
        animator.SetBool("IsMoving", true);
    }

    public void Idle()
    {
        animator.SetBool("IsMoving", false);
    }

    public void Rotate(Vector3 _movementDirection)
    {
        Quaternion _toRotation = Quaternion.LookRotation(_movementDirection ,Vector3.up);

        _modelTransform.rotation = Quaternion.RotateTowards(_modelTransform.rotation, _toRotation, _rotationSpeed);
    }

    public void Fishing(bool fishing)
    {
        animator.SetBool("Fishing", fishing);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    public void HidePlayer()
    {
        _modelTransform.gameObject.SetActive(false);
    }
    public void ShowPlayer()
    {
        _modelTransform.gameObject.SetActive(true);
    }
}
