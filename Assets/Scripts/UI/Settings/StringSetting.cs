using UnityEngine;
using TMPro;

namespace UI.Settings
{
    public class StringSetting : Setting<string>
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField inputField;

        private void Awake()
        {
            if (inputField != null)
            {
                inputField.text = Value;
                inputField.onEndEdit.AddListener(OnInputFieldChanged);
                OnValueChanged.AddListener(v => inputField.text = v);
            }
        }

        private void OnInputFieldChanged(string text)
        {
            Value = text;
        }
    }
}