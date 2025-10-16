using UnityEngine;

// Owner: Tomi (systems/stations)
// Purpose: ScriptableObject with per-method timing defaults so designers can tune without code.
// Notes: Stations/ticker can read these defaults; concrete items may override later if needed.

namespace Cooking.Tomi
{
    [CreateAssetMenu(menuName = "ScriptableObject/Cooking/Balance", fileName = "CookingBalance")]
    public class CookingBalance : ScriptableObject
    {
        [Header("Seconds per Turn (design knobs)")]
        public float boilSecondsPerTurn  = 7f;
        public float frySecondsPerTurn   = 6f;
        public float roastSecondsPerTurn = 10f;
        public float blendSecondsPerTurn = 5f;
        public float chopSecondsPerTurn  = 0f; 

        [Header("Max Turns Before Burn (normally 4)")]
        public int boilMaxTurnsBeforeBurn  = 4;
        public int fryMaxTurnsBeforeBurn   = 4;
        public int roastMaxTurnsBeforeBurn = 4;
        public int blendMaxTurnsBeforeBurn = 4;
        public int chopMaxTurnsBeforeBurn  = 0;
    }
}
