using System;
using Items.Core;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player
{
    public class PlayerInputs : MonoBehaviour
    {
        public bool active = true;
        
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        private PlayerController _playerController;
        public bool inventoryOpen = false;
        public bool bookOpen = false;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            SetCursor(false);
        }

        public static void SetCursor(bool visible)
        {
            if (visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            move = active ? value.Get<Vector2>() : Vector2.zero;
        }

        public void OnLook(InputValue value)
        {
            //cambiar la condicion del operador ternario
            look = active && !inventoryOpen ? value.Get<Vector2>() : Vector2.zero;
        }

        public void OnJump(InputValue value)
        {
            /*
            jump = value.isPressed;
            if (jump) _playerController.Jump();
            //Jump Commented
            */
        }

        public void OnSprint(InputValue value)
        {
            sprint = active && value.isPressed;
        }

        public void OnInteract(InputValue value)
        {
            if (active && value.isPressed)
            {
                _playerController.CameraRaycast();
            }
        }

        public void OnInventory(InputValue value)
        {
            if (value.isPressed)
            {
                inventoryOpen = GameManager.Canvas.ToggleInventory();
                SetCursor(inventoryOpen);
            }
        }

        public void OnBook(InputValue value)
        {
            if (value.isPressed)
            {
                bookOpen = GameManager.Canvas.ToggleBook();
                SetCursor(bookOpen);
            }
        }

        public void OnBackUI(InputValue value)
        {
            if (value.isPressed)
            {
                
            }
        }

        public void OnTest(InputValue value)
        {
            if (value.isPressed)
            {

            }
        }
#endif
    }
}