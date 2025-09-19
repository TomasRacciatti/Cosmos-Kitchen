using System;
using Regulators;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private Image image;

        private void OnEnable()
        {
            Invoke(nameof(ReturnToPool), 5);
        }

        private void ReturnToPool()
        {
            ObjectPool.ReturnObjectToPool(gameObject);
        }

        public void SetNotification(string text, Sprite sprite)
        {
            textMeshPro.SetText(text);
            image.sprite = sprite;
        }
    }
}