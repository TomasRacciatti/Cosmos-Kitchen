using Managers;
using UnityEngine;

namespace MiniGames
{
    public class SkillCheckMiniGame : MiniGame
    {
        [Header("SkillCheck Settings")]
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float successThreshold = 10f;
        [SerializeField] private float safeThreshold = 50f;
        [SerializeField] private int skillChecks = 10;
        [SerializeField] private float maxDeltaX = 600f;
        [SerializeField] private float maxDeltaY = 200f;
        [SerializeField] private float minSize = 0.5f;
        [SerializeField] private float maxSize = 1.3f;
        
        private int _currentSkillChecks;
        private float _currentZoneAngle;
        private float _currentNeedleAngle;
        private bool _positiveRotation;
        private SkillCheckUI _skillCheckUI;
        
        protected override void EnterMiniGame()
        {
            base.EnterMiniGame();

            if (_skillCheckUI == null) _skillCheckUI = GameManager.Canvas.MiniGamesUI.SkillCheckUI;
            _skillCheckUI.gameObject.SetActive(true);
            _currentZoneAngle = 0;
            _currentNeedleAngle = 0;
            _currentSkillChecks = 0;
            _skillCheckUI.SetSkillCheck(_currentZoneAngle, _currentNeedleAngle, new Vector2(0, 0), 1);
            _skillCheckUI.SetLives(Lives);
            _skillCheckUI.SetRemainingSkillChecks(skillChecks - _currentSkillChecks);
        }

        protected override void LeaveMiniGame()
        {
            base.LeaveMiniGame();
            _skillCheckUI.gameObject.SetActive(false);
        }

        private void SetSkillCheck()
        {
            _currentZoneAngle = Random.Range(0f, 360f);
            _positiveRotation = Random.value > 0.5f;
            
            _currentNeedleAngle = Random.Range(_currentZoneAngle + safeThreshold, _currentZoneAngle - safeThreshold + 360f);
            _currentNeedleAngle = NormalizeAngle(_currentNeedleAngle);
            
            float deltaX = Random.Range(-maxDeltaX, maxDeltaX);
            float deltaY = Random.Range(-maxDeltaY, maxDeltaY);
            float size = Random.Range(minSize, maxSize);
            
            _skillCheckUI.SetSkillCheck(_currentZoneAngle, _currentNeedleAngle, new Vector2(deltaX, deltaY), size);
        }

        private void Update()
        {
            if (!IsActive) return;

            if (_currentSkillChecks != 0) RotateNeedle();
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryCutting();
            }
        }

        private void TryCutting()
        {
            if (Mathf.Abs(Mathf.DeltaAngle(_currentZoneAngle, _currentNeedleAngle)) <= successThreshold)
            {
                _currentSkillChecks++;
                _skillCheckUI.SetRemainingSkillChecks(skillChecks - _currentSkillChecks);
                if (_currentSkillChecks >= skillChecks)
                {
                    WinMiniGame();
                    LeaveMiniGame();
                    return;
                }
                SetSkillCheck();
                _audioSource?.PlayOneShot(correctSound);
            }
            else
            {
                Lives--;
                _skillCheckUI.SetLives(Lives);
                if (Lives <= 0)
                {
                    LoseMiniGame();
                    LeaveMiniGame();
                    return;
                }
                SetSkillCheck();
                _audioSource?.PlayOneShot(wrongSound);
            }
        }

        private void RotateNeedle()
        {
            var dir = _positiveRotation ? 1f : -1f;
            _currentNeedleAngle += dir * rotationSpeed * Time.deltaTime * (difficulty + scaleDifficulty * _currentSkillChecks + (3 - Lives) * 0.5f);
            _currentNeedleAngle = NormalizeAngle(_currentNeedleAngle);
            _skillCheckUI.SetNeedle(_currentNeedleAngle);
        }
        
        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }
    }
}