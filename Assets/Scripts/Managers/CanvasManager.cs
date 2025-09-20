using Book;
using Items.Inventory;
using UnityEngine;

namespace Managers
{
    public class CanvasManager : MonoBehaviour
    {
        private static CanvasManager _instance;
        
        [SerializeField] private InvManager invManager;
        [SerializeField] private BookHandler bookHandler;
        [SerializeField] private GameObject pauseMenuUI;
        public InvManager InvManager => invManager;

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
