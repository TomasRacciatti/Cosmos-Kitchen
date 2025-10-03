using Items.Inventory;
using Managers;
using Regulators;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

namespace Items.Core
{
    public class ItemsDropper : MonoBehaviour, IDropHandler
    {
        private static ItemsDropper _instance;
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        
        public static bool IsActive => _instance.gameObject.activeSelf;

        private void Awake()
        {
            _instance = this;
            Hide();
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            InvItemUI fromItemUI = eventData.pointerDrag.GetComponent<InvItemUI>();
            if (fromItemUI == null) return;
            InvSlotUI slotUI = fromItemUI.SlotUI;
            Drop(fromItemUI.ItemAmount);
            slotUI.InvView.InventorySystem.SetItemByIndex(slotUI.InvSlot, new ItemAmount());
            Hide();
        }

        public static void Drop(ItemAmount itemAmount)
        {
            if (itemAmount.IsEmpty) return;
            GameObject itemPickup = ObjectPool.SpawnObject(PrefabsManager.ItemPrefabPickup, GameManager.Player.GetThrowPosition, Quaternion.identity, false);
            itemPickup.GetComponent<ItemPickup>().SetItemAmount(new ItemAmount(itemAmount));
            itemPickup.SetActive(true);
            AudioSource temp = new GameObject("TempAudio").AddComponent<AudioSource>();
            temp.spatialBlend = 0f; // 2D
            temp.outputAudioMixerGroup = _instance.audioMixerGroup;
            temp.PlayOneShot(PrefabsManager.ItemThrowSound);
            Rigidbody rb = itemPickup.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((GameManager.Player.transform.forward + Vector3.up).normalized * 3f, ForceMode.Impulse);
            }
        }
    }
}
