using UnityEngine;
using System;

// Owner: Tomi (systems/stations)
// Purpose: Lightweight model + events describing one active station slot being cooked.
// Notes: Ticker mutates this (accumulatedSeconds). Station listens for events to drive UX (toasts/SFX).

namespace Cooking.Tomi
{
    [Serializable]
    public class CookingSession
    {
        // Configuracion
        public CookingMethod method; 
        public int inventoryIndex; 
        public float secondsPerTurn;
        public int maxTurnsBeforeBurn = 4; // Dejo esto por las dudas, pero probablemente siempre quede como 4
        
        // Runtime
        public bool  isActive;
        public float accumulatedSeconds;
        
        // Eventos
        public event Action<int> OnDonenessCrossed; // Se dispara con cada cruce del umbral de cocina
        public event Action OnBurnt;
        
        // Helper
        public float TurnsCooked => (secondsPerTurn > 0f) ? (accumulatedSeconds / secondsPerTurn) : 0f;
        
        // Invokes
        public void RaiseDonenessCrossed(int turnIndex) => OnDonenessCrossed?.Invoke(turnIndex);
        public void RaiseBurnt() => OnBurnt?.Invoke();
    }
}
