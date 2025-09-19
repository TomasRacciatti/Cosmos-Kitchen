using System;
using System.Collections.Generic;
using System.Linq;
using Items.Core;
using UnityEngine;

namespace Items.Inventory
{
    public class InvSystem : MonoBehaviour
    {
        //Variables
        [SerializeField, Min(-1)] private int slots = 10;
        [SerializeField] private List<ItemAmount> items = new();

        //Getters
        public ItemAmount Item(int index) => items[index];
        public IReadOnlyList<ItemAmount> Items => items;
        public int Slots => slots;
        public bool Infinite => slots == -1;
        public bool ValidIndex(int index) => index >= 0 && index < items.Count;
        
        //Observers
        private event Action<int, ItemAmount> OnSlotChanged;
        public void Subscribe(Action<int, ItemAmount> listener) => OnSlotChanged += listener;
        public void Unsubscribe(Action<int, ItemAmount> listener) => OnSlotChanged -= listener;

        private void Awake()
        {
            if (Infinite) return;
            
            items = items
                .Take(slots)
                .Concat(Enumerable.Range(0, slots - items.Count)
                    .Select(_ => new ItemAmount()))
                .ToList();
        }

        private void Start()
        {
            NotifyInventoryChanged();
        }

        public void AddItem(ref ItemAmount itemAmount)
        {
            if (itemAmount.IsEmpty) return;
            AddStackSlot(ref itemAmount);
            if (itemAmount.IsEmpty) return;
            AddEmptySlot(ref itemAmount);
            if (itemAmount.IsEmpty) return;
            AddExtraSlot(ref itemAmount);
        }
        
        public void RemoveItem(ref ItemAmount itemAmount)
        {
            if (itemAmount.IsEmpty) return;

            for (int i = 0; i < items.Count; i++)
            {
                var slotItem = items[i];

                if (!slotItem.IsEmpty && slotItem.SoItem == itemAmount.SoItem)
                {
                    itemAmount.SetAmount(slotItem.RemoveAmount(itemAmount.Amount));

                    items[i] = slotItem.IsEmpty ? new ItemAmount() : slotItem;
                    NotifySlotChanged(i);

                    if (itemAmount.IsEmpty) return;
                }
            }
        }
        
        public void SetItemByIndex(int index, ItemAmount itemAmount)
        {
            if (!ValidIndex(index)) return;
            
            if (itemAmount.IsEmpty)
            {
                ClearSlot(index);
                return;
            }

            items[index] = itemAmount;
            NotifySlotChanged(index);
        }
        
        
        public void ClearSlot(int i)
        {
            if (Infinite)
            {
                items.RemoveAt(i);
                NotifyInventoryChanged(); //Se puede optimizar seguro
            }
            else
            {
                items[i].Clear();
                NotifySlotChanged(i);
            }
        }
        
        public void ClearInventory()
        {
            if (Infinite)
            {
                items.Clear();
                NotifyInventoryChanged();
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Clear();
                    NotifySlotChanged(i);
                }
            }
        }
        
        //Privates
        private void AddStackSlot(ref ItemAmount itemAmount)
        {
            if (itemAmount.SoItem.Stack <= 1) return;

            for (int i = 0; i < items.Count; i++)
            {
                var slotItemAmount = items[i];

                if (!slotItemAmount.IsEmpty && ItemsUtility.Stackable(itemAmount, slotItemAmount))
                {
                    itemAmount.SetAmount(slotItemAmount.AddAmount(itemAmount.Amount));
                    items[i] = slotItemAmount;
                    NotifySlotChanged(i);

                    if (itemAmount.Amount <= 0)
                        return;
                }
            }
        }
        
        private void AddEmptySlot(ref ItemAmount itemAmount)
        {
            if (itemAmount.IsEmpty) return;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (item.IsEmpty)
                {
                    itemAmount.SetAmount(item.SetItem(itemAmount));
                    items[i] = item;

                    NotifySlotChanged(i);
                    if (itemAmount.Amount <= 0)
                        return;
                }
            }
        }
        
        private void AddExtraSlot(ref ItemAmount itemAmount)
        {
            if (!Infinite) return;
            while (!itemAmount.IsEmpty)
            {
                ItemAmount newItem = new ItemAmount();
                itemAmount.SetAmount(newItem.SetItem(itemAmount));
                items.Add(newItem);
                NotifySlotChanged(items.Count - 1);
            }
        }        

        public void NotifySlotChanged(int index)
        {
            if (!ValidIndex(index)) return;
            OnSlotChanged?.Invoke(index, items[index]);
        }
        
        private void NotifyInventoryChanged()
        {
            for (int i = 0; i < items.Count; i++)
                NotifySlotChanged(i);
        }
        
        public void TransferIndexToIndex(InvSystem targetInventory, int fromIndex, int targetIndex)
        {
            if (targetInventory == null) return;
            if (fromIndex < 0 || fromIndex >= Slots) return;
            if (targetIndex < 0 || targetIndex >= targetInventory.Slots) return;
            if (this == targetInventory && fromIndex == targetIndex) return;

            ItemAmount fromItem = items[fromIndex];
            if (fromItem.IsEmpty) return;

            ItemAmount targetItem = targetInventory.Item(targetIndex);
            
            if (targetItem.IsEmpty) //si esta vacio
            {
                targetInventory.SetItemByIndex(targetIndex, new ItemAmount(fromItem));
                ClearSlot(fromIndex);
                NotifySlotChanged(fromIndex);
                return;
            }
            
            if (ItemsUtility.Stackable(fromItem, targetItem)) //si son stackeables
            {
                int remaining = targetInventory.items[targetIndex].AddAmount(fromItem.Amount);
                targetInventory.NotifySlotChanged(targetIndex);

                items[fromIndex].SetAmount(remaining);
                NotifySlotChanged(fromIndex);
                return;
            }

            // Si no son stackeables, hacemos un swap
            targetInventory.SetItemByIndex(targetIndex, new ItemAmount(fromItem));
            SetItemByIndex(fromIndex, targetItem);
        }
        
        public bool SplitItemStack(int index)
        {
            if (index < 0 || index >= Items.Count) return false;

            var item = Items[index];
            if (item.IsEmpty || item.Amount <= 1) return false;
            
            int halfAmount = item.Amount / 2;
            int remainder = item.Amount - halfAmount;
            
            int emptyIndex = GetFirstEmptySlotIndex();
            if (emptyIndex == -1) return false;

            // Asignar cantidades
            items[index].SetAmount(remainder);
            items[emptyIndex] = new ItemAmount(item.SoItem, halfAmount);

            NotifySlotChanged(index);
            NotifySlotChanged(emptyIndex);

            return true;
        }
        
        private int GetFirstEmptySlotIndex()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].IsEmpty) return i;
            }
            return -1;
        }
    }
}