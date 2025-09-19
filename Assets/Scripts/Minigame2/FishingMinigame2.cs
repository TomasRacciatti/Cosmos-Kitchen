using Regulators;
using UnityEngine;

namespace Minigame2
{
    public class FishingMinigame2 : CanvasMinigame
    {
        [Header("References")]
        [SerializeField] private RectTransform fish;

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
            float h = Input.GetAxisRaw("Horizontal"); // A (-1) D (+1)
            float v = Input.GetAxisRaw("Vertical");   // S (-1) W (+1)

            Vector2 input = new Vector2(h, v).normalized;

            // Movimiento absoluto (UI, sin depender de rotación previa)
            Vector2 movement = input * moveSpeed * Time.deltaTime;
            fish.anchoredPosition += movement;

            if (movement != Vector2.zero)
            {
                velocity = movement;

                // Ángulo objetivo (en grados) para mirar hacia la dirección de movimiento
                float targetAngle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

                // Rotación actual
                Quaternion currentRot = fish.rotation;

                // Rotación objetivo
                Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

                // Interpolar suavemente
                float rotationSpeed = 10f; // ajusta este valor para más o menos suavidad
                fish.rotation = Quaternion.Slerp(currentRot, targetRot, rotationSpeed * Time.deltaTime);
            }
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
        
        protected override bool IsActionCorrect()
        {
            Vector2 center = hudRect.rect.center;
            float distance = Vector2.Distance(fish.anchoredPosition, center);
            return distance <= maxDistance;
        }
    }
}