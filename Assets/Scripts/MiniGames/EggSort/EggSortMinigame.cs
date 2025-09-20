using System.Collections.Generic;
using System.Linq;
using Characters.Player;
using Regulators;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MiniGames.EggSort
{
    public class EggSortMinigame : CanvasMinigame
    {
        [Header("Board")]
        [SerializeField] private List<Column> allColumns;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private float time = 10;
        
        [Header("Colors")] 
        [SerializeField] private Color[] ballColors = { Color.red, Color.green, Color.blue };

        private void Start()
        {
            foreach (var column in allColumns)
            {
                column.OnEggDropped += () =>
                {
                    if (CanWin())
                    {
                        Win();
                        return;
                    }
                    Correct();
                };
            }
        }

        public override void StartMinigame()
        {
            base.StartMinigame();
            PlayerInputs.SetCursor(true);
            SpawnEggs();
            CancelInvoke(nameof(Wrong));
            InvokeRepeating(nameof(Wrong), time, time);
        }

        protected override bool IsActionCorrect()
        {
            return true;
        }

        private void SpawnEggs()
        {
            while (true)
            {
                ClearAll();

                foreach (var color in ballColors)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject spawnObject = ObjectPool.SpawnObject(ballPrefab, Vector3.one, Quaternion.identity);
                        spawnObject.transform.localScale = Vector3.one;
                        Egg egg = spawnObject.GetComponent<Egg>();
                        if (spawnObject.TryGetComponent(out Image img)) img.color = color;

                        while (true)
                        {
                            var column = allColumns[Random.Range(0, allColumns.Count)];
                            if (column.TryAddEgg(egg)) break;
                        }
                    }
                }

                if (CanWin()) continue;
                break;
            }
        }

        public override void ExitMinigame()
        {
            base.ExitMinigame();
            PlayerInputs.SetCursor(false);
            CancelInvoke(nameof(Wrong));
        }

        private void ClearAll()
        {
            foreach (var column in allColumns)
            {
                column.ClearAll();
            }
        }

        private bool CanWin()
        {
            return allColumns.All(column => column.ColumnSameColor());
        }

        protected override void Correct()
        {
            AudioManager.instance.PlaySFX(correctSound);
        }
    }
}