using System;
using Interfaces;
using Items.Inventory;
using Managers;
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
            else
            {
                //Debug.LogWarning("No Mesh Found"); //activar despues para que no sea rompebolas
            }
            if (itemAmount.SoItem.Materials is { Length: > 0 })
            {
                _meshRenderer.materials = itemAmount.SoItem.Materials;
            }
            
            Invoke(nameof(ReturnInTime), 300); // hacer que baje de calidad con el tiempo tirado
        }

        public void SetItemAmount(ItemAmount newItemAmount)
        {
            itemAmount = newItemAmount;
        }

        private void ReturnInTime()
        {
            ObjectPool.ReturnObjectToPool(gameObject);
        }
        
        public void Interact(GameObject interactableObject) //pick up
        {
            if (!interactableObject.TryGetComponent(out InvSystem invSystem)) return;
            invSystem.AddItem(ref itemAmount);
            if (TestEmptyDestroy())
            {
                AudioSource.PlayClipAtPoint(PrefabsManager.ItemPickupSound, transform.position);
            }
            else
            {
                //inv lleno
            }
        }

        public void EnableInteract()
        {
            
        }

        public void DisableInteract()
        {
            
        }

        private bool TestEmptyDestroy()
        {
            if (!itemAmount.IsEmpty) return false;
            ObjectPool.ReturnObjectToPool(gameObject);
            return true;
        }
    }
}