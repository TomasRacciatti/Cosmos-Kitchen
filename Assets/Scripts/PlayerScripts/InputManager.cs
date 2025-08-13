using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager _instance;
    private PlayerMovement _movement;
    private PlayerViewer _viewer;
    private CinemachineBrain _thirdPersonCamera;
    private bool _isMouseLocked;
    private bool _canMouseBeLocked = true;
    private bool _canMovementBeUnlocked = true;
    public bool _canToggleBook = true;
    public bool _canReceiveInput = true;
    public bool _canInteract = true;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else { Destroy(this); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_canReceiveInput)
            {
                InteractInventory();
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (_canReceiveInput)
            {
                InteractBook();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InteractPauseMenu();
        }
    }

    public void InteractInventory()
    {
        if (_canToggleBook)
        {
            if (_canMouseBeLocked)
            {
                SwitchMouselock();
            }
            InventoryManager._instance.ToggleInventory();
        }
    }
    public void InteractBook()
    {
        if (_canToggleBook)
        {
            if (_canMouseBeLocked)
            {
                SwitchMouselock();
            }
            InventoryManager._instance.ToggleBook();
        }
    }

    public void InteractPauseMenu()
    {
        if (_canToggleBook && !PanelManager._instance._isAnimating)
        {
            if (_canMouseBeLocked)
            {
                SwitchMouselock();
            }
            InventoryManager._instance.CloseAll();
            DialogueManager._instance.CloseDialogue();
            PanelManager._instance.CloseAll();
            PanelManager._instance.TogglePauseMenu();
        }
    }

    public void SetPlayerMovement(PlayerMovement _set)
    {
        if (_movement == null)
        _movement = _set;
    }

    public void SetPlayerViewer(PlayerViewer _set)
    {
        if (_viewer == null)
            _viewer = _set;
    }

    public void SetThirdPersonCamera(CinemachineBrain _set)
    {
        if (_thirdPersonCamera == null)
            _thirdPersonCamera = _set;
    }

    public void StopPlayerMovement()
    {
        _movement.enabled = false;
        _viewer.Idle();
    }

    public void ResumePlayerMovement()
    {
        if (_canMovementBeUnlocked)
        {
            _movement.enabled = true;
            _viewer.enabled = true;
        }
    }

    public void StopCamera()
    {
        _thirdPersonCamera.enabled = false;
    }

    public void ResumeCamera()
    {
        if (_canMovementBeUnlocked)
            _thirdPersonCamera.enabled = true;
    }

    public void UnlockMouse()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        _isMouseLocked = false;
    }

    public void LockMouse()
    {
        if (_canMouseBeLocked)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            _isMouseLocked = true;
        }
    }

    public void SwitchMouselock()
    {
        if (_isMouseLocked)
        {
            UnlockMouse();
        }
        else
        {
            if (_canMouseBeLocked)
            {
                LockMouse();
            }
        }
    }

    public void TurnOffMouselock()
    {
        Debug.Log("No se puede bloquear el mouse");
        _canMouseBeLocked = false;
    }
    public void TurnOnMouselock()
    {
        Debug.Log("Se puede bloquear el mouse");
        _canMouseBeLocked = true;
    }

    public void StopAll()
    {
        StopCamera();
        StopPlayerMovement();
    }
    public void ResumeAll()
    {
        ResumeCamera();
        ResumePlayerMovement();
    }
    
    public void StopInput()
    {
        _canReceiveInput = false;
        _canInteract = false;
    }
    public void ResumeInput()
    {
        _canReceiveInput = true;
        _canInteract = true;
    }

    public void LockMovement()
    {
        _canMovementBeUnlocked = false;
    }
    public void UnlockMovement()
    {
        _canMovementBeUnlocked = true;
    }

    public void TurnOnInteraction()
    {
        _canInteract = true;
    }
    public void TurnOffInteraction()
    {
        _canInteract = false;
    }

}
