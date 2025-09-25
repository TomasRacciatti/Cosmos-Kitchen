using UnityEngine;

namespace UI.Components
{
    public class LifeBar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float lifeWidth = 82f;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetLives(int lives)
        {
            float newWidth = lives * lifeWidth;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }
}