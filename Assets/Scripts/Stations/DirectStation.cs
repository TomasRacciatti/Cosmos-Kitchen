using Items.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Stations
{
    public class DirectStation : Station
    {
        [SerializeField] private SoTool soTool;
        protected override void EnterStation()
        {
            base.EnterStation();
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(UseTool);
        }

        protected override void LeaveStation()
        {
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.RemoveListener(UseTool);
            base.LeaveStation();
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