using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.EggSort
{
    public class BallSortUI : MiniGameUI
    {
        [Header("Board")] [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform dragLayer;
        [SerializeField] private List<Column> allColumns;
        [SerializeField] private int ballsPerColumn = 3;
        [SerializeField] private GameObject ballPrefab;

        [Header("Colors")] [SerializeField] private Color[] ballColors = { Color.red, Color.green, Color.blue };

        [Header("SFX (optional)")] [SerializeField]
        private AudioClip enterClip;

        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip loseClip;

        private System.Action<AudioClip> _playSfx;

        public void Setup(System.Action<AudioClip> playSfx)
        {
            _playSfx = playSfx;

            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            EnsureRaycaster(canvas);

            if (dragLayer == null && canvas != null)
            {
                var t = canvas.transform.Find("DragLayer");
                dragLayer = t ? (RectTransform)t : CreateDragLayer(canvas);
            }

            gameObject.SetActive(true);
            _playSfx?.Invoke(enterClip);

            SpawnAndShuffle();
        }
        
        public void Teardown()
        {
            gameObject.SetActive(false);
        }
        
        public bool HasWon()
        {
            int validColumns = 0;

            foreach (var column in allColumns)
            {
                if (column.transform.childCount != ballsPerColumn)
                    continue;

                Color? target = null;
                bool allSame = true;

                foreach (Transform child in column.transform)
                {
                    var b = child.GetComponent<Ball>();
                    if (b == null) continue;

                    if (target == null) target = b.GetColor();
                    else if (b.GetColor() != target) { allSame = false; break; }
                }

                if (allSame) validColumns++;
            }

            return validColumns == ballColors.Length;
        }

        public void OnWin()  => _playSfx?.Invoke(winClip);
        public void OnLose() => _playSfx?.Invoke(loseClip);
        
        private void SpawnAndShuffle()
        {
            // Limpiamos las columnas
            foreach (var column in allColumns)
                column.ClearAll();

            // Creamos las listas de huevos
            var all = new List<Color>();
            foreach (var color in ballColors)
                for (int i = 0; i < ballsPerColumn; i++)
                    all.Add(color);

            // Aleatorizamos el orden de los huevos
            System.Random rnd = new System.Random();
            all = all.OrderBy(_ => rnd.Next()).ToList();

            // Llenamos todas las columnas excepto la ultima porque esa la queremos vacia
            int usable = Mathf.Max(1, allColumns.Count - 1);
            int colIndex = 0;

            foreach (var color in all)
            {
                Column column = allColumns[colIndex % usable];
                GameObject ballGO = Instantiate(ballPrefab, column.transform);
                var ball = ballGO.GetComponent<Ball>();

                // Aca insertamos las dependencias al script de las pelotas
                ball.canvas = canvas;
                ball.dragLayer = dragLayer;
                ball.playSfx = _playSfx;

                ball.SetColor(color);
                ball.fromColumn = column;
                column.PushBall(ball);
                colIndex++;
            }
        }


        #region -------- Helpers ---------

        private static void EnsureRaycaster(Canvas c)
        {
            if (c == null) return;
            if (!c.TryGetComponent<GraphicRaycaster>(out _))
                c.gameObject.AddComponent<GraphicRaycaster>();
        }

        private static RectTransform CreateDragLayer(Canvas c)
        {
            var go = new GameObject("DragLayer", typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(c.transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        #endregion
    }
}