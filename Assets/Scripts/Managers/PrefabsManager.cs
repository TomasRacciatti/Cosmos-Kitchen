using UnityEngine;

namespace Managers
{
    public class PrefabsManager : MonoBehaviour
    {
        private static PrefabsManager _instance;
        
        [Header("Player and Canvas")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject canvas;
        
        [Header("Items / Inventory")]
        [SerializeField] private GameObject itemPrefabPickup;
        [SerializeField] private GameObject itemPrefabUI;
        [SerializeField] private GameObject slotPrefabUI;

        //Properties
        public static GameObject Player => _instance.player;
        public static GameObject Canvas => _instance.canvas;
        public static GameObject ItemPrefabPickup => _instance.itemPrefabPickup;
        public static GameObject ItemPrefabUI => _instance.itemPrefabUI;
        public static GameObject SlotPrefabUI => _instance.slotPrefabUI;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
        }
    }
}