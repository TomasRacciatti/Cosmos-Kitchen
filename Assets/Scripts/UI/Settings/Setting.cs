using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Settings
{
    [Serializable]
    public class SettingEvent<T> : UnityEvent<T> { }

    public abstract class Setting<T> : MonoBehaviour
    {
        [SerializeField] private SettingEvent<T> onValueChanged = new();
        [SerializeField] private T value;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                onValueChanged?.Invoke(this.value);
            }
        }
        
        public SettingEvent<T> OnValueChanged => onValueChanged;
    }
}