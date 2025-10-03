using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Settings
{
    public class BooleanSetting : Setting<bool>
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private string trueString = "ON";
        [SerializeField] private string falseString = "OFF";

        private void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(ToggleValue);
            }

            UpdateButtonText();
            OnValueChanged.AddListener(_ => UpdateButtonText());
        }

        private void ToggleValue()
        {
            Value = !Value;
        }

        private void UpdateButtonText()
        {
            if (buttonText != null)
            {
                buttonText.text = Value ? trueString : falseString;
            }
        }
    }
}