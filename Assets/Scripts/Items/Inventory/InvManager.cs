using System;
using Characters.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items.Inventory
{
    public class InvManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private bool inventoryOpen = false;
        
        public bool IsOpen => inventoryOpen;

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
        }

        private void Start()
        {
            animator.Play("CloseInventory", 0,1f);
        }

        public void ForceInventory(bool open)
        {
            if (inventoryOpen == open) return;
            ToggleInventory();
        }

        public bool ToggleInventory()
        {
            inventoryOpen = !inventoryOpen;
            animator.speed = 1;
            animator.Play(inventoryOpen ? "OpenInventory" : "CloseInventory", 0, 
                1 - Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
            return inventoryOpen;
        }
    }
}
