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

        [Header("Ingredients / Clients")]
        [SerializeField] private PlanetContentTabs tabs;
        [SerializeField] private PlanetEntryList entryList;

        private SoPlanet _shownPlanet;

        private void Awake()
        {
            if (icon == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Icon assigned.", this);
            if (planetName == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Planet Name assigned.", this);
            if (description == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Description assigned.", this);
            if (unlockedContent == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Unlocked Content assigned.", this);
            if (lockView == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Lock View assigned.", this);
            if (tabs == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Tabs assigned.", this);
            if (entryList == null) Debug.LogWarning($"[PlanetInfoPanel] {name}: no Entry List assigned.", this);

            if (tabs != null) tabs.Changed += OnTabChanged;
        }

        private void OnDestroy()
        {
            if (tabs != null) tabs.Changed -= OnTabChanged;
        }

        public void Show(SoPlanet planet, bool locked, INovaProgress progress)
        {
            var hasPlanet = planet != null;
            _shownPlanet = hasPlanet && !locked ? planet : null;

            if (icon != null)
            {
                icon.sprite = hasPlanet ? planet.Icon : null;
                icon.enabled = icon.sprite != null;
            }
            if (planetName != null) planetName.text = hasPlanet ? planet.DisplayName : string.Empty;
            if (description != null) description.text = hasPlanet ? planet.Description : string.Empty;

            if (unlockedContent != null) unlockedContent.SetActive(_shownPlanet != null);
            if (_shownPlanet != null) FillEntries();

            if (lockView == null) return;
            if (hasPlanet && locked) lockView.Show(planet, progress);
            else lockView.Hide();
        }

        private void OnTabChanged(PlanetContentTab tab)
        {
            if (_shownPlanet != null) FillEntries();
        }

        private void FillEntries()
        {
            if (entryList == null) return;

            entryList.BeginFill();
            var tab = tabs != null ? tabs.Current : PlanetContentTab.Ingredients;
            if (tab == PlanetContentTab.Ingredients) AddIngredients();
            else AddClients();
            entryList.EndFill();
        }

        private void AddIngredients()
        {
            var ingredients = _shownPlanet.Ingredients;
            for (var i = 0; i < ingredients.Count; i++)
            {
                var item = ingredients[i];
                if (item == null) continue;
                var sprite = item.VisualSet != null ? item.VisualSet.DefaultSprite : null;
                entryList.Add(sprite, item.ItemName);
            }
        }

        private void AddClients()
        {
            var clients = _shownPlanet.Clients;
            for (var i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                if (client != null) entryList.Add(client.clientIcon, client.clientName);
            }
        }
    }
}
