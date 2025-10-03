using UnityEngine;

namespace Managers
{
    public class InteractButton : MonoBehaviour
    {
        private static InteractButton _instance;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
            Hide();
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
        }
        
        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
        }
    }
}