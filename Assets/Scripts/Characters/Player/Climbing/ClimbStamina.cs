using System;
using Interfaces;
using UnityEngine;

namespace Characters.Player.Climbing
{
    [RequireComponent(typeof(PlayerClimbing))]
    public class ClimbStamina : MonoBehaviour, IStamina, IClimbGate
    {
        private const float MovingThreshold = 0.0001f;

        private PlayerClimbing _climbing;
        private ClimbSettings _settings;
        private float _lastUseTime = float.NegativeInfinity;

        public float Current { get; private set; }
        public float Max => _settings.MaxStamina;
        public bool IsExhausted { get; private set; }
        public bool IsInUse => _climbing.IsClimbing;
        public bool AllowsClimbing => Current >= GrabFloor;

        private float GrabFloor => Max * _settings.MinStaminaToGrab;

        public event Action Depleted;
        public event Action<float, float> Changed;

        private void Awake()
        {
            _climbing = GetComponent<PlayerClimbing>();
            _settings = _climbing.Settings;
            Current = _settings.MaxStamina;
        }

        // No regeneration while on a surface at all (including Climb Start and sliding); the regen delay
        // counts from letting go.
        private void Update()
        {
            if (!_climbing.IsClimbing)
            {
                Regenerate(Time.deltaTime);
                return;
            }

            _lastUseTime = Time.time;
            if (_climbing.IsInClimbStart || _climbing.IsSliding || _climbing.IsLedgeClimbing) return;
            Consume(GetDrainPerSecond() * Time.deltaTime);
        }

        public void Consume(float amount)
        {
            if (amount <= 0f) return;
            _lastUseTime = Time.time;
            if (Current <= 0f) return;

            SetCurrent(Current - amount);
            if (Current > 0f) return;

            IsExhausted = true;
            Depleted?.Invoke();
            if (!_climbing.IsClimbing) return;

            if (_climbing.SurfaceTilt < -_settings.SlideMinInclination) _climbing.StartSliding();
            else _climbing.JumpOff();
        }

        private void Regenerate(float dt)
        {
            if (Current >= Max) return;
            if (Time.time - _lastUseTime < _settings.StaminaRegenDelay) return;

            SetCurrent(Current + _settings.StaminaRegenRate * dt);
            if (IsExhausted && Current >= GrabFloor) IsExhausted = false;
        }

        private float GetDrainPerSecond()
        {
            bool moving = _climbing.ClimbInput.sqrMagnitude > MovingThreshold;
            float drain = moving ? _settings.ClimbMoveDrain : _settings.ClimbIdleDrain;
            return drain * GetTiltMultiplier(_climbing.SurfaceTilt);
        }

        // 1 on a vertical wall, blending linearly to the incline/decline multiplier at the max angles.
        private float GetTiltMultiplier(float tilt)
        {
            if (tilt < 0f)
            {
                float t = _settings.MaxInclination > 0f ? -tilt / _settings.MaxInclination : 1f;
                return Mathf.Lerp(1f, _settings.InclineDrainMultiplier, t);
            }

            float d = _settings.MaxDeclination > 0f ? tilt / _settings.MaxDeclination : 1f;
            return Mathf.Lerp(1f, _settings.DeclineDrainMultiplier, d);
        }

        private void SetCurrent(float value)
        {
            float clamped = Mathf.Clamp(value, 0f, Max);
            if (Mathf.Approximately(clamped, Current)) return;
            Current = clamped;
            Changed?.Invoke(Current, Max);
        }
    }
}
