using TMPro;
using UIScripts.HUD;
using UnityEngine;

namespace MiniGames
{
    public class SkillCheckMinigame : CanvasMinigame
    {
        [Header("SkillCheck Settings")]
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float successThreshold = 20f;
        [SerializeField] private float safeThreshold = 90f;
        [SerializeField] private float maxDeltaX = 600f;
        [SerializeField] private float maxDeltaY = 200f;
        [SerializeField] private float minSize = 0.5f;
        [SerializeField] private float maxSize = 1.3f;
        
        [Header("References")]
        [SerializeField] private RectTransform canvasTransform;
        [SerializeField] private RectTransform zoneTransform;
        [SerializeField] private RectTransform needleTransform;
        [SerializeField] private TextMeshProUGUI livesText;
        [SerializeField] private TextMeshProUGUI skillCheckText;
        [SerializeField] private LifeBar lifeBar;
        
        private float _currentZoneAngle;
        private float _currentNeedleAngle;
        private bool _positiveRotation;
        
        public override void StartMinigame()
        {
            base.StartMinigame();
            _currentZoneAngle = 0;
            _currentNeedleAngle = 0;
            SetLives();
            SetSkillCheck(_currentZoneAngle, _currentNeedleAngle, Vector2.zero, 1);
        }
        
        protected override void Update()
        {
            base.Update();
            if (CurrentActions != 0) RotateNeedle();
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryCutting();
            }
        }
        
        private void SetRandomSkillCheck()
        {
            _currentZoneAngle = Random.Range(0f, 360f);
            _positiveRotation = Random.value > 0.5f;
            
            _currentNeedleAngle = Random.Range(_currentZoneAngle + safeThreshold, _currentZoneAngle - safeThreshold + 360f);
            _currentNeedleAngle = NormalizeAngle(_currentNeedleAngle);
            
            float deltaX = Random.Range(-maxDeltaX, maxDeltaX);
            float deltaY = Random.Range(-maxDeltaY, maxDeltaY);
            float size = Random.Range(minSize, maxSize);
            
            SetSkillCheck(_currentZoneAngle, _currentNeedleAngle, new Vector2(deltaX, deltaY), size);
        }
        
        private void SetSkillCheck(float zoneAngle, float needleAngle, Vector2 position, float size)
        {
            zoneTransform.localEulerAngles = new Vector3(0f, 0f, zoneAngle);
            needleTransform.localEulerAngles = new Vector3(0f, 0f, needleAngle);
            canvasTransform.localScale = new Vector2(size, size);
            canvasTransform.anchoredPosition = position;
        }
        
        protected override bool IsActionCorrect()
        {
            return Mathf.Abs(Mathf.DeltaAngle(_currentZoneAngle, _currentNeedleAngle)) <= successThreshold;
        }

        private void TryCutting()
        {
            EvaluateAction();
            SetRandomSkillCheck();
        }

        private void RotateNeedle()
        {
            var dir = _positiveRotation ? 1f : -1f;
            _currentNeedleAngle += dir * rotationSpeed * Time.deltaTime * ProgressDifficulty;
            _currentNeedleAngle = NormalizeAngle(_currentNeedleAngle);
            needleTransform.localEulerAngles = new Vector3(0f, 0f, _currentNeedleAngle);
        }
        
        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }

        protected override void Wrong()
        {
            base.Wrong();
            SetLives();
        }

        private void SetLives()
        {
            lifeBar.SetLives(CurrentLives);
        }
    }
}
