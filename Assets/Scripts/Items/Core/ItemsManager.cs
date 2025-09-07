using UnityEngine;

namespace Items.Core
{
    public class ItemsManager : MonoBehaviour
    {
        public static ItemsManager Instance { get; private set; }

        public GameObject itemPrefabPickup;
        public GameObject itemPrefabUI;
        public GameObject slotPrefabUI;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance.gameObject);
                return;
            }
            Instance = this;
        }
    }
}