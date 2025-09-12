using System;
using UnityEngine;

namespace Items.Core
{
    [Serializable]
    public class ItemAmount
    {
        //Variables
        [SerializeField] private SoItem soItem;
        [SerializeField] private int amount;
        
        //Getters
        public SoItem SoItem => soItem;
        public int Amount => amount;
        public int Stack => soItem != null ? soItem.Stack : 0;
        public bool ValidSoItem => soItem != null;
        public bool IsEmpty => soItem == null || amount <= 0;
        public bool IsFull => soItem != null && amount >= Stack;
        
        //Constructors
        public ItemAmount(ItemAmount newItemAmount)
        {
            soItem = newItemAmount.SoItem;
            amount = newItemAmount.Amount;
        }
        
        public ItemAmount(SoItem newSoItem = null, int newAmount = 0)
        {
            soItem = newSoItem;
            amount = newAmount;
        }
        
        //Setters
        public int SetItem(ItemAmount itemAmount, bool clampToStack = true)
        {
            soItem = itemAmount.SoItem;
            return SetAmount(itemAmount.Amount, clampToStack);
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
        }
    }
}
