using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player
{
	public class PlayerInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		
		private PlayerController _playerController;

		private void Awake()
		{
			_playerController = GetComponent<PlayerController>();
		}

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			move = value.Get<Vector2>();
		}

		public void OnLook(InputValue value)
		{
			look = value.Get<Vector2>();
		}

		public void OnJump(InputValue value)
		{
			jump = value.isPressed;
			//if (jump) _playerController.Jump(); //Jump Commented
		}

		public void OnSprint(InputValue value)
		{
			sprint = value.isPressed;
		}
#endif
	}
}