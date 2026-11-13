using Interfaces;
using Planets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Travel
{
    public class PlanetLockView : MonoBehaviour
    {
        [SerializeField, Tooltip("Image Type: Filled.")] private Image progressFill;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text missingText;

        [Header("Texts")]
        [SerializeField, Tooltip("{0} = Novas collected, {1} = Novas required")]
        private string progressFormat = "{0}/{1}";
        [SerializeField, TextArea(2, 4), Tooltip("{0} = Novas missing")]
        private string missingFormat = "Need {0} more Novas";
        [SerializeField, TextArea(2, 4), Tooltip("Used when exactly one Nova is missing.")]
        private string missingOneText = "Need 1 more Nova";

        public void Show(SoPlanet planet, INovaProgress progress)
        {
            gameObject.SetActive(true);

            var required = planet.RequiredNovas;
            var collected = Mathf.Min(progress.Novas, required);
            var missing = planet.MissingNovas(progress);

            if (progressFill != null) progressFill.fillAmount = required > 0 ? (float)collected / required : 1f;
            if (progressText != null) progressText.text = string.Format(progressFormat, collected, required);
            if (missingText != null)
                missingText.text = missing == 1 ? missingOneText : string.Format(missingFormat, missing);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
