using System;
using System.Collections.Generic;
using Characters.Clients;
using Interfaces;

namespace Progress
{
    // Each critic rewards one Golden Nova, once: serving the same critic again (e.g. after returning to a planet) adds nothing.
    public sealed class NovaProgress : INovaProgress
    {
        private readonly HashSet<ClientSO> _rewardingCritics = new();
        private int _debugOffset;

        public int Novas => Math.Max(0, _rewardingCritics.Count + _debugOffset);

        public event Action<int> Changed;

        public bool HasNovaFrom(ClientSO critic) => critic != null && _rewardingCritics.Contains(critic);

        public bool TryAward(ClientSO critic)
        {
            if (critic == null || !critic.isCritic || !_rewardingCritics.Add(critic)) return false;

            Changed?.Invoke(Novas);
            return true;
        }

        // Debug only: shifts the total without touching which critics were actually served.
        public void SetDebugNovas(int novas)
        {
            _debugOffset = Math.Max(0, novas) - _rewardingCritics.Count;
            Changed?.Invoke(Novas);
        }
    }
}
