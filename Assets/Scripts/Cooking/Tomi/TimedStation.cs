using Stations;
using Cooking;
using System;
using Items.Core;
using UnityEngine;

// Owner: Tomi (systems/stations)
// Purpose: Concrete, data-driven world-time cooking station. Attach to in-world prefabs.
// Notes: Configure via Inspector (method, timings, slots). Registers CookingSessions with ICookingTicker.

namespace Cooking.Tomi
{
    public abstract class TimedStation : Station
    {
        [Header("Cooking Config")]
        [SerializeField] private CookingMethod method = CookingMethod.Boil;
        [SerializeField] private float secondsPerTurn = 7f;
        [SerializeField] private int maxTurnsBeforeBurn = 4;
        [SerializeField] private int slotCount = 1;

        [Header("World-Time Ticker (provide a component implementing ICookingTicker)")]
        [SerializeField] private MonoBehaviour tickerProvider;
        
        [Header("Runtime (do not touch in Inspector)")]
        [SerializeField, HideInInspector] private StationSlot[] slots;
        
        // Propiedades de acceso
        public CookingMethod Method => method;
        public float SecondsPerTurn => secondsPerTurn;
        public int MaxTurnsBeforeBurn => maxTurnsBeforeBurn;
        public int SlotCount => slotCount;
        
        /// <summary>
        /// Attempts to insert an item into the given slot and start (or resume) a timed cooking session.
        /// Returns true if the insert succeeds.
        /// </summary>
        public bool TryInsert(int slotIndex, ref ItemAmount item) { return false; }
        
        /// <summary>
        /// Attempts to remove the item from the given slot, writing back its PreparationState.
        /// Returns true if the removal succeeds and outputs the item.
        /// </summary>
        public bool TryRemove(int slotIndex, out ItemAmount item) { item = default; return false; }
        
        /// <summary>
        /// Read-only view of the current preparation state for UI bars/indicators without removing the item.
        /// If no item is present, this should reflect an empty/None state.
        /// </summary>
        public PreparationState PeekState(int slotIndex) { return default; }
    }
}
