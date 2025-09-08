using System;
using Items.Inventory;
using UnityEngine;

namespace Managers
{
    public class CanvasManager : MonoBehaviour
    {
        private static CanvasManager _instance;
        
        [SerializeField] private InvManager invManager;
        [SerializeField] private GameObject pauseMenuUI;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
        }

        private void Start()
        {
            GameManager.RegisterCanvas(this);
        }
        
        public bool ToggleInventory()
        {
            return invManager.ToggleInventory();
        }
    }
}
