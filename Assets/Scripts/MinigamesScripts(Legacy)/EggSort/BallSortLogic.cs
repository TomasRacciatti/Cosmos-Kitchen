using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSortLogic : MonoBehaviour
{
    [SerializeField] private List<Column> allColumns;
    [SerializeField] private int ballsPerColumn = 3;
    [SerializeField] private GameObject ballPrefab;

    private Color[] ballColors = { Color.red, Color.green, Color.blue };
    private System.Action onWinCallback;
    private System.Action onFailCallback;

    public void StartPuzzle(System.Action onWin, System.Action onFail)
    {
        onWinCallback = onWin;
        onFailCallback = onFail;
        SetupPuzzle();
    }

    private void SetupPuzzle()
    {
        foreach (var column in allColumns)
            column.ClearAll();

        var allBalls = new List<Color>();
        foreach (var color in ballColors)
            for (int i = 0; i < ballsPerColumn; i++)
                allBalls.Add(color);

        System.Random rnd = new System.Random();
        allBalls = allBalls.OrderBy(_ => rnd.Next()).ToList();

        int colIndex = 0;
        foreach (var color in allBalls)
        {
            Column column = allColumns[colIndex % (allColumns.Count - 1)];
            GameObject ball = Instantiate(ballPrefab);
            var ballScript = ball.GetComponent<Ball>();
            ballScript.SetColor(color);
            ballScript.fromColumn = column;
            //ballScript.canvas = column.canvas;

            column.PushBall(ballScript);
            colIndex++;
        }
    }

    public bool HasWon()
    {
        int validColumns = 0;

        foreach (var column in allColumns)
        {
            // rely on UI hierarchy count
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
}
