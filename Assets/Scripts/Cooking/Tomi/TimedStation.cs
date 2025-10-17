using Stations;
using Cooking;
using Items.Core;
using Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking.Tomi
{
    public class TimedStation : Station
    {
        
        [SerializeField] private InvSystem invSystem;
        
        [Header("Cooking Config")]
        [SerializeField] private CookingMethod method = CookingMethod.Boil;
        [SerializeField] private float secondsPerTurn = 7f;
        [SerializeField] private int maxTurnsBeforeBurn = 4;

        [Header("World-Time Ticker (provide a component implementing ICookingTicker)")]
        [SerializeField] private MonoBehaviour tickerProvider;
        
        private ICookingTicker _ticker;
        
        private CookingSession _session;
        
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
        }
        protected override void LeaveStation()
        {
            var button = CanvasInstance.GetComponentInChildren<Button>();
            if (button) button.onClick.RemoveListener(OnStationButtonPressed);
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
            
            _session.OnDonenessCrossed += HandleBoundary;
            _session.OnBurnt           += HandleBurnt;
            _ticker.Register(_session);
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
                    var ps = item.Prep;
                    float turns = s.TurnsCooked;
                    if (turns > ps.turnsCooked)
                        ps.turnsCooked = turns;
                    ps.method = method;
                    item.Prep = ps;
                    invSystem.Items[0].SetItem(item);
                    invSystem.NotifySlotChanged(0);
                }
            }
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
            
             invSystem.Items[0].SetItem(item);
             invSystem.NotifySlotChanged(0);
             
             StopCooking();
            
            Debug.Log($"[{name}] BURNT (Method={method})"); // BORRAR despues
        }
    }
}
