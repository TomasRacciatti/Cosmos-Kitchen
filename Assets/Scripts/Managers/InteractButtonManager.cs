using UnityEngine;

namespace Managers
{
    public class InteractButtonManager : MonoBehaviour
    {
        private static InteractButtonManager _instance;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
        }
    }
}