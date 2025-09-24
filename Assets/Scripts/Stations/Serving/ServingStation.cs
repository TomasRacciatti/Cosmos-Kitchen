using System.Linq;
using Characters.Clients.Plates;
using Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Stations.Serving
{
    public class ServingStation : Station
    {
        [Header("Serving Settings")]
        [SerializeField] private PlateSO[] availablePlates; // Todos los platos que esta estación puede preparar
        [SerializeField] private InvSystem inputInventory;  // Donde el jugador pone los ingredientes
        [SerializeField] private InvSystem outputInventory; // Donde se entrega el plato (puede ser el mismo o distinto)

        protected override void EnterStation()
        {
            base.EnterStation();
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(TryCraftPlate);
            ServingUIManager servUIManager = CanvasInstance.GetComponent<ServingUIManager>();
            if (servUIManager != null)
            {
                servUIManager.inputView.SetInventory(inputInventory);
                servUIManager.outputView.SetInventory(outputInventory);
            }
        }

        protected override void LeaveStation()
        {
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.RemoveListener(TryCraftPlate);
            base.LeaveStation();
        }

        public void TryCraftPlate()
        {
            if (!outputInventory.Items[0].IsEmpty) return;
            foreach (var plate in availablePlates)
            {
                if (!CanCraftPlate(plate)) continue;
                CraftPlate(plate);
                return;
            }
        }

        private bool CanCraftPlate(PlateSO plate)
        {
            var required = plate.RequiredIngredients.Where(i => i != null).ToArray();

            foreach (var ingredient in required)
            {
                if (!inputInventory.HasItem(ingredient)) return false;
            }

            return true;
        }

        private void CraftPlate(PlateSO plate)
        {
            // Sacar ingredientes
            foreach (var ingredient in plate.RequiredIngredients.Where(i => i != null))
            {
                inputInventory.RemoveItem(ingredient, 1);
            }

            // Agregar el plato resultante
            outputInventory.AddItem(plate, 1);

            Debug.Log($"¡Se preparó el plato {plate.name}!");
        }
    }
}