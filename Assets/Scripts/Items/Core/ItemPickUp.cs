using System;
using UnityEngine;

namespace Items.Core
{
    [Serializable]
    public class ItemPickUp : MonoBehaviour
    {
        [SerializeField] private ItemAmount itemAmount;

        private void Awake()
        {
            if (itemAmount.IsEmpty) Destroy(gameObject);
        }
    }
}