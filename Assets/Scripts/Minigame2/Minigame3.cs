using System;
using Regulators;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minigame2
{
    public abstract class Minigame3 : MonoBehaviour
    {
        [Header("Minigame Settings")]
        [SerializeField] private int totalLives = 3;
        [SerializeField] private int totalActions = 10;
        [SerializeField] private float difficulty = 1f;
        [SerializeField] private float scaleDifficulty = 0.1f;
        
        [SerializeField] private AudioClip startSound;
        [SerializeField] private AudioClip correctSound;
        [SerializeField] private AudioClip wrongSound;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;
        
        public event Action<int> OnWin;
        public event Action OnLose;
        protected abstract bool IsActionCorrect();
        protected int CurrentLives { get; private set; }
        private int TotalLives => totalLives;
        protected float ProgressLives => (totalLives - (float)CurrentLives) / TotalLives;
        protected int CurrentActions { get; private set; }
        private int TotalActions => totalActions;
        protected float ProgressActions => (float)CurrentActions / TotalActions;
        
        protected float ProgressDifficulty => difficulty + scaleDifficulty * (ProgressActions + 1) * (4 * ProgressLives + 1);

        public virtual void StartMinigame()
        {
            AudioManager.instance.PlaySFX(startSound);
            CurrentLives = totalLives;
            CurrentActions = 0;
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
    }
}