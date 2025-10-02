using UnityEngine;

namespace UI
{
    public class CrosshairManager : MonoBehaviour
    {
        private static CrosshairManager _instance;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
        }

        public static void ShowCrosshair(bool visible)
        {
            if (_instance) _instance.gameObject.SetActive(visible);
        }
    }
}