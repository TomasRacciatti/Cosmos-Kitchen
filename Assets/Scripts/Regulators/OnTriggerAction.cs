using Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Regulators
{
    public class OnTriggerAction : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;

        private int _playerContacts;

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            _playerContacts++;
            if (_playerContacts == 1) onEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (_playerContacts == 0 || !IsPlayer(other)) return;
            _playerContacts--;
            if (_playerContacts == 0) onExit?.Invoke();
        }

        private void OnDisable()
        {
            _playerContacts = 0;
        }

        private static bool IsPlayer(Collider other) => other.TryGetComponent<IPlayerMotor>(out _);
    }
}
