using UnityEngine;
using System.Collections.Generic;

namespace Characters.Clients.Spawner
{
    public static class ClientSpawnpointRegistry
    {
        private static readonly Dictionary<int, ClientSpawnpoint> Map = new();
        
#if UNITY_EDITOR
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => Map.Clear();
#endif
        
        public static bool Register(ClientSpawnpoint spawnpoint)
        {
            if (spawnpoint == null) return false;

            // Creamos un error si pusimos dos IDs iguales
            if (Map.TryGetValue(spawnpoint.ClientId, out var existing) && existing != null && existing != spawnpoint)
            {
                Debug.LogError($"[ClientSpawnpointRegistry] Duplicate spawnpoint ID {spawnpoint.ClientId} on '{spawnpoint.name}'. "
                               + $"Already registered by '{existing.name}'. Keeping the first one.");
                return false;
            }

            Map[spawnpoint.ClientId] = spawnpoint;
            return true;
        }
        
        public static void Unregister(ClientSpawnpoint spawnpoint)
        {
            if (spawnpoint == null) return;

            if (Map.TryGetValue(spawnpoint.ClientId, out var existing) && existing == spawnpoint)
                Map.Remove(spawnpoint.ClientId);
        }
        
        public static bool TryGet(int id, out ClientSpawnpoint sp) => Map.TryGetValue(id, out sp);
        
        public static IEnumerable<ClientSpawnpoint> GetSpawnpoints()
        {
            foreach (var key in Map) yield return key.Value;
        }
    }
}
