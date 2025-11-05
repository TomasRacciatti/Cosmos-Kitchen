using System;
using System.Collections.Generic;
using Characters.Player;
using Interfaces;
using Items.Inventory;
using Cooking;
using Items.Core;
using Items.Tools;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.UI;

namespace Stations
{
    public abstract class Station : MonoBehaviour, IInteractable
    {
        public Transform InteractionPoint => transform;
        
        [Header("VFX")]
        [SerializeField] private AnimationClip openAnimation;
        [SerializeField] private AnimationClip closeAnimation;
        [Header("Canvas")]
        [SerializeField] protected GameObject canvas;
        
        [Header("Outline")]
        // Remove: [SerializeField] private Material outlineMat;
        // Remove: [SerializeField] private MeshRenderer[] outlineMeshes;
        private Outline _outlineComponent; // new
        
        protected GameObject CanvasInstance;
        private Animator animator;
        private string openClipName;
        private string closeClipName;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            
            openClipName = openAnimation  ? openAnimation.name : "";
            closeClipName = closeAnimation   ? closeAnimation.name : "";
            
            // New: Get the Outline component
            _outlineComponent = GetComponent<Outline>(); // new
            
            // REMOVE ALL OUTLINE MAT/MESH INITIALIZATION CODE:
            /*
            if (outlineMat)
            {
                outlineMat = new Material(outlineMat);
                for (int i = 0; i < outlineMeshes.Length; i++)
                {
                    Renderer rend = outlineMeshes[i];
                    if (!rend) continue;

                    var mats = new List<Material>(rend.materials);
                    mats.Add(outlineMat);
                    
                    rend.materials = mats.ToArray();
                }
                
                outlineMat.SetFloat("_Intensity", 0);
            }
            */
            
            // New: Ensure the outline is disabled initially (handled by OnDisable in Outline.cs)
            if (_outlineComponent != null) _outlineComponent.enabled = false; // new
        }

        public void Interact(GameObject interactableObject)
        {
            EnterStation();
        }

        private void Update()
        {
            if (CanvasInstance && Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveStation();
            }
        }
        
        protected virtual IEnumerable<InvSystem> GetInventoriesForAcceptance() { yield break; }
        
        protected virtual bool CanAcceptAtThisStation(ItemAmount it)
        {
            if (it == null || it.IsEmpty) return false;
            return it.Prep.Doneness != Doneness.Burnt;
        }

        protected virtual void EnterStation()
        {
            CanvasInstance = ObjectPool.SpawnObject(canvas, transform.position, Quaternion.identity);
            GameManager.Player.SetInputActive(false);
            GameManager.Canvas.InvManager.ForceInventory(true);
            PlayerInputs.SetCursor(true);
            
            foreach (var inv in GetInventoriesForAcceptance())
                if (inv != null) inv.CanAcceptItem = CanAcceptAtThisStation;
        }

        protected virtual void LeaveStation()
        {
            foreach (var inv in GetInventoriesForAcceptance())
                            if (inv != null) inv.CanAcceptItem = null;
            
            ObjectPool.ReturnObjectToPool(CanvasInstance);
            GameManager.Player.SetInputActive(true);
            GameManager.Canvas.InvManager.ForceInventory(false);
            PlayerInputs.SetCursor(false);
            CanvasInstance = null;
        }

        public void EnableInteract()
        {
            // New: Enable the Outline component
            if (_outlineComponent != null) // new
            {                              // new
                _outlineComponent.enabled = true; // new
            }                              // new
            /* Removed manual shader setting:
            if (outlineMat && outlineMeshes.Length > 0)
            {
                outlineMat.SetFloat("_Intensity", 1);
            }
            */
            
            if (!animator) return;
            if (openClipName == "") return;
            animator.Play(openClipName, 0, 
                1 - Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
        }

        public void DisableInteract()
        {
            // New: Disable the Outline component
            if (_outlineComponent != null) // new
            {                              // new
                _outlineComponent.enabled = false; // new
            }                              // new

            /* Removed manual shader setting:
            if (outlineMat && outlineMeshes.Length > 0)
            {
                for (int i = 0; i < outlineMeshes.Length; i++)
                    outlineMat.SetFloat("_Intensity", 0);
            }
            */
            
            if (!animator) return;
            if (closeClipName == "") return;
            animator.Play(closeClipName, 0, 
                1 - Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
        }
    }
}