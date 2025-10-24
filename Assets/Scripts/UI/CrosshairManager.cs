using System.Collections;
using UnityEngine;

namespace UI
{
    public class CrosshairManager : MonoBehaviour
    {
        private static CrosshairManager _instance;
        
        [SerializeField] private RectTransform crosshairTransform;
        [SerializeField] private float minScale = 10f;
        [SerializeField] private float maxScale = 20f;
        [SerializeField] private float speed = 5f; 
        
        private bool isAnimating = false;
        private Coroutine animRoutine;
        
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

        public static void StartAnimate()
        {
            if (_instance == null) return;
            if (_instance.isAnimating) return;
            _instance.isAnimating = true;
            _instance.StartCoroutine(_instance.Animate());
        }
        
        public static void StopAnimate()
        {
            _instance.isAnimating = false;
        }
        
        private IEnumerator Animate()
        {
            float t = 0f;
            bool growing = true;

            while (isAnimating)
            {
                t += Time.deltaTime * speed;

                float scale = growing
                    ? Mathf.Lerp(minScale, maxScale, t)
                    : Mathf.Lerp(maxScale, minScale, t);

                crosshairTransform.sizeDelta = new Vector2(scale, scale);

                if (t >= 1f)
                {
                    t = 0f;
                    growing = !growing;
                }

                yield return null;
            }
            
            crosshairTransform.sizeDelta = new Vector2(minScale, minScale);
        }
    }
}