using System;

// Purpose: Interface for the world-time cooking service that advances all active CookingSessions.
// Notes: Stations register/unregister sessions here. UI and Serving should not depend on this directly.

namespace Cooking
{
    public interface ICookingTicker
    {
        // El reloj que usa el ticker para avanzar las sesiones
        float SecondsPerTick { get; }
        
        // Registra que la sesion tiene que avanzar con el ticker
        void Register(CookingSession session);
        
        // Deja de avanzar la sesion (saco el item o corto la estacion)
        void Unregister(CookingSession session);
    }
}
