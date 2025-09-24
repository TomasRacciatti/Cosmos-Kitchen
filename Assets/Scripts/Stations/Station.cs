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
        
        [Header("VFX")]
        [SerializeField] private AnimationClip openAnimation;
        [SerializeField] private AnimationClip closeAnimation;
        [Header("Canvas")]
        [SerializeField] private GameObject canvas;
        
        protected GameObject CanvasInstance;
        private Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
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