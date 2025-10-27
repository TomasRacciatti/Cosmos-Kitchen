using Cooking;
using Items.Core;
using Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Stations
{
    public class TimedStation : Station
    {
        
        [SerializeField] private InvSystem invSystem;
        
        [Header("Cooking Config")]
        [SerializeField] private CookingMethod method = CookingMethod.Boil;
        [SerializeField] private float secondsPerTurn = 7f;
        private readonly int maxTurnsBeforeBurn = 3;

        [Header("World-Time Ticker (provide a component implementing ICookingTicker)")]
        [SerializeField] private MonoBehaviour tickerProvider;
        
        private ICookingTicker _ticker;
        
        private CookingSession _session;
        private bool _recordedThisSession = false;
        
        // UI
        public event System.Action<CookingSession> OnSessionStarted;
        public event System.Action OnSessionStopped;
        
        public bool  IsCooking => _session != null && _session.isActive;
        public float AccumulatedSeconds => _session != null ? _session.accumulatedSeconds : 0f;

        public float SavedTurnsCooked
        {
            get
            {
                if (invSystem == null) return 0f;
                var item = invSystem.Item(0);
                return item.IsEmpty ? 0f : item.Prep.turnsCooked;
            }
        }
        
        // Propiedades de acceso
        public CookingMethod Method => method;
        public float SecondsPerTurn => secondsPerTurn;
        public int MaxTurnsBeforeBurn => maxTurnsBeforeBurn;

        protected override void Awake()
        {
            base.Awake();
            
            if (!invSystem) invSystem = GetComponent<InvSystem>();
        }

        private void OnEnable()
        {
            if (tickerProvider != null)
                _ticker = tickerProvider as ICookingTicker;

            if (_ticker == null) // Si no entra de forma manual, se settea solo
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
            
            if (invSystem != null) invSystem.Subscribe(OnInventorySlotChanged);
        }

        private void OnDisable()
        {
            if (invSystem != null) invSystem.Unsubscribe(OnInventorySlotChanged);
            StopCooking();
        }

        protected override void EnterStation()
        {
            base.EnterStation();

            var invView = CanvasInstance.GetComponentInChildren<InvView>();
            if (invView) invView.SetInventory(invSystem);

            var button = CanvasInstance.GetComponentInChildren<Button>();
            if (button) button.onClick.AddListener(OnStationButtonPressed);
            
            // UI
            var progress = CanvasInstance.GetComponentInChildren<CookingProgressUI>(true);
            if (progress) progress.station = this;
        }
        protected override void LeaveStation()
        {
            var button = CanvasInstance.GetComponentInChildren<Button>();
            if (button) button.onClick.RemoveListener(OnStationButtonPressed);
            
            // UI
            var progress = CanvasInstance ? CanvasInstance.GetComponentInChildren<CookingProgressUI>(true) : null;
            if (progress && ReferenceEquals(progress.station, this)) progress.station = null;
            
            base.LeaveStation();
        }
        
        private void OnStationButtonPressed()
        {
            if (_session == null) StartCooking();
            else StopCooking();
        }

        private void StartCooking()
        {
            if (_ticker == null || invSystem == null) return;

            var item = invSystem.Item(0);
            if (item.IsEmpty) return;
            
            if (item.Prep.Doneness == Doneness.Burnt)
            {
                // No permitimos cocinar ingredientes arruinados
                return;
            }
            
            var prepState = item.Prep;
            if (prepState.method != method)
            {
                prepState.method = method;
                prepState.turnsCooked = 0f;
                item.Prep = prepState;
                invSystem.Items[0].SetItem(item);
                invSystem.NotifySlotChanged(0);
            }
            
            float spt = Mathf.Max(0.01f, secondsPerTurn);
            _session = new CookingSession
            {
                method = method,
                inventoryIndex = 0,
                secondsPerTurn = spt,
                maxTurnsBeforeBurn = Mathf.Max(1, maxTurnsBeforeBurn),
                isActive = true,
                accumulatedSeconds = item.Prep.turnsCooked * spt
            };
            
            _recordedThisSession = false;
            
            _session.OnDonenessCrossed += HandleBoundary;
            _session.OnBurnt           += HandleBurnt;
            _ticker.Register(_session);
            
            OnSessionStarted?.Invoke(_session);
        }
        
        private void StopCooking()
        {
            if (_session == null) return;
            
            var s = _session;
            _session = null;

            if (_ticker != null) _ticker.Unregister(s);
            s.OnDonenessCrossed -= HandleBoundary;
            s.OnBurnt           -= HandleBurnt;
            
            if (invSystem != null)
            {
                var item = invSystem.Item(0);
                if (!item.IsEmpty && s.secondsPerTurn > 0f)
                {
                    var prepState = item.Prep;
                    int finishedTurns = Mathf.Clamp(Mathf.FloorToInt(s.TurnsCooked), 0 ,3);
                    if (s.TurnsCooked > prepState.turnsCooked)
                        prepState.turnsCooked = s.TurnsCooked;
                    
                    prepState.method = method;
                    item.Prep = prepState;
                    
                    if (!_recordedThisSession)
                    {
                        item.AddProcessStep(method, finishedTurns);
                        _recordedThisSession = true;
                    }
                    
                    invSystem.Items[0].SetItem(item);
                    invSystem.NotifySlotChanged(0);
                }
            }
            
            OnSessionStopped?.Invoke();
        }

        private void OnInventorySlotChanged(int index, Items.Core.ItemAmount current)
        {
            if (index != 0) return;
            bool wasRunning = _session != null;
            StopCooking();
            if (wasRunning && !current.IsEmpty) StartCooking();
        }

        private void HandleBoundary(int boundaryIndex)
        {
            var item = invSystem.Item(0);
            if (item.IsEmpty)
            {
                StopCooking();
                return;
            }

            var ps = item.Prep;
            ps.method = method;
            ps.turnsCooked = boundaryIndex;
            item.Prep = ps;

            invSystem.Items[0].SetItem(item);
            invSystem.NotifySlotChanged(0);

            Debug.Log($"[{name}] Turn crossed: {boundaryIndex}  (Method={method})"); // BORRAR despues
        }
        
        private void HandleBurnt()
        {
            var item = invSystem.Item(0);
            if (item.IsEmpty)
            {
                StopCooking();
                return;
            }

             var prepState = item.Prep;
             prepState.method = method;
             prepState.turnsCooked = maxTurnsBeforeBurn + 1f;
             item.Prep = prepState;
             
             if (!_recordedThisSession)
             {
                 item.AddProcessStep(method, 4);
                 _recordedThisSession = true;
             }
            
             invSystem.Items[0].SetItem(item);
             invSystem.NotifySlotChanged(0);
             
             StopCooking();
            
            Debug.Log($"[{name}] BURNT (Method={method})"); // BORRAR despues
        }
    }
}
