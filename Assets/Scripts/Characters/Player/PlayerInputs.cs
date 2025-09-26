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
        
        public bool canMove = true;
        
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool inventoryOpen = false;
        public bool bookOpen = false;
        public bool menuOpen = false;
        
        private PlayerController _playerController;
        private InteractComponent _interactComponent;

        public bool ActiveAndNoHUD => active && !inventoryOpen && !bookOpen && !menuOpen;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _interactComponent = GetComponent<InteractComponent>();
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

        private void CheckCursor()
        {
            SetCursor(inventoryOpen || menuOpen || bookOpen);
        }
        
#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            move = !active || !canMove ? Vector2.zero : value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            //cambiar la condicion del operador ternario
            look = ActiveAndNoHUD && canMove ? value.Get<Vector2>() : Vector2.zero;
        }

        public void OnJump(InputValue value)
        {
            if (!active) return;
            jump = value.isPressed;
            if (jump) _playerController.Jump();
        }

        public void OnSprint(InputValue value)
        {
            sprint = active && value.isPressed;
        }

        public void OnInteract(InputValue value)
        {
            if (!ActiveAndNoHUD || !value.isPressed) return;
            _interactComponent.Interact();
        }

        public void OnInventory(InputValue value)
        {
            if (!active || !value.isPressed || bookOpen || menuOpen) return;
            
            inventoryOpen = GameManager.Canvas.ToggleInventory();
            CheckCursor();
        }

        public void OnBook(InputValue value)
        {
            if (!active || !value.isPressed || inventoryOpen || menuOpen) return;
            bookOpen = GameManager.Canvas.ToggleBook();
            
            CheckCursor();
        }

        public void OnBackUI(InputValue value)
        {
            if (!active || !value.isPressed) return;
            
            
            
            CheckCursor();
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