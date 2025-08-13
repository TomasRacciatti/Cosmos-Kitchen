using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class PanelManager : MonoBehaviour
{
    public static PanelManager _instance;
    [SerializeField] GameObject[] _panels;
    [SerializeField] MenuManager _menuManager;
    [SerializeField] GameObject _firstPersonHud;

    public bool _isAnimating = false;
    private int _currentPanelOpen = 99;
    private CookingStationInteractor _currentStation;

    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }   
    }

    public void OpenCookingStation(int number)
    {
        _isAnimating = true;
        CameraMovement._instance.GoToStation(number);
        StartCoroutine(OpenStation(number));
        _currentPanelOpen = number;
    }

    public void CloseCookingStation(int number)
    {
        _isAnimating = true;
        _panels[number].SetActive(false);
        CameraMovement._instance.ExitStation(number);
        StartCoroutine(CloseStation(number));
        _currentPanelOpen = 99;
        _currentStation.Closed();
    }

    IEnumerator OpenStation(int i)
    {
        yield return new WaitForSeconds(0.7f);
        _panels[i].SetActive(true);
        _isAnimating = false;
    }
    IEnumerator CloseStation(int i)
    {
        yield return new WaitForSeconds(0.7f);
        _isAnimating = false;
    }

    public void TogglePauseMenu()
    {
        CloseAll();
        _menuManager.ToggleMainMenu();
    }

    public void CloseAll()
    {
        if (_currentPanelOpen < 11)
        {
            CloseCookingStation(_currentPanelOpen);
        }
    }
    
    public void OpenFirstPerson()
    {
        _firstPersonHud.SetActive(true);
    }

    public void CloseFirstPerson()
    {
        _firstPersonHud.SetActive(false);
    }

    public void SetCurrent(CookingStationInteractor station)
    {
        _currentStation = station;
    }
}
