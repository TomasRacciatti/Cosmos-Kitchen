using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MiniGames.Milking
{
    public class MilkingMinigame : CanvasMinigame
    {
        private enum Expected { None, A, D }
        
        [Header("Timer ")]
        [SerializeField] private float totalTime  = 20f;
        
        [Header("Milk Bar")]
        [SerializeField] private Image milkBarFill;
        
        [Header("Animators")]
        [SerializeField] private Animator sideL;
        [SerializeField] private Animator sideR;
        [SerializeField] private string squishParam = "Squish";
        
        [Header("Keys")]
        [SerializeField] private CanvasGroup keysGroup;
        [SerializeField] private Image keyA;
        [SerializeField] private Image keyD;
        
        [Header("Rhythm")]
        [SerializeField, Min(0f)] private float minGapBetweenPrompts = 0.1f;
        [SerializeField, Min(0f)] private float maxGapBetweenPrompts = 0.3f;
        [SerializeField, Min(0f)] private float perfectWindow = 0.15f;
        [SerializeField, Min(0f)] private float goodWindow = 0.40f;
        [SerializeField, Min(0f)] private float missHold = 0.05f;
        [SerializeField, Min(0f)] private float fadeOutSeconds = 0.1f;
        
        [Header("Game Settings")]
        [SerializeField, Min(0f)] private float baseGain = 0.18f;
        [SerializeField, Min(0f)] private float minGain = 0.02f;
        [SerializeField, Min(0f)] private float perfectMultiplier = 1.5f;
        [SerializeField, Min(0.5f)] private float resistance = 1.6f;
        [SerializeField, Min(0f)] private float baseLoss = 0.20f;
        [SerializeField, Min(0f)] private float decayPerSecond = 0.12f;
        
        [Header("Colors")]
        [SerializeField] private Color colorDefault = Color.white;
        [SerializeField] private Color colorMiss = new Color(1f, 0f, 0f);
        [SerializeField] private Color colorOk = new Color(1f, 0.843f, 0f);
        [SerializeField] private Color colorPerfect= new Color(0f, 1f, 0f);
        
        private float _progress;
        private float _timeRemaining;
        private bool _ended;
        
        private Expected _expected = Expected.None;
        private Expected _lastPressed  = Expected.None;
        private Expected _currentPrompt = Expected.None;

        private Action _tickPhase;
        private float _phaseTimer;
        private float _promptElapsed;
        private float _fadeClock;


        public override void StartMinigame()
        {
            base.StartMinigame();
            
            _ended = false;
            _progress = 0;
            _timeRemaining = Mathf.Max(0.1f, totalTime);
            
            SetSquish(false, false);
            SetKeysVisible(false, 0f);
            TintKeys(colorDefault, colorDefault);
            
            _expected = Expected.None;
            _currentPrompt = Expected.None;
            _lastPressed = Expected.None;
            
            EnterWaitingPhase();
            
            UpdateVisuals();
        }

        protected override void Update()
        {
            base.Update();
            if (_ended) return;
            
            // Timer
            _timeRemaining  -= Time.deltaTime;
            if (_timeRemaining  < 0 && _progress < 1f)
            {
                Lose();
                _ended = true;
                return;
            }
            
            // Decay passivo
            if (_progress > 0f)
                _progress = Mathf.Max(0f, _progress - decayPerSecond * Time.deltaTime);
            

            _tickPhase?.Invoke();
            UpdateVisuals();
        }
        
        protected override bool IsActionCorrect()
        {
            return _lastPressed == _currentPrompt;
        }

        protected override void Correct()
        {
            AudioManager.instance.PlaySFX(correctSound);
            float multiplier;
            
            if (_promptElapsed <= perfectWindow)
            {
                multiplier = perfectMultiplier;
                TintKeys(_currentPrompt == Expected.A ? colorPerfect : colorDefault,
                         _currentPrompt == Expected.D ? colorPerfect : colorDefault);
            }
            else
            {
                multiplier = 1f;
                TintKeys(_currentPrompt == Expected.A ? colorOk : colorDefault,
                         _currentPrompt == Expected.D ? colorOk : colorDefault);
            }

            float resistedGain = baseGain * Mathf.Pow(1f - Mathf.Clamp01(_progress), resistance);
            float gain = Mathf.Max(minGain, resistedGain);

            _progress += gain * multiplier;
            
            // Anim
            if (_currentPrompt == Expected.A) 
                SetSquish(true, false);
            else
                SetSquish(false, true);
            
            _expected = (_currentPrompt == Expected.A) ? Expected.D : Expected.A;
            
            // Win
            if (_progress >= 1f && !_ended)
            {
                Win();
                _ended = true;
                return;
            }

            EnterResolvingPhase();
        }

        protected override void Wrong()
        {
            _progress = Mathf.Max(0f, _progress - baseLoss);
            
            ResolveMiss();

            base.Wrong();
        }
        
        #region Strategy - Lo que hago porque no me dejas usar un switch
        
        private void EnterWaitingPhase()
        {
            _phaseTimer = Random.Range(minGapBetweenPrompts, maxGapBetweenPrompts);
            _tickPhase = TickWaiting;
        }

        private void TickWaiting()
        {
            _phaseTimer -= Time.deltaTime;
            
            // Tecla prematura
            bool aDown = Input.GetKeyDown(KeyCode.A);
            bool dDown = Input.GetKeyDown(KeyCode.D);
            if (aDown || dDown)
            {
                _lastPressed = aDown ? Expected.A : Expected.D;
                Wrong();
                return;
            }
            
            if (_phaseTimer <= 0f)
                EnterShowingPhase();
        }

        private void EnterShowingPhase()
        {
            _currentPrompt = (_expected == Expected.None)
                ? (Random.value < 0.5f ? Expected.A : Expected.D)
                : _expected;

            TintKeys(colorDefault, colorDefault);
            keyA.gameObject.SetActive(_currentPrompt == Expected.A);
            keyD.gameObject.SetActive(_currentPrompt == Expected.D);
            SetKeysVisible(true, 1f);

            _promptElapsed = 0f;
            _tickPhase = TickShowing;
        }

        private void TickShowing()
        {
            _promptElapsed += Time.deltaTime;

            bool aDown = Input.GetKeyDown(KeyCode.A);
            bool dDown = Input.GetKeyDown(KeyCode.D);

            if (aDown || dDown)
            {
                _lastPressed = aDown ? Expected.A : Expected.D;
                EvaluateAction();
                return;
            }
            
            if (_promptElapsed > goodWindow) // Se le acabo el tiempo
                ResolveMiss();
        }
        
        private void EnterResolvingPhase()
        {
            _phaseTimer = missHold + fadeOutSeconds;
            _fadeClock = fadeOutSeconds;
            _tickPhase = TickResolving;
        }

        private void TickResolving()
        {
            _phaseTimer -= Time.deltaTime;
            if (_phaseTimer <= 0f)
                EnterWaitingPhase();
        }
        
        #endregion
        
        
        #region Helpers
        private void ResolveMiss()
        {
            if (_currentPrompt == Expected.A) 
                TintKeys(colorMiss, colorDefault);
            else                              
                TintKeys(colorDefault, colorMiss);
            
            EnterResolvingPhase();
        }

        private void UpdateVisuals()
        {
            if (milkBarFill) milkBarFill.fillAmount = Mathf.Clamp01(_progress);
            if (progressBar) progressBar.SetProgress(Mathf.Max(0f, _timeRemaining), totalTime);
            
            if (keysGroup && _fadeClock > 0f)
            {
                _fadeClock -= Time.deltaTime;
                float fade = Mathf.Clamp01(1f - (_fadeClock / fadeOutSeconds));
                keysGroup.alpha = 1f - fade;
                if (_fadeClock <= 0f)
                {
                    SetKeysVisible(false, 0f);
                    keyA.gameObject.SetActive(false);
                    keyD.gameObject.SetActive(false);
                }
            }
        }
        
        private void SetSquish(bool left, bool right)
        {
            if (sideL) sideL.SetBool(squishParam, left);
            if (sideR) sideR.SetBool(squishParam, right);
        }

        private void SetKeysVisible(bool visible, float alphaIfVisible = 1f)
        {
            if (!keysGroup) return;
            keysGroup.alpha = visible ? alphaIfVisible : 0f;
            keysGroup.interactable = visible;
            keysGroup.blocksRaycasts = visible;
        }

        private void TintKeys(Color colorA, Color colorD)
        {
            if (keyA) keyA.color = colorA;
            if (keyD) keyD.color = colorD;
        }
        
        #endregion
    }
}
