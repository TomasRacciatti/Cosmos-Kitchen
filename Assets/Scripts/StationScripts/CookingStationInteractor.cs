using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]

public class CookingStationInteractor : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CookingStationType _cookingStationType;
    private bool _interactionEnabled = false;
    private bool _panelOpen = false;
    private bool _canInteractPanel;
    
    [Header("SFX")]
    [SerializeField] AudioClip OpenStation;
    [SerializeField] AudioClip CloseStation;

    private void Update()
    {
        if (_interactionEnabled)
        {
            if (Input.GetKeyDown(KeyCode.E) && !PanelManager._instance._isAnimating)
            {
                Interaction();
            }
        }
    }

    void Interaction()
    {
        int number = (int)_cookingStationType;
        if (_panelOpen)
        {
            _panelOpen = false;
            AudioManager.instance.PlaySFX(CloseStation);
            PanelManager._instance.CloseCookingStation(number);
            _interactionEnabled = false;
        }
        else
        {
            _panelOpen = true;
            AudioManager.instance.PlaySFX(OpenStation); 
            PanelManager._instance.OpenCookingStation(number);
            PanelManager._instance.SetCurrent(this);
            _interactionEnabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_panelOpen && !PanelManager._instance._isAnimating)
        {
            Interaction();
        }
    }

    public void Closed()
    {
        _panelOpen = false;
        _interactionEnabled = false;
    }
}
