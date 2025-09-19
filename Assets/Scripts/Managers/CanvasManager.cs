using System;
using Book;
using Items.Inventory;
using MiniGames;
using UnityEngine;

namespace Managers
{
    public class CanvasManager : MonoBehaviour
    {
        private static CanvasManager _instance;
        
        [SerializeField] private InvManager invManager;
        [SerializeField] private BookHandler bookHandler;
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
            bookHandler.gameObject.SetActive(true);
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
            return bookHandler.ToggleBook();
        }
        
        public bool ToggleMenu()
        {
            return false; //logica
        }
    }
}
