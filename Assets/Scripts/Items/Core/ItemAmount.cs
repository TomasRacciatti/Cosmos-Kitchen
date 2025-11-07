using System;
using System.Text;
using System.Collections.Generic;
using Cooking;
using Managers;
using UnityEngine;

namespace Items.Core
{
    [Serializable]
    public class ItemAmount
    {
        //Variables
        [SerializeField] private SoItem soItem;
        [SerializeField] private int amount;
        [SerializeField] private int rating;
        
        [SerializeField] private PreparationState prep;
        
        [SerializeField, HideInInspector] private List<StepRecord> processHistory = new List<StepRecord>();
        public IReadOnlyList<StepRecord> ProcessHistory => processHistory;
        
        //Getters
        public SoItem SoItem => soItem;
        public int Amount => amount;
        public int Stack => soItem != null ? soItem.Stack : 0;
        public bool ValidSoItem => soItem != null;
        public bool IsEmpty => soItem == null || amount <= 0;
        public bool IsFull => soItem != null && amount >= Stack;
        public int Rating => rating;
        public Sprite GetRatingSprite => !IsEmpty ? PrefabsManager.ItemStars[rating] : null;
        public PreparationState Prep { get => prep; set => prep = value; }
        
        public string GetProcessHistoryText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var step in processHistory)
            {
                sb.AppendLine($"{step.method} - Turns: {step.turns}");
            }

            return sb.ToString();
        }
        
        // Helper
        public void AddProcessStep(CookingMethod method, int turns)
        {
            // guardrail para evitar doble conteo
            if (processHistory.Count > 0)
            {
                var last = processHistory[^1];
                if (last.method == method)
                {
                    processHistory[^1] = new StepRecord { method = method, turns = turns };
                    return;
                }
            }
            
            processHistory.Add(new StepRecord { method = method, turns = turns });
        }
        
        //Constructors
        public ItemAmount(ItemAmount newItemAmount)
        {
            soItem = newItemAmount.SoItem;
            amount = newItemAmount.Amount;
            rating = newItemAmount.Rating;
            prep = newItemAmount.Prep;
            processHistory = new List<StepRecord>(newItemAmount.processHistory);
        }
        
        public ItemAmount(SoItem newSoItem = null, int newAmount = 0, int newStarRating = 3)
        {
            soItem = newSoItem;
            amount = newAmount;
            rating    = newStarRating;
            
            prep.method = CookingMethod.None;
            prep.turnsCooked = 0f;
            
            processHistory = new List<StepRecord>();
        }
        
        //Setters
        public int SetItem(ItemAmount itemAmount, bool clampToStack = true)
        {
            soItem = itemAmount.SoItem;
            rating = itemAmount.Rating;
            prep = itemAmount.Prep;
            
            if (processHistory == null) 
                processHistory = new List<StepRecord>();
            else 
                processHistory.Clear();
            
            if (itemAmount.ProcessHistory != null)
                processHistory.AddRange(itemAmount.ProcessHistory);
            
            return SetAmount(itemAmount.Amount, clampToStack);
        }

        public void SetItem(SoItem newItem)
        {
            soItem = newItem;
            
            prep.method = CookingMethod.None;
            prep.turnsCooked = 0f;
        }

        // Rating - Tomi
        public void SetRating(int newStarRating)
        {
            rating = newStarRating;
        }
        
        public int SetAmount(int newAmount, bool clampToStack = true)
        {
            if (!ValidSoItem) return newAmount;

            int clampedAmount = clampToStack 
                ? Mathf.Clamp(newAmount, 0, SoItem.Stack)
                : Mathf.Max(0, newAmount);

            amount = clampedAmount;

            if (amount <= 0) Clear();
            return newAmount - clampedAmount;
        }
        
        public int AddAmount(int amountToAdd)
        {
            if (IsEmpty || amountToAdd <= 0) return amountToAdd;
            return SetAmount(amount + amountToAdd);
        }

        public int RemoveAmount(int amountToRemove)
        {
            if (IsEmpty || amountToRemove <= 0) return amountToRemove;
            return SetAmount(amount - amountToRemove, false);
        }

        public void Clear()
        {
            soItem = null;
            amount = 0;
            rating = 0;
            
            prep.method = CookingMethod.None;
            prep.turnsCooked = 0f;
            
            processHistory.Clear();
        }
    }
}
