using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Travel
{
    public class PlanetEntryList : MonoBehaviour
    {
        [SerializeField] private PlanetEntryView rowPrefab;
        [SerializeField] private ScrollRect scrollRect;

        private readonly List<PlanetEntryView> _rows = new();
        private int _usedRows;

        private void Awake()
        {
            if (rowPrefab == null) Debug.LogWarning($"[PlanetEntryList] {name}: no Row Prefab assigned.", this);
            if (scrollRect == null || scrollRect.content == null)
                Debug.LogWarning($"[PlanetEntryList] {name}: no Scroll Rect (with Content) assigned.", this);
        }

        public void BeginFill() => _usedRows = 0;

        public void Add(Sprite icon, string text)
        {
            if (rowPrefab == null || scrollRect == null) return;

            if (_usedRows == _rows.Count) _rows.Add(Instantiate(rowPrefab, scrollRect.content));
            var row = _rows[_usedRows++];
            row.gameObject.SetActive(true);
            row.Set(icon, text);
        }

        public void EndFill()
        {
            for (var i = _usedRows; i < _rows.Count; i++) _rows[i].gameObject.SetActive(false);
            if (scrollRect == null) return;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
