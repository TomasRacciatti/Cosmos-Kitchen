using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("Display Settings")]
        [SerializeField] private bool showTotal = true;
        [SerializeField] private int currentDecimals = 2;

        public void SetProgress(float newCurrent, float newTotal)
        {
            var current = Mathf.Clamp(newCurrent, 0, newTotal);
            var total = Mathf.Max(1, newTotal);

            float fill = current / total;

            if (fillImage != null) fillImage.fillAmount = fill;

            if (progressText != null)
            {
                string currentFormatted = current.ToString($"F{currentDecimals}");

                if (showTotal)
                {
                    progressText.text = $"{currentFormatted} / {total}";
                }
                else
                {
                    progressText.text = currentFormatted;
                }
            }
        }
    }
}