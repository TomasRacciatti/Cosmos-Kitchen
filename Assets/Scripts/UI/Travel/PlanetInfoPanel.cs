using Interfaces;
using Planets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Travel
{
    public class PlanetInfoPanel : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text planetName;
        [SerializeField] private TMP_Text description;

        [Header("Content")]
        [SerializeField, Tooltip("Shown for unlocked planets (PlanetData: description, tabs, list).")]
        private GameObject unlockedContent;
        [SerializeField, Tooltip("Shown for locked planets (PlanetBlocked).")]
        private PlanetLockView lockView;

        private void Awake()
        {
            if (icon == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Icon assigned.", this);
            if (planetName == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Planet Name assigned.", this);
            if (description == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Description assigned.", this);
            if (unlockedContent == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Unlocked Content assigned.", this);
            if (lockView == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Lock View assigned.", this);
        }

        public void Show(SoPlanet planet, bool locked, INovaProgress progress)
        {
            var hasPlanet = planet != null;

            if (icon != null)
            {
                icon.sprite = hasPlanet ? planet.Icon : null;
                icon.enabled = icon.sprite != null;
            }
            if (planetName != null) planetName.text = hasPlanet ? planet.DisplayName : string.Empty;
            if (description != null) description.text = hasPlanet ? planet.Description : string.Empty;

            if (unlockedContent != null) unlockedContent.SetActive(hasPlanet && !locked);

            if (lockView == null) return;
            if (hasPlanet && locked) lockView.Show(planet, progress);
            else lockView.Hide();
        }
    }
}
