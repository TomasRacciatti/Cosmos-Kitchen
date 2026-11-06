using Characters.Player.Climbing;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class StaminaWheel : MonoBehaviour
    {
        [Tooltip("The circle that drains. Set to Filled / Radial 360 automatically.")]
        [SerializeField] private Image fillImage;
        [Tooltip("Optional. If empty, the registered player's stamina is used (for the HUD canvas).")]
        [SerializeField] private ClimbStamina stamina;
        [Tooltip("Wheel color by stamina left (left = empty, right = full).")]
        [SerializeField] private Gradient colorByFill = new Gradient
        {
            colorKeys = new[]
            {
                new GradientColorKey(Color.red, 0f),
                new GradientColorKey(Color.red, 0.1f),
                new GradientColorKey(Color.yellow, 0.5f),
                new GradientColorKey(Color.white, 1f)
            },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        };

        private IStamina _source;

        private void Awake()
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Radial360;
        }

        private void OnDisable()
        {
            if (_source != null) _source.Changed -= OnStaminaChanged;
            _source = null;
        }

        // Bound lazily: Update only runs after every Awake (so the stamina is initialized), and the HUD
        // can exist before the player has registered itself.
        private void Update()
        {
            if (_source == null) TryBind();
        }

        private void TryBind()
        {
            if (stamina != null)
            {
                _source = stamina;
            }
            else if (GameManager.Player != null)
            {
                _source = GameManager.Player.GetComponent<IStamina>();
                if (_source == null)
                {
                    Debug.LogWarning($"{nameof(StaminaWheel)}: the player has no stamina component.", this);
                    enabled = false;
                    return;
                }
            }
            if (_source == null) return;

            _source.Changed += OnStaminaChanged;
            OnStaminaChanged(_source.Current, _source.Max);
        }

        private void OnStaminaChanged(float current, float max)
        {
            float fill = max > 0f ? current / max : 0f;
            fillImage.fillAmount = fill;
            fillImage.color = colorByFill.Evaluate(fill);
        }
    }
}
