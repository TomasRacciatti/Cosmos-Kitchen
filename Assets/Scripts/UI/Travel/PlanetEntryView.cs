using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Travel
{
    public class PlanetEntryView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text label;

        public void Set(Sprite sprite, string text)
        {
            if (icon != null)
            {
                icon.sprite = sprite;
                icon.enabled = sprite != null;
            }
            if (label != null) label.text = text;
        }
    }
}
