using UnityEngine;
using UnityEngine.UI;
using UI.Components;
using MiniGames;
using UnityEngine.Serialization;

namespace MiniGames.Milking
{
    public class MilkingMinigame : CanvasMinigame
    {
        private enum Expected { None, A, D }
        private enum Side { Left, Right }
        
        [Header("Timer ")]
        //[SerializeField] private ProgressBar radialTimer;
        [SerializeField] private float stageDuration = 10f;
        [SerializeField] private int stages = 3;
        
        [Header("Milk Bar")]
        [SerializeField] private Image milkBarFill;
        
        [Header("Animators")]
        [SerializeField] private Animator sideL;
        [SerializeField] private Animator sideR;
        [SerializeField] private string squishParam = "Squish";
        
        [Header("Keys")]
        [SerializeField] private GameObject keyA;
        [SerializeField] private GameObject keyD;
        
        [Header("Game Settings")]
        [SerializeField, Min(0f)] private float baseGain = 0.18f;
        [SerializeField, Min(0f)] private float minGain = 0.02f;
        [SerializeField, Min(0.5f)] private float resistance = 1.6f;
        [SerializeField, Min(0f)] private float baseLoss = 0.20f;
        [SerializeField, Min(0f)] private float decayPerSecond = 0.12f;
        
        private float _progress; // 1 = ganado
        private float _totalTime;
        private float _remainingTime;
        private Expected _expected = Expected.None;
        private Expected _pressed = Expected.None;
        private bool _ended;


        public override void StartMinigame()
        {
            base.StartMinigame();
            
            _ended = false;
            _pressed = 0;
            
            _totalTime = Mathf.Max(0.1f, stageDuration);
            _remainingTime = _totalTime;
            
            SetAnim(sideL, false);
            SetAnim(sideR, false);
            SetKeyHints(true, true);
            _expected = Expected.None;
            
            UpdateVisuals();
        }

        protected override void Update()
        {
            base.Update();
            if (_ended) return;
            
            _remainingTime -= Time.deltaTime;

            if (_remainingTime < 0 && _progress < 1f)
            {
                Lose();
                _ended = true;
                return;
            }
            
            // Decay passivo
            if (_progress > 0f)
                _progress = Mathf.Max(0f, _progress - decayPerSecond * Time.deltaTime);
            
            // Captura de input
            bool inputA = Input.GetKeyDown(KeyCode.A);
            bool inputD = Input.GetKeyDown(KeyCode.D);

            if (inputA || inputD)
            {
                _pressed = inputA ? Expected.A : Expected.D;
                EvaluateAction();
            }
            
            UpdateVisuals();
        }
        
        protected override bool IsActionCorrect()
        {
            if (_pressed == Expected.None) return false;

            if (_expected == Expected.None) return true; // Para el arranque A o D son validas

            return _pressed == _expected;
        }

        protected override void Correct()
        {
            _progress = Mathf.Min(1f, _progress + ComputeGain(_progress));

            ApplyAccepted(_pressed);

            if (_progress >= 1 && !_ended)
            {
                Win();
                _ended = true;
            }
        }

        protected override void Wrong()
        {
            _progress = Mathf.Max(0f, _progress - baseLoss);
            // Deberia poner el audio aca dado que no esta el base.Wrong()?
            // En ese caso tengo que hacer que la variable del audio sea protected
        }
        
        #region Helpers

        private void UpdateVisuals()
        {
            if (milkBarFill) milkBarFill.fillAmount = Mathf.Clamp01(_progress);
            if (progressBar) progressBar.SetProgress(Mathf.Max(0f, _remainingTime), _totalTime);
        }
        
        private void SetAnim(Animator anim, bool value)
        {
            if (anim) anim.SetBool(squishParam, value);
        }

        private void SetKeyHints(bool aActive, bool dActive)
        {
            if (keyA) keyA.SetActive(aActive);
            if (keyD) keyD.SetActive(dActive);
        }

        private float ComputeGain(float progress)
        {
            // Cuanto mas cerca del final, mas cuesta sumar
            float resist = baseGain * Mathf.Pow(1f - Mathf.Clamp01(progress), resistance);
            return Mathf.Max(minGain, resist);
        }

        private void ApplyAccepted(Expected pressed)
        {
            var next = (pressed == Expected.A) ? Expected.D : Expected.A;
            
            SetAnim(sideL, pressed == Expected.A);
            SetAnim(sideR, pressed == Expected.D);
            SetKeyHints(next == Expected.A, next == Expected.D);

            _expected = next;
        }
        
        #endregion
    }
}
