using System.Collections.Generic;
using UnityEngine;

namespace Cooking
{
    public class CookingTicker : MonoBehaviour, ICookingTicker
    {
        [Header("Tick Settings")]
        [SerializeField] private float secondsPerTick = 1f;
        public float SecondsPerTick => secondsPerTick;
    
        [Header("Control")]
        [SerializeField] private bool paused;
    
        private readonly List<CookingSession> _sessions = new List<CookingSession>();
        private WaitForSeconds _wait;

        private float _lastAppliedTick;
    
        public void Pause()  => paused = true;
        public void Resume() => paused = false;
    
        private void OnEnable()
        {
            secondsPerTick = Mathf.Max(0.01f, secondsPerTick);
            _wait = new WaitForSeconds(Mathf.Max(0.01f, secondsPerTick));
            _lastAppliedTick = secondsPerTick;
            StartCoroutine(TickLoop());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _sessions.Clear();
        }
    
        public void Register(CookingSession session)
        {
            if (session == null) return;
            if (!_sessions.Contains(session))
            {
                _sessions.Add(session);
                session.isActive = true;
            }
        }

        public void Unregister(CookingSession session)
        {
            if (session == null) return;
            int idx = _sessions.IndexOf(session);
            if (idx >= 0)
            {
                _sessions[idx].isActive = false;
                _sessions.RemoveAt(idx);
            }
        }
    
        private System.Collections.IEnumerator TickLoop()
        {
            while (true)
            {
                if (!Mathf.Approximately(secondsPerTick, _lastAppliedTick))
                {
                    secondsPerTick = Mathf.Max(0.01f, secondsPerTick);
                    _wait = new WaitForSeconds(secondsPerTick);
                    _lastAppliedTick = secondsPerTick;
                }
                
                if (!paused && _sessions.Count > 0)
                {
                    AdvanceAll(SecondsPerTick);
                }
                yield return _wait;
            }
        }

        private void AdvanceAll(float dt)
        {
            var snapshot = _sessions.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                var session = snapshot[i];
                if (session == null || !session.isActive || session.secondsPerTurn <= 0f)
                    continue;
            
                float prevTurns = session.TurnsCooked;
                int prevFloor = Mathf.FloorToInt(prevTurns);

                session.accumulatedSeconds += dt;
            
                float newTurns = session.TurnsCooked;
                int newFloor = Mathf.FloorToInt(newTurns);

                for (int boundary = prevFloor + 1; boundary <= newFloor; boundary++)
                {
                    if (boundary <= session.maxTurnsBeforeBurn)
                    {
                        session.RaiseDonenessCrossed(boundary);
                    }
                }
            
                int burnIndex  = session.maxTurnsBeforeBurn + 1;
                bool crossedToBurn  = (prevFloor < burnIndex) && (newFloor >= burnIndex);

                if (crossedToBurn)
                {
                    session.RaiseBurnt();
                }
            }
        }
    }
}
