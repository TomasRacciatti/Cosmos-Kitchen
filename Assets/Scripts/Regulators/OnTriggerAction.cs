using UnityEngine;
using UnityEngine.Events;

namespace Regulators
{
    public class OnTriggerAction : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;

        private void OnTriggerEnter(Collider other)
        {
            onEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            onExit?.Invoke();
        }
    }
}