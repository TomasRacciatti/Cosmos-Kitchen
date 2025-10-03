using System;
using System.Collections.Generic;
using System.Linq;
using Regulators;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGames.EggSort
{
    public class Column : MonoBehaviour, IDropHandler
    {
        [SerializeField] private int maxEggs = 3;
        [SerializeField] public RectTransform childTransform;
        private readonly Stack<Egg> eggs = new();
        
        public event Action OnEggDropped;

        private void Awake()
        {
            if (childTransform == null) childTransform = GetComponentInChildren<RectTransform>();
        }
        
        public bool IsTopEgg(Egg egg)
        {
            return eggs.Count > 0 && eggs.Peek() == egg;
        }

        private bool HasSpace()
        {
            return eggs.Count < maxEggs;
        }
        
        public bool TryAddEgg(Egg egg)
        {
            if (egg == null) return false;

            if (eggs.Count >= maxEggs) return false;

            eggs.Push(egg);
            egg.transform.SetParent(childTransform, false);
            egg.SetColumn(this);
            egg.transform.localPosition = Vector3.zero;
            return true;
        }
        
        private bool TryRemoveEgg(Egg egg)
        {
            if (!IsTopEgg(egg)) return false;
            eggs.Pop();
            return true;
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            var egg = eventData.pointerDrag.GetComponent<Egg>();
            if (egg == null) return;

            var originColumn = egg.GetColumn();
            if (originColumn == null) return;
            if (originColumn == this) return;

            if (!originColumn.IsTopEgg(egg)) return;

            if (!HasSpace()) return;

            originColumn.TryRemoveEgg(egg);
            TryAddEgg(egg);
            egg.SetColumn(this);
            
            OnEggDropped?.Invoke();
        }
        
        public void ClearAll()
        {
            while (eggs.Count > 0)
            {
                var egg = eggs.Pop();
                if (egg != null)
                {
                    ObjectPool.ReturnObjectToPool(egg.gameObject);
                }
            }
        }
        
        public bool ColumnSameColor()
        {
            if (eggs.Count == 0) return true;
            if (eggs.Count != maxEggs) return false;
            Color firstColor = eggs.Peek().GetComponent<UnityEngine.UI.Image>().color;
            return eggs.All(e => e.GetComponent<UnityEngine.UI.Image>().color == firstColor);
        }
    }
}