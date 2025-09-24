using Regulators;
using UnityEngine;

namespace MiniGames
{
    public class FishingMinigame1 : CanvasMinigame
    {
        [Header("References")]
        [SerializeField] private RectTransform fish;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 300f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float maxDistance = 400f;

        [Header("Tide Settings")]
        [SerializeField] private float tideForce = 400;
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
        
        private void FishMovement()
        {
            float rotationInput = Input.GetAxisRaw("Horizontal");
            fish.Rotate(0f, 0f, -rotationInput * rotationSpeed * Time.deltaTime);

            float moveInput = Mathf.Max(0, Input.GetAxisRaw("Vertical"));
            Vector2 forward = fish.right;
            Vector2 movement = forward * (moveInput * moveSpeed * Time.deltaTime);

            fish.anchoredPosition += movement;
            velocity = movement;
            
            RotateFishTowardsTide();
        }
        
        private void RotateFishTowardsTide()
        {
            if (currentForceDirection.sqrMagnitude > 0.01f)
            {
                Vector2 targetDir = velocity + currentForceDirection * tideInfluence;

                if (targetDir.sqrMagnitude > 0.01f)
                {
                    float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                    fish.rotation = Quaternion.Slerp(fish.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
        }
        
        private void CheckFishDistance()
        {
            if (IsActionCorrect()) return;
            ResetFish();
            ResetTide();
            Wrong();
        }
        
        protected override bool IsActionCorrect()
        {
            Vector2 center = hudRect.rect.center;
            float distance = Vector2.Distance(fish.anchoredPosition, center);
            return distance <= maxDistance;
        }
        
        private void ResetFish()
        {
            fish.anchoredPosition = hudRect.rect.center;
            fish.rotation = Quaternion.identity;

            currentForceDirection = Random.insideUnitCircle.normalized;
            tideCooldown.ResetCooldown();
            velocity = Vector2.zero;
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
        }
    }
}