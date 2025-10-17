using System.Collections.Generic;
using System.Linq;
using Characters.Clients.Plates;
using Cooking;
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

        private RecipeValidator _validator;
        
        private SoPlate _pendingPlate;
        private List<(int slotIndex, ItemAmount item)> _inputSnapshot = new();
        private List<int> _matchedInputIndices = new();
        
        protected override void Awake()
        {
            base.Awake();
            _validator = new RecipeValidator();
        }

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

        private bool CanCraftPlate(SoPlate plate)
        {
            _inputSnapshot.Clear();
            for (int i = 0; i < inputInventory.Items.Count; i++)
            {
                var it = inputInventory.Items[i];
                if (!it.IsEmpty) _inputSnapshot.Add((i, it));
            }
            if (_inputSnapshot.Count == 0) return false;
            
            var inputItems = _inputSnapshot.Select(t => t.item).ToList();

            string reason;
            if (!_validator.ValidateIdentity(plate, inputItems, out reason))
            {
                return false;
            }
            
            _pendingPlate = plate;
            _matchedInputIndices = new List<int>(_validator.LastMatchedIndices); 

            return true;
        }

        private void CraftPlate(SoPlate plate)
        {
            if (_pendingPlate != plate || _matchedInputIndices == null || _matchedInputIndices.Count == 0)
            {
                if (!CanCraftPlate(plate)) return;
            }

            var inputItems = _inputSnapshot.Select(t => t.item).ToList();
            
            int mistakes = _validator.EvaluateDonenessMistakes(plate, inputItems);
            int rating   = _validator.ComputeOutputRating(plate, baseRating: 5, mistakes: mistakes);
            
            var usesPerSlot = new Dictionary<int, int>();
            foreach (var matchedIdx in _matchedInputIndices)
            {
                int slotIndex = _inputSnapshot[matchedIdx].slotIndex;
                if (!usesPerSlot.ContainsKey(slotIndex)) usesPerSlot[slotIndex] = 0;
                usesPerSlot[slotIndex]++;
            }

            foreach (var kv in usesPerSlot)
            {
                int slotIndex = kv.Key;
                int count = kv.Value;
                for (int c = 0; c < count; c++)
                    inputInventory.Items[slotIndex].RemoveAmount(1);
                
                if (inputInventory.Items[slotIndex].Amount <= 0)
                    inputInventory.Items[slotIndex].Clear();

                inputInventory.NotifySlotChanged(slotIndex);
            }
            
            outputInventory.AddItem(plate, 1);
            var outItem = outputInventory.Items[0];
            outItem.SetRating(rating);
            outputInventory.NotifySlotChanged(0);

            Debug.Log($"¡Se preparó el plato {plate.name}! (mistakes={mistakes}, rating={rating})");
            
            _pendingPlate = null;
            _matchedInputIndices.Clear();
            _inputSnapshot.Clear();
        }
    }
}