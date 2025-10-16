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
    public class TimedStation : Station
    {
        [Header("Cooking Config")]
        [SerializeField] private CookingMethod method = CookingMethod.Boil;
        [SerializeField] private float secondsPerTurn = 7f;
        [SerializeField] private int maxTurnsBeforeBurn = 4;
        [SerializeField] private int slotCount = 1;

        [Header("World-Time Ticker (provide a component implementing ICookingTicker)")]
        [SerializeField] private MonoBehaviour tickerProvider;
        
        private StationSlot[] _slots;
        
        private ICookingTicker _ticker;
        
        private System.Action<int> _boundaryHandler0;
        private System.Action _burntHandler0;
        
        // Propiedades de acceso
        public CookingMethod Method => method;
        public float SecondsPerTurn => secondsPerTurn;
        public int MaxTurnsBeforeBurn => maxTurnsBeforeBurn;
        public int SlotCount => slotCount;

        protected override void Awake()
        {
            base.Awake();
            
            slotCount = Mathf.Max(1, slotCount);
            _slots = new StationSlot[slotCount];
            for (int i = 0; i < _slots.Length; i++) 
                _slots[i] = new StationSlot();
        }

        private void OnEnable()
        {
            if (tickerProvider != null)
                _ticker = tickerProvider as ICookingTicker;

            if (_ticker == null) // Voy a ponerlo manualmente idealmente asi que no deberia entrar aca, pero por las dudas lo dejo
            {
                var found = FindFirstObjectByType<CookingTicker>();
                if (found != null) _ticker = found as ICookingTicker;
            }

            if (_ticker == null) // Si entra aca es que no consiguio uno arriba
            {
                Debug.LogError($"{name}: Falta un CookingTicker en la escena.");
                enabled = false;
                return;
            }
        }

        private void OnDisable()
        {
            var slot = _slots[0];
            if (slot.session != null)
            {
                _ticker.Unregister(slot.session);
                UnsubscribeSessionEvents(0, slot.session);
                slot.session = null;
            }
            slot.occupied = false;
            _slots[0] = slot;
        }

        public bool TryInsert(int slotIndex, ref ItemAmount item)
        {
            // MULTI-SLOT: si hacemos multi-slot aca agregariamos el slotIndex real.
            slotIndex = 0;
            
            var slot = _slots[slotIndex];
            if (slot.occupied) return false;
            if (item.IsEmpty)  return false;
            
            var prepState = item.Prep;
            if (prepState.method != method)
            {
                prepState.method = method;
                prepState.turnsCooked = 0f;
            }
            item.Prep = prepState;
            
            slot.item = item;
            slot.occupied = true;

            float spt = Mathf.Max(0.01f, secondsPerTurn);
            slot.session = new CookingSession
            {
                method = method,
                inventoryIndex = slotIndex,
                secondsPerTurn = spt,
                maxTurnsBeforeBurn = Mathf.Max(1, maxTurnsBeforeBurn),
                isActive = true,
                
                accumulatedSeconds = prepState.turnsCooked * spt
            };

            SubscribeSessionEvents(slotIndex, slot.session);
            _ticker.Register(slot.session);
            
            item.Clear();

            _slots[slotIndex] = slot;
            return true;
        }
        
        public bool TryRemove(int slotIndex, out ItemAmount item)
        {
            slotIndex = 0;
            
            var slot = _slots[slotIndex];
            item = default;

            if (!slot.occupied) return false;
            
            var prepState = slot.item.Prep;
            
            if (slot.session != null)
            {
                _ticker.Unregister(slot.session);

                if (slot.session.secondsPerTurn > 0f)
                {
                    float turns = slot.session.TurnsCooked;
                    if (turns > prepState.turnsCooked)
                        prepState.turnsCooked = turns;
                }

                UnsubscribeSessionEvents(slotIndex, slot.session);
                slot.session = null;
            }
            
            if (prepState.method != method)
                prepState.method = method;
            
            slot.item.Prep = prepState;

            // Devuelve el item
            item = slot.item;

            // Limpia el slot
            slot.item = default;
            slot.occupied = false;

            _slots[slotIndex] = slot;
            return true;
        }
        
        public PreparationState PeekState(int slotIndex)
        {
            slotIndex = 0;

            var slot = _slots[slotIndex];
            if (!slot.occupied) return default;

            var prepState = slot.item.Prep;
            if (slot.session != null && slot.session.secondsPerTurn > 0f)
            {
                prepState.method = method;
                prepState.turnsCooked = slot.session.TurnsCooked;
            }
            return prepState;
        }

        private void SubscribeSessionEvents(int slotIndex, CookingSession s)
        {
            if (slotIndex == 0)
            {
                _boundaryHandler0 = (boundary) => HandleBoundary(slotIndex, boundary);
                _burntHandler0    = () => HandleBurnt(slotIndex);

                s.OnDonenessCrossed += _boundaryHandler0;
                s.OnBurnt           += _burntHandler0;
            }
            else
            {
                // MULTI-SLOT
            }
        }

        private void UnsubscribeSessionEvents(int slotIndex, CookingSession s)
        {
            if (s == null) return;

            if (slotIndex == 0)
            {
                if (_boundaryHandler0 != null) s.OnDonenessCrossed -= _boundaryHandler0;
                if (_burntHandler0    != null) s.OnBurnt           -= _burntHandler0;

                _boundaryHandler0 = null;
                _burntHandler0    = null;
            }
            else
            {
                // MULTI-SLOT
            }
        }

        private void HandleBoundary(int slotIndex, int boundaryIndex)
        {
            var slot = _slots[slotIndex];
            if (!slot.occupied) return;

            var prepState = slot.item.Prep;
            prepState.method = method;
            prepState.turnsCooked = boundaryIndex;

            slot.item.Prep = prepState;
            _slots[slotIndex] = slot;
            
            Debug.Log($"[{name}] Turn crossed: {boundaryIndex}  (Method={method})"); // BORRAR despues
        }
        
        private void HandleBurnt(int slotIndex)
        {
            var slot = _slots[slotIndex];
            if (!slot.occupied) return;

            var prepState = slot.item.Prep;
            prepState.method = method;
            prepState.turnsCooked = maxTurnsBeforeBurn + 1f;

            // Si se quemo dejamos de tickear
            if (slot.session != null)
            {
                _ticker.Unregister(slot.session);
                UnsubscribeSessionEvents(slotIndex, slot.session);
                slot.session = null;
            }

            slot.item.Prep = prepState;
            _slots[slotIndex] = slot;
            
            Debug.Log($"[{name}] BURNT (Method={method})"); // BORRAR despues
        }
    }
}
