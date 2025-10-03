using Regulators;
using UnityEngine;

namespace MiniGames
{
    public class FishingMinigame2 : CanvasMinigame
    {
        [Header("References")]
        [SerializeField] private RectTransform fish;
        [SerializeField] private RectTransform tideArrow;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 350;
        [SerializeField] private float maxDistance = 300;

        [Header("Tide Settings")]
        [SerializeField] private float tideForce = 300;
        [SerializeField] private AnimationCurve tideCurve = AnimationCurve.Linear(0, 0.2f, 1, 1f);
        [SerializeField] private AnimationCurve intervalCurve = AnimationCurve.Linear(0, 1, 1, 0.2f);
        [SerializeField] private float tideInfluence = 0.5f;
        
        private Vector2 currentForceDirection;
        private RectTransform hudRect;
        private Vector2 velocity;
        private Cooldown tideCooldown;
        private float _interval;
        
        private void Awake()
        {
            hudRect = GetComponent<RectTransform>();
            OnCorrect += SetProgress;
        }
        
        public override void StartMinigame()
        {
            base.StartMinigame();
            _interval = 1;
            ResetFish();
            ResetTide();
        }

        protected override void Update()
        {
            base.Update();
            FishMovement();
            Tide();
            CheckFishDistance();
        }
        
        private void CheckFishDistance()
        {
            if (IsActionCorrect()) return;
            ResetFish();
            ResetTide();
            Wrong();
        }
        
        private void FishMovement()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            Vector2 movement = input * (moveSpeed * Time.deltaTime);
            fish.anchoredPosition += movement;

            if (movement == Vector2.zero) return;
            velocity = movement;
            float targetAngle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            fish.rotation = Quaternion.Slerp(fish.rotation, Quaternion.Euler(0, 0, targetAngle), 10f * Time.deltaTime);
        }
        
        private void ResetFish()
        {
            fish.anchoredPosition = hudRect.rect.center;
            fish.rotation = Quaternion.identity;

            currentForceDirection = Random.insideUnitCircle.normalized;
            tideCooldown.ResetCooldown();
            velocity = Vector2.zero;

            ResetProgress();
        }
        
        private void Tide()
        {
            if (tideCooldown.IsReady)
            {
                ResetTide();
                EvaluateAction();
            }
            Vector2 push = currentForceDirection * (tideCurve.Evaluate(ProgressActions) * Time.deltaTime * tideForce * ProgressDifficulty);
            fish.anchoredPosition += push;
            velocity += push;
        }
        
        private void ResetTide()
        {
            _interval = intervalCurve.Evaluate(ProgressActions);

            Vector2 randomDir = Random.insideUnitCircle.normalized;
            currentForceDirection = randomDir;
            tideCooldown.StartCooldown(_interval);
            
            if (tideArrow != null)
            {
                float angle = Mathf.Atan2(currentForceDirection.y, currentForceDirection.x) * Mathf.Rad2Deg;
                tideArrow.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        
        protected override bool IsActionCorrect()
        {
            Vector2 center = hudRect.rect.center;
            float distance = Vector2.Distance(fish.anchoredPosition, center);
            return distance <= maxDistance;
        }
    }
}