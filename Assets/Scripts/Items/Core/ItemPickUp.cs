using System;
using Interfaces;
using Items.Inventory;
using UnityEngine;

namespace Items.Core
{
    [Serializable]
    public class ItemPickUp : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemAmount itemAmount;
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
            // if (TestEmptyDestroy()) return; activar esto si usamos object pool
            
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
        
        public void Interact(GameObject interactableObject) //agarrar
        {
            if (!interactableObject.TryGetComponent(out InvSystem invSystem)) return;
            invSystem.AddItem(ref itemAmount);
            TestEmptyDestroy();
        }

        private bool TestEmptyDestroy()
        {
            if (!itemAmount.IsEmpty) return false;
            Destroy(gameObject);
            return true;
        }
    }
}