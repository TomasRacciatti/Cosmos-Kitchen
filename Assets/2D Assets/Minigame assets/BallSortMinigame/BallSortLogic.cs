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
        {
            foreach (Transform child in column.transform)
                Destroy(child.gameObject);
            column.balls.Clear();
        }

        List<Color> allBalls = new List<Color>();
        foreach (Color color in ballColors)
        {
            for (int i = 0; i < ballsPerColumn; i++)
                allBalls.Add(color);
        }

        System.Random rnd = new System.Random();
        allBalls = allBalls.OrderBy(x => rnd.Next()).ToList();

        int colIndex = 0;
        foreach (Color color in allBalls)
        {
            Column column = allColumns[colIndex % (allColumns.Count - 1)];
            GameObject ball = Instantiate(ballPrefab, column.transform);
            Ball ballScript = ball.GetComponent<Ball>();
            ballScript.SetColor(color);
            ballScript.fromColumn = column;
            ballScript.canvas = column.canvas;
            column.AddBall(ball);
            colIndex++;
        }
    }

    public bool HasWon()
    {
        int validColumns = 0;

        foreach (var column in allColumns)
        {
            if (column.balls.Count != ballsPerColumn)
                continue;

            Color? targetColor = null;
            bool allSame = true;

            foreach (var ballObj in column.balls)
            {
                if (ballObj == null) continue;

                Ball ball = ballObj.GetComponent<Ball>();
                if (ball == null) continue;

                if (targetColor == null)
                {
                    targetColor = ball.GetColor();
                }
                else if (ball.GetColor() != targetColor)
                {
                    allSame = false;
                    break;
                }
            }

            if (allSame)
                validColumns++;
        }

        return validColumns == 3;
    }
}
