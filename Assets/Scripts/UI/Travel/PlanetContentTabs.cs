using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Travel
{
    public enum PlanetContentTab
    {
        Ingredients,
        Clients
    }

    public class PlanetContentTabs : MonoBehaviour
    {
        [SerializeField] private Button ingredientsButton;
        [SerializeField] private Button clientsButton;

        [Header("Look")]
        [SerializeField, Tooltip("Color of the active tab's image.")]
        private Color selectedColor = Color.white;
        [SerializeField, Tooltip("Color of the inactive tab's image.")]
        private Color unselectedColor = new(0.65f, 0.65f, 0.65f, 1f);

        public PlanetContentTab Current { get; private set; } = PlanetContentTab.Ingredients;

        public event Action<PlanetContentTab> Changed;

        private void Awake()
        {
            if (ingredientsButton != null) ingredientsButton.onClick.AddListener(SelectIngredients);
            else Debug.LogWarning($"[PlanetContentTabs] {name}: no Ingredients Button assigned.", this);
            if (clientsButton != null) clientsButton.onClick.AddListener(SelectClients);
            else Debug.LogWarning($"[PlanetContentTabs] {name}: no Clients Button assigned.", this);

            RefreshLook();
        }

        private void OnDestroy()
        {
            if (ingredientsButton != null) ingredientsButton.onClick.RemoveListener(SelectIngredients);
            if (clientsButton != null) clientsButton.onClick.RemoveListener(SelectClients);
        }

        public void Select(PlanetContentTab tab)
        {
            if (Current == tab) return;
            Current = tab;
            RefreshLook();
            Changed?.Invoke(tab);
        }

        private void SelectIngredients() => Select(PlanetContentTab.Ingredients);

        private void SelectClients() => Select(PlanetContentTab.Clients);

        // Tints Graphic.color, which the Button's own color transition (on the CanvasRenderer) multiplies on top of.
        private void RefreshLook()
        {
            SetTint(ingredientsButton, Current == PlanetContentTab.Ingredients);
            SetTint(clientsButton, Current == PlanetContentTab.Clients);
        }

        private void SetTint(Button button, bool selected)
        {
            if (button != null && button.targetGraphic != null)
                button.targetGraphic.color = selected ? selectedColor : unselectedColor;
        }
    }
}
