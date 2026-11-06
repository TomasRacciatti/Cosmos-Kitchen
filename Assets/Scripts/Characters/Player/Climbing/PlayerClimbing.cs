using System;
using UnityEngine;

namespace Characters.Player.Climbing
{
    /// <summary>
    /// Climbing movement. Owns the CharacterController while climbing;
    /// PlayerController steps aside whenever <see cref="IsClimbing"/> is true.
    /// Based on the Madalaski BOTW climbing tutorial, adapted to CharacterController + Input System.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputs))]
    public class PlayerClimbing : MonoBehaviour
    {
        [SerializeField] private ClimbSettings settings;

        private CharacterController _controller;
        private PlayerInputs _input;

        public bool IsClimbing { get; private set; }

        public event Action ClimbStarted;
        public event Action ClimbEnded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
        }

        private void Update()
        {
            if (!IsClimbing) return;

            Vector2 input = SquareToCircle(_input.move);
            
            Vector3 velocity = transform.TransformDirection(input) * settings.ClimbSpeed;
            _controller.Move(velocity * Time.deltaTime);
        }

        public void StartClimbing()
        {
            if (IsClimbing) return;
            IsClimbing = true;
            ClimbStarted?.Invoke();
        }

        public void StopClimbing()
        {
            if (!IsClimbing) return;
            IsClimbing = false;
            ClimbEnded?.Invoke();
        }

        // Keeps diagonal input from being faster than straight input.
        private static Vector2 SquareToCircle(Vector2 input)
        {
            return input.sqrMagnitude >= 1f ? input.normalized : input;
        }
    }
}
