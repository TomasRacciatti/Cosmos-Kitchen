using UnityEngine;
using UnityEngine.Events;

namespace MiniGames
{
    public class FishingUI : MiniGameUI
    {
        [Header("Fish Reference")]
        [SerializeField] private RectTransform fish;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 200f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float pushForce = 50f;

        [Header("Circle Settings")]
        [SerializeField] private float maxDistance = 150f;

        [Header("Events")]
        public UnityEvent OnFishEscape = new UnityEvent();

        private RectTransform hudRect;

        private void Awake()
        {
            hudRect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            OnFishEscape.RemoveAllListeners();
            fish.anchoredPosition = hudRect.rect.center;
            fish.rotation = Quaternion.identity;
            print("aaa");
        }

        private void Update()
        {
            HandlePlayerMovement();
            ApplyPushForce();
            CheckFishDistance();
        }

        private void HandlePlayerMovement()
        {
            if (fish == null) return;

            float rotationInput = Input.GetAxisRaw("Horizontal");
            fish.Rotate(0f, 0f, -rotationInput * rotationSpeed * Time.deltaTime);

            float moveInput = Input.GetAxisRaw("Vertical");
            Vector2 forward = fish.right;
            fish.anchoredPosition += forward * (moveInput * moveSpeed * Time.deltaTime);
        }

        private void ApplyPushForce()
        {
            if (fish == null || hudRect == null) return;

            Vector2 center = hudRect.rect.center;
            Vector2 direction = (fish.anchoredPosition - center).normalized;

            fish.anchoredPosition += direction * (pushForce * Time.deltaTime);
        }

        private void CheckFishDistance()
        {
            if (fish == null || hudRect == null) return;

            Vector2 center = hudRect.rect.center;
            float distance = Vector2.Distance(fish.anchoredPosition, center);

            if (distance >= maxDistance)
            {
                OnFishEscape?.Invoke();
                enabled = false;
            }
        }
    }
}