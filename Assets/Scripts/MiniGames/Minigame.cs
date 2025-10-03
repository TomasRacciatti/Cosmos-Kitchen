using System;
using UI.Components;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MiniGames
{
    public abstract class Minigame : MonoBehaviour
    {
        [Header("Minigame Settings")]
        [SerializeField] private int totalLives = 3;
        [SerializeField] private int totalActions = 10;
        [SerializeField] private float difficulty = 1f;
        [SerializeField] private float scaleDifficulty = 0.1f;
        
        [SerializeField] private AudioClip startSound;
        [SerializeField] protected AudioClip correctSound;
        [SerializeField] private AudioClip wrongSound;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;
        [FormerlySerializedAs("linearBar")] [SerializeField] protected ProgressBar progressBar;
        [SerializeField] private LifeBar lifeBar;
        
        public event Action<int> OnWin;
        public event Action OnLose;
        public event Action OnCorrect;
        public event Action OnWrong;
        
        protected abstract bool IsActionCorrect();
        protected int CurrentLives { get; private set; }
        private int TotalLives => totalLives;
        protected float ProgressLives => (totalLives - (float)CurrentLives) / TotalLives;
        protected int CurrentActions { get; private set; }
        protected int TotalActions => totalActions;
        protected float ProgressActions => (float)CurrentActions / TotalActions;
        
        protected float ProgressDifficulty => difficulty + scaleDifficulty * (4 * ProgressActions) * (2 * ProgressLives + 1);

        public virtual void StartMinigame()
        {
            AudioManager.instance.PlaySFX(startSound);
            CurrentLives = totalLives;
            CurrentActions = 0;
            SetProgress();
            SetLives();
        }
        
        public virtual void ExitMinigame()
        {
            
        }
        
        protected void EvaluateAction()
        {
            if (IsActionCorrect()) Correct();
            else Wrong();
        }
        
        protected virtual void Correct()
        {
            CurrentActions++;
            if (CurrentActions < totalActions)
            {
                AudioManager.instance.PlaySFX(correctSound);
                OnCorrect?.Invoke();
                return;
            }
            Win();
            ExitMinigame();
        }
        
        protected virtual void Wrong()
        {
            CurrentLives--;
            if (CurrentLives > 0)
            {
                AudioManager.instance.PlaySFX(wrongSound);
                OnWrong?.Invoke();
                SetLives();
                return;
            }
            Lose();
            ExitMinigame();
        }

        protected void Win()
        {
            AudioManager.instance.PlaySFX(winSound);
            OnWin?.Invoke(CurrentLives);
        }
        
        protected void Lose()
        {
            AudioManager.instance.PlaySFX(loseSound);
            OnLose?.Invoke();
        }

        protected virtual void SetProgress()
        {
            if (progressBar) progressBar.SetProgress(CurrentActions, totalActions);
        }
        
        private void SetLives()
        {
            lifeBar.SetLives(CurrentLives);
        }

        protected void ResetProgress()
        {
            CurrentActions = 0;
            SetProgress();
        }
    }
}