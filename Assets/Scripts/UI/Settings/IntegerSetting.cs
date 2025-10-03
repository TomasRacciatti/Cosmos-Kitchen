using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Settings
{
    public class IntegerSetting : Setting<int>
    {
        [Header("Step & Range")]
        [SerializeField] private int stepValue = 10;
        [SerializeField] private int minValue = 0;
        [SerializeField] private int maxValue = 100;

        [Header("UI")]
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button addButton;
        [SerializeField] private Button subtractButton;

        private void Awake()
        {
            if (slider != null)
            {
                slider.wholeNumbers = true;
                slider.minValue = minValue;
                slider.maxValue = maxValue;
                slider.value = Value;
                slider.onValueChanged.AddListener(v => Value = (int)v);
                OnValueChanged.AddListener(v => slider.value = v);
            }

            if (inputField != null)
            {
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                inputField.onEndEdit.AddListener(OnInputFieldChanged);
                OnValueChanged.AddListener(v => inputField.text = v.ToString());
                inputField.text = Value.ToString();
            }

            if (addButton != null)
                addButton.onClick.AddListener(AddStep);

            if (subtractButton != null)
                subtractButton.onClick.AddListener(SubtractStep);
        }

        private void OnInputFieldChanged(string text)
        {
            if (int.TryParse(text, out int result))
            {
                Value = Mathf.Clamp(result, minValue, maxValue);
            }
            else
            {
                inputField.text = Value.ToString();
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