using Interfaces;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerCutsceneHold : MonoBehaviour
    {
        private Renderer[] _renderers;
        private ISuspendableMotor _motor;
        private PlayerInputs _input;
        private bool _held;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _motor = GetComponentInParent<ISuspendableMotor>();
            _input = GetComponentInParent<PlayerInputs>();
        }

        public void Hold()
        {
            if (_held) return;
            _held = true;
            SetVisible(false);
            _motor?.SetSuspended(true);
            if (_input == null) return;
            _input.inCinematic = true;
            _input.move = Vector2.zero;
            _input.look = Vector2.zero;
        }

        public void Release()
        {
            if (!_held) return;
            _held = false;
            SetVisible(true);
            _motor?.SetSuspended(false);
            if (_input != null) _input.inCinematic = false;
        }

        private void SetVisible(bool visible)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null) _renderers[i].forceRenderingOff = !visible;
            }
        }
    }
}
