using System;
using Interfaces;
using Planets;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Travel
{
    public class TravelHud : MonoBehaviour
    {
        [SerializeField] private PlanetButton[] planets;
        [SerializeField] private PlanetInfoPanel infoPanel;
        [SerializeField] private Button travelButton;

        public event Action<SoPlanet> TravelRequested;

        private INovaProgress _progress;
        private string _currentSceneName;
        private PlanetButton _selected;

        private void Awake()
        {
            if (infoPanel == null) Debug.LogWarning($"[TravelHud] {name}: no Info Panel assigned.", this);
            if (travelButton == null) Debug.LogWarning($"[TravelHud] {name}: no Travel Button assigned.", this);

            foreach (var planet in planets)
            {
                if (planet != null) planet.Clicked += Select;
                else Debug.LogWarning($"[TravelHud] {name}: empty entry in Planets.", this);
            }
            if (travelButton != null) travelButton.onClick.AddListener(RequestTravel);
        }

        private void OnDestroy()
        {
            foreach (var planet in planets)
                if (planet != null) planet.Clicked -= Select;
            if (travelButton != null) travelButton.onClick.RemoveListener(RequestTravel);
            Close();
        }

        public void Open(INovaProgress progress, string currentSceneName)
        {
            Close();
            _progress = progress;
            _currentSceneName = currentSceneName;
            _progress.Changed += OnNovasChanged;

            RefreshPlanetStates();
            Select(FindCurrentPlanet());
        }

        public void Close()
        {
            if (_progress != null) _progress.Changed -= OnNovasChanged;
            Select(null);
            _progress = null;
        }

        private void OnNovasChanged(int novas)
        {
            RefreshPlanetStates();
            Select(_selected);
        }

        private void RefreshPlanetStates()
        {
            foreach (var button in planets)
            {
                if (button == null) continue;
                var planet = button.Planet;
                var current = planet != null && planet.SceneName == _currentSceneName;
                var locked = !current && (planet == null || !planet.IsUnlocked(_progress));
                button.SetState(locked, current);
            }
        }

        private PlanetButton FindCurrentPlanet()
        {
            foreach (var button in planets)
                if (button != null && button.IsCurrent) return button;
            return null;
        }

        private void Select(PlanetButton button)
        {
            if (_selected != null && _selected != button) _selected.SetSelected(false);
            _selected = button;
            if (_selected != null) _selected.SetSelected(true);

            var planet = _selected != null ? _selected.Planet : null;
            var locked = _selected != null && _selected.IsLocked;
            if (infoPanel != null) infoPanel.Show(planet, locked, _progress);

            if (travelButton != null)
                travelButton.gameObject.SetActive(planet != null && !locked && !_selected.IsCurrent
                                                  && !string.IsNullOrEmpty(planet.SceneName));
        }

        private void RequestTravel()
        {
            if (_selected != null && !_selected.IsLocked && !_selected.IsCurrent)
                TravelRequested?.Invoke(_selected.Planet);
        }
    }
}
