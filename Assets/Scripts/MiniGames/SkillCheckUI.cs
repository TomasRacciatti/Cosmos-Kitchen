using UnityEngine;

namespace MiniGames
{
    public class SkillCheckUI : MonoBehaviour
    {
        [SerializeField] private RectTransform zoneTransform;
        [SerializeField] private RectTransform needleTransform;
        
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponentInChildren<RectTransform>();
        }

        public void SetSkillCheck(float zoneAngle, float needleAngle, Vector2 position, float size)
        {
            zoneTransform.localEulerAngles = new Vector3(0f, 0f, zoneAngle);
            SetNeedle(needleAngle);
            _rectTransform.localScale = new Vector2(size, size);
            _rectTransform.anchoredPosition = position;
        }
        
        public void SetNeedle(float needleAngle)
        {
            needleTransform.localEulerAngles = new Vector3(0f, 0f, needleAngle);
        }
    }
}
