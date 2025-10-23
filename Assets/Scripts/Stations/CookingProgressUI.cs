using UnityEngine;
using UnityEngine.UI;

namespace Stations
{
    public class CookingProgressUI : MonoBehaviour
    {
        [SerializeField] private RectTransform bar;
        [SerializeField] private RectTransform slotIcon; 

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
        }

        private void OnStartStopClicked()
        {
            // Esto se maneja en TimedStation pero que exista aca nos asegura que no se suscriba dos veces
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
