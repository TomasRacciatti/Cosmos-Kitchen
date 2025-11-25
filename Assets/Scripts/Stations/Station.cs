using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Player;
using Cinemachine;
using Interfaces;
using Items.Inventory;
using Cooking;
using Items.Core;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stations
{
    public abstract class Station : MonoBehaviour, IInteractable
    {
        public Transform InteractionPoint => transform;
        
        [Header("Canvas")]
        [SerializeField] protected GameObject canvas;

        [Header("Camera settings")] 
        [SerializeField] protected CinemachineVirtualCamera stationCamera;
        
        [Header ("Audio Settings")]
        [SerializeField, Tooltip("Se puede dejar vacio y se settea en awake")] protected AudioSource audioSource;
        [SerializeField] protected AudioClip processingClip;
        
        [Header("Animation")]
        [SerializeField] protected Animator animator;
        
        protected GameObject CanvasInstance;
        private Outline _outlineComponent;

        protected virtual void Awake()
        {
            _outlineComponent = GetComponent<Outline>();
            
            if (_outlineComponent != null) _outlineComponent.enabled = false;
            
            if (audioSource == null) 
                audioSource = GetComponent<AudioSource>();
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
            if (stationCamera == null)
            {
                Debug.LogWarning($"Station {gameObject.name} is missing stationCamera or playerPosition reference!");
                return;
            }
            
            GameManager.Player.SetCamera(stationCamera);

            // El teleport este rompia las camaras porque probablemente triggereaba un exit de la cocina sin que nos demos cuenta
            //StartCoroutine(TeleportPlayerAfterDelay());
            
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
            
            GameManager.Player.SetFirstPersonCamera();
            
            ObjectPool.ReturnObjectToPool(CanvasInstance);
            GameManager.Player.SetInputActive(true);
            GameManager.Canvas.InvManager.ForceInventory(false);
            PlayerInputs.SetCursor(false);
            CanvasInstance = null;
        }

        public void EnableInteract()
        {
            if (_outlineComponent != null) 
            {
                _outlineComponent.enabled = true;
            }
            
            animator.SetBool("IsOver", true);
        }

        public void DisableInteract()
        {
            if (_outlineComponent != null)
            {
                _outlineComponent.enabled = false;
            }
            
            animator.SetBool("IsOver", false);
        }

        // private IEnumerator TeleportPlayerAfterDelay()
        // {
        //     yield return new WaitForSeconds(1.5f);
        //     GameManager.Player.SetPositionAndRotationWithCamera(teleportPosition.position, teleportPosition.rotation);
        // }
    }
}