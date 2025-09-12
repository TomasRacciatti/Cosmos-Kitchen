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
        [SerializeField] private MiniGamesUIManager miniGamesUI;

        public InvManager InvManager => invManager;
        public MiniGamesUIManager MiniGamesUI => miniGamesUI;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
            
            invManager.gameObject.SetActive(true);
        }

        private void Start()
        {
            GameManager.RegisterCanvas(this);
        }
        
        public bool ToggleInventory()
        {
            return invManager.ToggleInventory();
        }
        
        public bool ToggleBook()
        {
            return false; //logica
        }
        
        public bool ToggleMenu()
        {
            return false; //logica
        }
    }
}
