using System;
using Interfaces;
using Items.Inventory;
using Regulators;
using UnityEngine;

namespace Items.Core
{
    [Serializable]
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemAmount itemAmount;
        
        public Transform InteractionPoint => transform;
        
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        private void Awake()
        {
            if (TestEmptyDestroy()) return;

            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        private void OnEnable()
        {
            if (TestEmptyDestroy()) return;
            
            //assign mesh and material
            if (itemAmount.SoItem.Mesh != null)
            {
                _meshFilter.mesh = itemAmount.SoItem.Mesh;
            }
            if (itemAmount.SoItem.Materials is { Length: > 0 })
            {
                _meshRenderer.materials = itemAmount.SoItem.Materials;
            }
        }

        public void SetItemAmount(ItemAmount newItemAmount)
        {
            itemAmount = newItemAmount;
        }
        
        public void Interact(GameObject interactableObject) //pick up
        {
            if (!interactableObject.TryGetComponent(out InvSystem invSystem)) return;
            invSystem.AddItem(ref itemAmount);
            TestEmptyDestroy();
        }

        private bool TestEmptyDestroy()
        {
            if (!itemAmount.IsEmpty) return false;
            ObjectPool.ReturnObjectToPool(gameObject);
            return true;
        }
    }
}