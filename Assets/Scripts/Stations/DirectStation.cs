using Items.Inventory;
using Items.Tools;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Stations
{
    public class DirectStation : Station
    {
        [Header("Direct Station")]
        [SerializeField] private InvSystem invSystem;
        [SerializeField] private SoTool soTool;

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
            button.onClick.AddListener(UseTool);
            
            InvSystem invPlayer = GameManager.Player.Inventory;

            invSystem.otherInvVinc = invPlayer;
            invPlayer.otherInvVinc = invSystem;
        }

        protected override void LeaveStation()
        {
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.RemoveListener(UseTool);
            base.LeaveStation();
            InvSystem invPlayer = GameManager.Player.Inventory;
            invSystem.otherInvVinc = null;
            invPlayer.otherInvVinc = null;
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
    }
}