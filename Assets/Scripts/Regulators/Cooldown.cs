using UnityEngine;

namespace Managers
{
    public struct Cooldown
    {
        //Vars
        private float _nextTime;
        //Getters
        public bool IsReady => Time.time >= _nextTime;
        public float RemainingTime => Mathf.Max(0, _nextTime - Time.time);
        //Setters
        public void StartCooldown(float duration) => _nextTime = Time.time + duration;
        public void ResetCooldown() => _nextTime = Time.time;
        public void AddTime(float delta) => _nextTime = _nextTime + delta;
        public void SubtractTime(float delta) => _nextTime = _nextTime - delta;
    }
}