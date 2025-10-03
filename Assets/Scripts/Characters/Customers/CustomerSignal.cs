using System.Collections.Generic;
using UnityEngine;

namespace Characters.Customers
{
    public class CustomerSignal : MonoBehaviour
    {
        [SerializeField] private List<GameObject> signals = new();

        public void SetSignal(int index)
        {
            for (int i = 0; i < signals.Count; i++)
            {
                signals[i].SetActive(index == i);
            }
        }
    }
}