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
        [SerializeField] private Mesh itemMesh;
        [SerializeField] private Material[] itemMaterials;
        
        [Header("Notifications")]
        [SerializeField] private GameObject notificationPrefabUI;
        [SerializeField] private Sprite notificationWinUI;
        [SerializeField] private Sprite notificationLoseUI;
        
        [Header("Sounds")]
        [SerializeField] private AudioClip itemPickupSound;
        [SerializeField] private AudioClip itemThrowSound;
        
        [Header("Notifications")]
        [SerializeField] private Sprite[] itemStars;

        //Properties
        public static GameObject Player => _instance.player;
        public static GameObject Canvas => _instance.canvas;
        public static GameObject ItemPrefabPickup => _instance.itemPrefabPickup;
        public static GameObject ItemPrefabUI => _instance.itemPrefabUI;
        public static GameObject SlotPrefabUI => _instance.slotPrefabUI;
        public static Mesh ItemMesh => _instance.itemMesh;
        public static Material[] ItemMaterials => _instance.itemMaterials;
        public static GameObject NotificationPrefabUI => _instance.notificationPrefabUI;
        public static Sprite NotificationWinUI => _instance.notificationWinUI;
        public static Sprite NotificationLoseUI => _instance.notificationLoseUI;
        public static AudioClip ItemPickupSound => _instance.itemPickupSound;
        public static AudioClip ItemThrowSound => _instance.itemThrowSound;

        public static Sprite[] ItemStars => _instance.itemStars;
        
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