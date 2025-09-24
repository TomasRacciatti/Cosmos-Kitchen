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
    public class Station : MonoBehaviour, IInteractable
    {
        public Transform InteractionPoint => transform;
        
        [SerializeField] private InvSystem invSystem;
        [SerializeField] private GameObject canvas;
        [SerializeField] private SoTool soTool;
        [SerializeField] private AnimationClip openAnimation;
        [SerializeField] private AnimationClip closeAnimation;
        
        private GameObject canvasInstance;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (!invSystem) invSystem = GetComponent<InvSystem>();
        }

        public void Interact(GameObject interactableObject)
        {
            canvasInstance = ObjectPool.SpawnObject(canvas, transform.position, Quaternion.identity);
            InvView invView = canvasInstance.GetComponentInChildren<InvView>();
            invView.SetInventory(invSystem);
            GameManager.Player.SetInputActive(false);
            GameManager.Canvas.InvManager.ForceInventory(true);
            PlayerInputs.SetCursor(true);
            Button button = canvasInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(UseTool);
        }

        private void Update()
        {
            if (canvasInstance && Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveStation();
            }
        }

        private void LeaveStation()
        {
            ObjectPool.ReturnObjectToPool(canvasInstance);
            GameManager.Player.SetInputActive(true);
            GameManager.Canvas.InvManager.ForceInventory(false);
            PlayerInputs.SetCursor(false);
            Button button = canvasInstance.GetComponentInChildren<Button>();
            button.onClick.RemoveListener(UseTool);
            canvasInstance = null;
        }

        public void UseTool()
        {
            for (int i = 0; i < invSystem.Items.Count; i++)
            {
                var item = invSystem.Items[i];
                if (item.IsEmpty) continue;

                foreach (var tool in item.SoItem.Tools)
                {
                    if (tool.tool != soTool) continue;
                    item.SetItem(tool.item);
                    invSystem.NotifySlotChanged(i);
                    break;
                }
            }
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