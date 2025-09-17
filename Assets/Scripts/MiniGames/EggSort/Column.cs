using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.EggSort
{
    public class Column : MonoBehaviour
    {
        [SerializeField] private int maxBalls = 3;
    
        private readonly Stack<Ball> balls = new Stack<Ball>();
        public int BallCount => balls.Count;
        public int Capacity => maxBalls;
        public bool IsEmpty => balls.Count == 0;
    
        public Ball TopBall => balls.Count > 0 ? balls.Peek() : null;
        public bool IsTop(Ball ball) => TopBall == ball.gameObject;

        public bool CanAddBall(Ball ball)
        {
            if (ball == null) return false;
            if (balls.Count >= maxBalls) return false;

            return true;
        }

        public void PushBall(Ball ball)
        {
            balls.Push(ball);
            ball.fromColumn = this;
            ball.transform.SetParent(transform, false);
            ball.transform.SetAsFirstSibling();
        }

        public bool TryPopBall(Ball ball)
        {
            if (balls.Count == 0 || balls.Peek() != ball) return false;

            balls.Pop();
            return true;
        }
    
        public void AddBall(GameObject ballGO)
        {
            var ball = ballGO.GetComponent<Ball>();
            PushBall(ball);
        }
    
        public void ClearAll()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            balls.Clear();
        }
    }
}
