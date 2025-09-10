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
        
        [Header("Sounds")]
        [SerializeField] private AudioClip itemPickupSound;
        [SerializeField] private AudioClip itemThrowSound;

        //Properties
        public static GameObject Player => _instance.player;
        public static GameObject Canvas => _instance.canvas;
        public static GameObject ItemPrefabPickup => _instance.itemPrefabPickup;
        public static GameObject ItemPrefabUI => _instance.itemPrefabUI;
        public static GameObject SlotPrefabUI => _instance.slotPrefabUI;
        public static AudioClip ItemPickupSound => _instance.itemPickupSound;
        public static AudioClip ItemThrowSound => _instance.itemThrowSound;

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