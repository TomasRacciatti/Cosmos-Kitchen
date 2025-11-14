using System.Collections.Generic;
using Cooking;
using Items.Inventory;
using Items.Tools;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Stations
{
    public class DirectStation : Station
    {
        [Header("Direct Station")]
        [SerializeField] private InvSystem invSystem;
        //[SerializeField] private SoTool soTool;
        
        [SerializeField] private CookingMethod method;
        [SerializeField] private UnityEvent onCooked;

        protected override void Awake()
        {
            base.Awake();
            if (!invSystem) invSystem = GetComponent<InvSystem>();
        }

        protected override void EnterStation()
        {
            base.EnterStation();
            
            InvView invView = CanvasInstance.GetComponentInChildren<InvView>();
            invView.SetInventory(invSystem);
            
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(ApplyDirectProcess);
            
            InvSystem invPlayer = GameManager.Player.Inventory;

            invSystem.otherInvVinc = invPlayer;
            invPlayer.otherInvVinc = invSystem;
        }

        protected override void LeaveStation()
        {
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.RemoveListener(ApplyDirectProcess);
            
            base.LeaveStation();
            
            InvSystem invPlayer = GameManager.Player.Inventory;
            invSystem.otherInvVinc = null;
            invPlayer.otherInvVinc = null;
        }
        
        private  void ApplyDirectProcess()
        {
            animator.SetTrigger("StartCook");
            onCooked.Invoke();
            for (int i = 0; i < invSystem.Items.Count; i++)
            {
                var item = invSystem.Items[i];
                if (item.IsEmpty) continue;
                
                if (item.Prep.Doneness == Cooking.Doneness.Burnt) continue;

                // bool toolAllowed = false;
                // var tools = item.SoItem.Tools;
                // if (tools != null)
                // {
                //     for (int t = 0; t < tools.Length; t++)
                //     {
                //         if (tools[t].tool == soTool) { toolAllowed = true; break; }
                //     }
                // }
                // if (!toolAllowed) continue;
                
                // Validacion de prereq
                if (!ProcessingPrerequisiteChecker.CanProcess(item, method, out string failureReason))
                {
                    Debug.Log($"Cannot process {item.SoItem.ItemName}: {failureReason}");
                    return;
                }
                
                item.AddProcessStep(method, 1);
                
                var prep = item.Prep;
                prep.method = method;
                prep.turnsCooked = 1; // Para los visuales nomas
                item.Prep = prep;
                invSystem.NotifySlotChanged(i);
                
                prep.method = CookingMethod.None;
                prep.turnsCooked = 0f;
                item.Prep = prep;
                invSystem.NotifySlotChanged(i);
            }
        }
        
        protected override IEnumerable<InvSystem> GetInventoriesForAcceptance()
        {
            if (invSystem != null) yield return invSystem;
        }
        
        protected override bool CanAcceptAtThisStation(Items.Core.ItemAmount item)
        {
            if (!base.CanAcceptAtThisStation(item)) return false;

            return ProcessingPrerequisiteChecker.CanProcess(item, method, out _);
        }
    }
}