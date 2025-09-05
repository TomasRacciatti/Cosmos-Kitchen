using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstPersonCameraMovement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool _left = false;
    private bool _isPointer = false;

    private void Update()
    {
        if (_isPointer)
        {
            if (_left)
            {
                CameraMovement._instance.TurnLeft();
            }
            else
            {
                CameraMovement._instance.TurnRight();
            }
        }    
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _isPointer = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointer = false;
    }

}
