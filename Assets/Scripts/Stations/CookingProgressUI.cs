using UnityEngine;
using UnityEngine.UI;

namespace Stations
{
    public class CookingProgressUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform bar;
        [SerializeField] private RectTransform slotIcon; 
        
        [Header("Progresion parameters")]
        [SerializeField] private float leftPadding  = 0f;
        [SerializeField] private float rightPadding = 0f;
        [SerializeField] private float startYPos  = 0f;
        [SerializeField] private float endYPos  = 0f;
        [SerializeField] private float endScale = 1f;

        private TimedStation _station;
        
        public TimedStation station { get => _station; set => _station = value; }
        
        private Vector2 _startLocal;
        private Vector2 _endLocal;

        private void Awake()
        {
            if (bar)
            {
                float half = bar.rect.width * 0.5f;
                _startLocal = new Vector2(-half, 0f);
                _endLocal   = new Vector2(+half,  0f);
            }
            
            RecomputeEndpoints();
        }

        private void OnEnable()
        {
            if (station != null) station.OnSessionStarted += OnSessionStarted;
            if (station != null) station.OnSessionStopped += OnSessionStopped;
        }
        
        private void OnDisable()
        {
            if (station != null) station.OnSessionStarted -= OnSessionStarted;
            if (station != null) station.OnSessionStopped -= OnSessionStopped;
        }

        private void Update()
        {
            if (station == null || bar == null || slotIcon == null) return;
            
            float maxSeconds =
                (station.MaxTurnsBeforeBurn + 1) * station.SecondsPerTurn; // Deberia ser ese +1?
            
            float tSeconds = station.IsCooking ? station.AccumulatedSeconds 
                                               : station.SavedTurnsCooked * station.SecondsPerTurn;
            
            tSeconds = Mathf.Min(tSeconds, maxSeconds);
            float norm = maxSeconds <= 0f ? 0f : tSeconds / maxSeconds;
            
            slotIcon.anchoredPosition = Vector2.Lerp(_startLocal, _endLocal, norm);
            
            float currentScale = Mathf.Lerp(1f, endScale, norm);
            slotIcon.localScale = Vector3.one * currentScale;
        }

        private void OnRectTransformDimensionsChange()
        {
            RecomputeEndpoints();
        }

        private void RecomputeEndpoints()
        {
            if (bar == null) return;

            float half = bar.rect.width * 0.5f;
            
            float totalPad = leftPadding + rightPadding;
            float usable = Mathf.Max(1f, bar.rect.width - totalPad);
            
            float startX = -half + leftPadding;
            float endX =  half - rightPadding;
            
            if (endX <= startX)
            {
                float mid = 0f;
                float halfUsable = usable * 0.5f;
                startX = -halfUsable;
                endX   =  halfUsable;
            }
            
            _startLocal = new Vector2(startX, startYPos);
            _endLocal   = new Vector2(endX, endYPos);
        }

        private void OnSessionStarted(Cooking.CookingSession s)
        {
             // Aca podemos aplicar las anims
        }

        private void OnSessionStopped()
        {
            // Si llegamos a querer aplicar algo cuando lo pausamos (podriamos apagar las anims)
        }

        private void SnapToSaved()
        {
            Update();
        }
    }
}
