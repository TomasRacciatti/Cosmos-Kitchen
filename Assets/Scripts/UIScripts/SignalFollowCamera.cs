using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalFollowCamera : MonoBehaviour
{
    [SerializeField] Transform _mainCamera;

    private void Start()
    {
        _mainCamera = FindObjectOfType<CinemachineBrain>().transform;
    }

    private void Update()
    {
        if (_mainCamera != null)
        {
            Vector3 direction = _mainCamera.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
