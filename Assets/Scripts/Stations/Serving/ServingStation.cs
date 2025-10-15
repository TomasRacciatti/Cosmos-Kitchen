using System.Linq;
using Characters.Clients.Plates;
using Items.Core;
using Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Stations.Serving
{
    public class ServingStation : Station
    {
        [Header("Serving Settings")]
        [SerializeField] private SoPlate[] availablePlates; // Todos los platos que esta estación puede preparar
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
            PlatesOrdered.Show();
        }

        protected override void LeaveStation()
        {
            Button button = CanvasInstance.GetComponentInChildren<Button>();
            button.onClick.RemoveListener(TryCraftPlate);
            PlatesOrdered.Hide();
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

        private bool CanCraftPlate(SoPlate soPlate)
        {
            var required = soPlate.RequiredIngredients.Where(i => i != null).ToArray();

            foreach (var ingredient in required)
            {
                if (!inputInventory.HasItem(ingredient)) return false;
            }

            return true;
        }

        private void CraftPlate(SoPlate soPlate)
        {
            // Sacar ingredientes
            foreach (var ingredient in soPlate.RequiredIngredients.Where(i => i != null))
            {
                inputInventory.RemoveItem(ingredient, 1);
            }

            // Agregar el plato resultante
            outputInventory.AddItem(soPlate, 1);

            Debug.Log($"¡Se preparó el plato {soPlate.name}!");
        }
    }
}