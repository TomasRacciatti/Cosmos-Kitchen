using System;

// Purpose: Global event signatures so UX (toasts/SFX) can subscribe without coupling to station internals.
// Notes: The ticker/stations will raise these when thresholds are crossed.
namespace Cooking
{
    public static class CookingEvents
    {
        /// <summary>
        /// Raised when any timed station slot hits an integer turn boundary: 1, 2, 3, or 4.
        /// args: (stationId, slotIndex, turnIndex)
        /// </summary>
        public static event Action<string, int, int> OnDonenessReached;

        /// <summary>
        /// Raised when any timed station slot exceeds max turns and becomes Burnt.
        /// args: (stationId, slotIndex)
        /// </summary>
        public static event Action<string, int> OnBurnt;

        // These invokers are public so systems can broadcast without referencing subscribers.
        public static void RaiseDonenessReached(string stationId, int slotIndex, int turnIndex)
            => OnDonenessReached?.Invoke(stationId, slotIndex, turnIndex);

        public static void RaiseBurnt(string stationId, int slotIndex)
            => OnBurnt?.Invoke(stationId, slotIndex);
    }
}
