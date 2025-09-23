using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Settings
{
    public class FloatSetting : Setting<float>
    {
        [Header("Step & Range")]
        [SerializeField] private float stepValue = 10;
        [SerializeField] private float minValue = 0;
        [SerializeField] private float maxValue = 100;

        [Header("UI")]
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button addButton;
        [SerializeField] private Button subtractButton;

        private void Awake()
        {
            if (slider != null)
            {
                slider.minValue = minValue;
                slider.maxValue = maxValue;
                slider.value = Value;
                slider.onValueChanged.AddListener(v => Value = v);
                OnValueChanged.AddListener(v => slider.value = v);
            }

            if (inputField != null)
            {
                inputField.onEndEdit.AddListener(OnInputFieldChanged);
                OnValueChanged.AddListener(v => inputField.text = v.ToString("0.##"));
                inputField.text = Value.ToString("0.##");
            }

            if (addButton != null)
                addButton.onClick.AddListener(AddStep);

            if (subtractButton != null)
                subtractButton.onClick.AddListener(SubtractStep);
        }

        private void OnInputFieldChanged(string text)
        {
            if (float.TryParse(text, out float result))
            {
                Value = Mathf.Clamp(result, minValue, maxValue);
            }
            else
            {
                inputField.text = Value.ToString("0.##");
            }
        }

        private void AddStep()
        {
            Value = Mathf.Clamp(Value + stepValue, minValue, maxValue);
        }

        private void SubtractStep()
        {
            Value = Mathf.Clamp(Value - stepValue, minValue, maxValue);
        }
    }
}