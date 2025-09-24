using System;
using Characters.Player;
using Interfaces;
using Items.Inventory;
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
        
        [SerializeField] protected InvSystem invSystem;
        [SerializeField] private GameObject canvas;
        [SerializeField] private AnimationClip openAnimation;
        [SerializeField] private AnimationClip closeAnimation;
        
        protected GameObject CanvasInstance;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (!invSystem) invSystem = GetComponent<InvSystem>();
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

        protected virtual void EnterStation()
        {
            CanvasInstance = ObjectPool.SpawnObject(canvas, transform.position, Quaternion.identity);
            InvView invView = CanvasInstance.GetComponentInChildren<InvView>();
            invView.SetInventory(invSystem);
            GameManager.Player.SetInputActive(false);
            GameManager.Canvas.InvManager.ForceInventory(true);
            PlayerInputs.SetCursor(true);
        }

        protected virtual void LeaveStation()
        {
            ObjectPool.ReturnObjectToPool(CanvasInstance);
            GameManager.Player.SetInputActive(true);
            GameManager.Canvas.InvManager.ForceInventory(false);
            PlayerInputs.SetCursor(false);
            CanvasInstance = null;
        }

        public void EnableInteract()
        {
            if (!animator) return;
            if (!openAnimation) return;
            animator.Play(openAnimation.name, 0, 
                1 - Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
        }

        public void DisableInteract()
        {
            if (!animator) return;
            if (!closeAnimation) return;
            animator.Play(closeAnimation.name, 0, 
                1 - Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
        }
    }
}