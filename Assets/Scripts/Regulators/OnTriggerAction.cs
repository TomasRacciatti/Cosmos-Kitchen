using UnityEngine;
using UnityEngine.Events;

namespace Regulators
{
    public class OnTriggerAction : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;

        private int colliders;

        private void OnTriggerEnter(Collider other)
        {
            colliders++;
            if (colliders != 1)
                return;
            onEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            colliders--;
            if (colliders != 0)
                return;
            onExit?.Invoke();
        }
    }
}