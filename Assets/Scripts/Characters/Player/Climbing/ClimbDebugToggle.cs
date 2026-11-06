using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player.Climbing
{
    /// <summary>
    /// Temporary test helper: toggles climbing with a key.
    /// Remove once automatic wall grabbing is in (Iteration 2).
    /// </summary>
    [RequireComponent(typeof(PlayerClimbing))]
    public class ClimbDebugToggle : MonoBehaviour
    {
        [SerializeField] private Key toggleKey = Key.C;

        private PlayerClimbing _climbing;

        private void Awake()
        {
            _climbing = GetComponent<PlayerClimbing>();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard[toggleKey].wasPressedThisFrame) return;

            if (_climbing.IsClimbing) _climbing.StopClimbing();
            else _climbing.StartClimbing();
        }
    }
}
