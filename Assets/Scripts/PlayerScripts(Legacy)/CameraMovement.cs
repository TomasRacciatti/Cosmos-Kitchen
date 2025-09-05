using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _followCamera;
    [SerializeField] private GameObject _firstPersonCamera;
    private int _cameraActive = 0;
    [SerializeField] private float _rotationSpeed = 5;
    [SerializeField] private Transform _firstPersonCameraTransform;
    [SerializeField] private GameObject[] _stationCameras;
    private Animator _animator;
    private string _changeCameraCondition = "Change";
    public static CameraMovement _instance { get; private set; }

    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _animator = GetComponent<Animator>();
    }

    public void RESETCAMERAPOSITION()
    {
        _firstPersonCamera.transform.position = _firstPersonCameraTransform.position;
        _firstPersonCamera.transform.rotation = _firstPersonCameraTransform.rotation;
        _firstPersonCamera.transform.localScale = _firstPersonCameraTransform.localScale;
    }

    private void ActivateMainCamera()
    {
        _firstPersonCamera.SetActive(false);
        _followCamera.SetActive(true);
        _mainCamera.SetActive(true);

        _cameraActive = 0;
    }

    private void ActivateFirstPersonCamera()
    {
        _followCamera.SetActive(false);
        RESETCAMERAPOSITION();
        _mainCamera.SetActive(false);
        _firstPersonCamera.SetActive(true);

        _cameraActive = 1;
    }

    public void GoToStation(int i)
    {
        StartCoroutine(ChangeStation(i));
    }
    public void ExitStation(int i)
    {
        StartCoroutine(ExitStationRoutine(i));
    }


    public void SwitchCamera()
    {
        StartCoroutine(SwitchCameraRoutine());
        if (_cameraActive == 0)
        {
            _followCamera.SetActive(false);
            ActivateFirstPersonCamera();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            ActivateMainCamera();
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    IEnumerator SwitchCameraRoutine()
    {
        _animator.SetTrigger(_changeCameraCondition);
        return new WaitForSecondsRealtime(0.2f);
    }

    IEnumerator ChangeStation(int i)
    {
        _stationCameras[10].transform.position = _firstPersonCamera.transform.position;
        _stationCameras[10].transform.rotation = _firstPersonCamera.transform.rotation;
        _stationCameras[10].SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        _stationCameras[i].SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        _stationCameras[10].SetActive(false);
    }
    IEnumerator ExitStationRoutine(int i)
    {
        _stationCameras[10].SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        _stationCameras[i].SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        _stationCameras[10].SetActive(false);
    }

    public void TurnLeft()
    {
        _firstPersonCamera.transform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime);
    }

    public void TurnRight()
    {
        _firstPersonCamera.transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

    }

}
