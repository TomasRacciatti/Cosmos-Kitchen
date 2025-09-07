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
        public int Amount => amount;
        public SoItem SoItem => soItem;
        public bool IsEmpty => soItem == null || amount <= 0;
        public bool IsFull => soItem != null && amount >= Stack;
        public int Stack => soItem.Stack;
        
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
        public int SetItem(ItemAmount itemAmount)
        {
            soItem = itemAmount.SoItem;
            SetAmount(Mathf.Clamp(itemAmount.Amount, 0, SoItem.Stack));
            return Mathf.Max(0, itemAmount.Amount - SoItem.Stack);
        }
        
        public int SetAmount(int newAmount)
        {
            if (IsEmpty) return newAmount;

            int clampedAmount = Mathf.Clamp(newAmount, 0, soItem.Stack);
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
            return SetAmount(amount - amountToRemove);
        }

        public void Clear()
        {
            soItem = null;
            amount = 0;
        }
    }
}
