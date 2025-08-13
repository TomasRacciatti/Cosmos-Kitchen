using UnityEngine;
using System;

public class CameraReference : MonoBehaviour
{
    public static Camera Instance { get; private set; }
    public static event Action<Camera> OnCameraAvailable;

    private void OnEnable()
    {
        Instance = GetComponent<Camera>();
        OnCameraAvailable?.Invoke(Instance);
    }

    private void OnDisable()
    {
        if (Instance == GetComponent<Camera>())
        {
            Instance = null;
        }
    }
}
