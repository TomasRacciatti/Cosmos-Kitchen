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
        
        [Header("Settings")]
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Transform interactionPoint;
        
        public Transform InteractionPoint => interactionPoint ? interactionPoint : transform;
        
        private void OnEnable()
        {
            if (TestEmptyDestroy()) return;
            
            //assign mesh and material
            if (itemAmount.SoItem.Mesh != null)
            {
                meshFilter.mesh = itemAmount.SoItem.Mesh;
                if (itemAmount.SoItem.Materials is { Length: > 0 })
                {
                    meshRenderer.materials = itemAmount.SoItem.Materials;
                }
            }
            else
            {
                meshFilter.mesh = PrefabsManager.ItemMesh;
                meshRenderer.materials = PrefabsManager.ItemMaterials;
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