using UnityEngine;
using UnityEngine.Events;

namespace MiniGames
{
    public class FishingUI : MiniGameUI
    {
        [Header("Fish Reference")]
        [SerializeField] private RectTransform fish;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 300f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float pushForce = 250f;

        [Header("Marea Settings")]
        [SerializeField] private float directionChangeInterval = 2f; 
        [SerializeField] private float mareaInfluence = 0.3f;
        private Vector2 currentForceDirection;
        private float directionTimer;

        [Header("Circle Settings")]
        [SerializeField] private float maxDistance = 400f;

        [Header("Events")]
        public UnityEvent OnFishEscape = new UnityEvent();

        private RectTransform hudRect;
        private Vector2 velocity; // guarda la velocidad actual del pez

        private void Awake()
        {
            hudRect = GetComponent<RectTransform>();
        }

        public void StartFishing()
        {
            OnFishEscape.RemoveAllListeners();
            fish.anchoredPosition = hudRect.rect.center;
            fish.rotation = Quaternion.identity;

            currentForceDirection = Random.insideUnitCircle.normalized;
            directionTimer = directionChangeInterval;
            velocity = Vector2.zero;
        }

        private void Update()
        {
            HandlePlayerMovement();
            ApplyRandomPushForce();
            RotateTowardsMovement();
            CheckFishDistance();
        }

        private void HandlePlayerMovement()
        {
            if (fish == null) return;

            float rotationInput = Input.GetAxisRaw("Horizontal");
            fish.Rotate(0f, 0f, -rotationInput * rotationSpeed * Time.deltaTime);

            float moveInput = Mathf.Max(0f, Input.GetAxisRaw("Vertical"));
            Vector2 forward = fish.right;
            Vector2 movement = forward * (moveInput * moveSpeed * Time.deltaTime);

            fish.anchoredPosition += movement;
            velocity = movement;
        }

        private void ApplyRandomPushForce()
        {
            if (fish == null) return;

            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0f)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                Vector2 influence = velocity.normalized * mareaInfluence;

                currentForceDirection = (randomDir + influence).normalized;

                directionTimer = directionChangeInterval;
            }

            Vector2 push = currentForceDirection * (pushForce * Time.deltaTime);
            fish.anchoredPosition += push;
            velocity += push;
        }

        private void RotateTowardsMovement()
        {
            if (velocity.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                
                fish.rotation = Quaternion.Slerp(
                    fish.rotation,
                    targetRotation,
                    Time.deltaTime * mareaInfluence
                );
            }
        }
        
        private void CheckFishDistance()
        {
            if (fish == null || hudRect == null) return;

            Vector2 center = hudRect.rect.center;
            float distance = Vector2.Distance(fish.anchoredPosition, center);

            if (distance >= maxDistance)
            {
                OnFishEscape?.Invoke();
            }
        }
    }
}