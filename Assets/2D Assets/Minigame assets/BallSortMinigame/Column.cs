using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Column : MonoBehaviour
{
    public Stack<GameObject> balls = new Stack<GameObject>();
    public Canvas canvas;
    [SerializeField] private int maxBalls = 3;

    public void AddBall(GameObject ballObj)
    {
        ballObj.transform.SetParent(transform);
        ballObj.transform.SetSiblingIndex(transform.childCount);
        balls.Push(ballObj);
        UpdateBallInteractivity();
    }

    public void RemoveBall(Ball ball)
    {
        Stack<GameObject> tempStack = new Stack<GameObject>();
        bool removed = false;

        while (balls.Count > 0)
        {
            var top = balls.Pop();
            if (!removed && top == ball.gameObject)
            {
                removed = true;
                continue;
            }
            tempStack.Push(top);
        }

        while (tempStack.Count > 0)
            balls.Push(tempStack.Pop());

        UpdateBallInteractivity();
    }

    public void UpdateBallInteractivity()
    {
        foreach (var ballObj in balls)
        {
            if (ballObj == null) continue;

            CanvasGroup cg = ballObj.GetComponent<CanvasGroup>();
            if (cg == null) continue;

            cg.blocksRaycasts = true;
        }
    }

    public bool CanAddBall(Ball ball)
    {
        return balls.Count < maxBalls;
    }
}
