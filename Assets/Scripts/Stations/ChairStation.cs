using Managers;
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
            base.EnterStation();
            if (CanvasInstance == null) return;

            _hud = CanvasInstance.GetComponentInChildren<TravelHud>(true);
            if (_hud == null)
            {
                Debug.LogWarning($"[ChairStation] {name}: the station canvas has no TravelHud.", this);
                return;
            }

            _hud.Open(GameManager.Novas, SceneManager.GetActiveScene().name);
        }

        protected override void LeaveStation()
        {
            if (_hud != null)
            {
                _hud.Close();
                _hud = null;
            }
            base.LeaveStation();
        }
    }
}
