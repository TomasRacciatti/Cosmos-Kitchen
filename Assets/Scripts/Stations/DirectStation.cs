using System.Collections;
using System.Collections.Generic;
using Cooking;
using Items.Inventory;
using Items.Tools;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Stations
{
    public class DirectStation : Station
    {
        [Header("Direct Station")]
        [SerializeField] private InvSystem invSystem;
        
        [SerializeField] private CookingMethod method;
        [SerializeField] private UnityEvent onCooked;
        [SerializeField] private float processDelay = 2f;
        
        private Image slotOverlay;
        private GameObject buttonOverlay;

        private InvView invView;

        protected override void Awake()
        {
            base.Awake();
            if (!invSystem) invSystem = GetComponent<InvSystem>();
        }

        protected override void EnterStation()
        {
            base.EnterStation();
            
            invView = CanvasInstance.GetComponentInChildren<InvView>();
            invView.SetInventory(invSystem);
            
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(ApplyDirectProcess);
            
            // Esto es medio fragil porque depende de que el nombre sea tal cual (y no se repita)
            slotOverlay = CanvasInstance.transform.Find("SlotOverlay").GetComponent<Image>();
            buttonOverlay= CanvasInstance.transform.Find("ButtonOverlay").gameObject;
            
            slotOverlay.gameObject.SetActive(false);
            buttonOverlay.SetActive(false);
            
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
            StartCoroutine(ProcessWithDelay());
        }

        private IEnumerator ProcessWithDelay()
        {
            slotOverlay.gameObject.SetActive(true);
            buttonOverlay.SetActive(true);

            InvItemUI itemUI = invView.GetComponentInChildren<InvItemUI>();
            itemUI.enabled = false;
            
            ProcessIngredient();
            
            slotOverlay.fillAmount = 1f;
            
            float elapsedTime = 0f;

            while (elapsedTime < processDelay)
            {
                elapsedTime += Time.deltaTime;
                slotOverlay.fillAmount = (processDelay - elapsedTime) / processDelay;
                yield return null;
            }
            
            itemUI.enabled = true;
            slotOverlay.gameObject.SetActive(false);
            buttonOverlay.SetActive(false);
        }

        private void ProcessIngredient()
        {
            animator.SetTrigger("StartCook");
            onCooked.Invoke();
            for (int i = 0; i < invSystem.Items.Count; i++)
            {
                var item = invSystem.Items[i];
                if (item.IsEmpty) continue;
                
                if (item.Prep.Doneness == Cooking.Doneness.Burnt) continue;
                
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