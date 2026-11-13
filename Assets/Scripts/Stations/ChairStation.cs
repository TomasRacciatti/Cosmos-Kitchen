using Managers;
using Planets;
using UI.Travel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stations
{
    public class ChairStation : Station
    {
        protected override bool OpensInventory => false;
        protected override bool RequiresCamera => false;

        private TravelHud _hud;

        protected override void EnterStation()
        {
            if (GameManager.Scenes.IsLoading) return;

            base.EnterStation();
            if (CanvasInstance == null) return;

            _hud = CanvasInstance.GetComponentInChildren<TravelHud>(true);
            if (_hud == null)
            {
                Debug.LogWarning($"[ChairStation] {name}: the station canvas has no TravelHud.", this);
                return;
            }

            _hud.TravelRequested += OnTravelRequested;
            _hud.Open(GameManager.Novas, SceneManager.GetActiveScene().name);
        }

        protected override void LeaveStation()
        {
            if (_hud != null)
            {
                _hud.TravelRequested -= OnTravelRequested;
                _hud.Close();
                _hud = null;
            }
            base.LeaveStation();
        }

        private void OnTravelRequested(SoPlanet planet)
        {
            if (!GameManager.Scenes.CanLoad(planet.SceneName))
            {
                Debug.LogWarning($"[ChairStation] Can't travel to {planet.name}: scene '{planet.SceneName}' isn't in the Build Settings.", planet);
                return;
            }

            LeaveStation();
            // Keep the player still for the frames the async load takes.
            GameManager.Player.SetInputActive(false);
            GameManager.Scenes.TryLoad(planet.SceneName);
        }
    }
}
