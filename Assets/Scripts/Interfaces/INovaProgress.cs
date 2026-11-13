using System;
using Characters.Clients;

namespace Interfaces
{
    public interface INovaProgress
    {
        int Novas { get; }

        bool HasNovaFrom(ClientSO critic);

        // (novas)
        event Action<int> Changed;
    }
}
