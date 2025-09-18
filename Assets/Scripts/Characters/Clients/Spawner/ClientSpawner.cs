using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Clients.Spawner
{
    public sealed class ClientSpawner : MonoBehaviour
    {
        [Serializable]
        public class ClientSlot
        {
            [Tooltip("ID of the spawnpoint in the scene (Should match ClientSpawnpoint.ClientId)")]
            public int spawnpointId;
            
            [Tooltip("Must contain ClientController")]
            public GameObject clientPrefab;
            
            [Tooltip("Spawn this client on Start(), false for critic")]
            public bool spawnOnStart = true;
        }
        
        [Header("Config")]
        [SerializeField] private List<ClientSlot> slots = new();
        
        // Referencias para respawn o despawn
        private readonly Dictionary<int, GameObject> _spawnedBySpawnpoint = new();

        private void Start()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var client = slots[i];
                if (client.spawnOnStart)
                    TrySpawn(client);
            }
        }

        public GameObject TrySpawn(ClientSlot slot)
        {
            #region Warnings
            if (slot == null || slot.clientPrefab == null)
            {
                Debug.LogWarning("[ClientSpawner] Slot or prefab is null.");
                return null;
            }
            
            if (!ClientSpawnpointRegistry.TryGet(slot.spawnpointId, out var sp))
            {
                Debug.LogWarning($"[ClientSpawner] No spawnpoint with id {slot.spawnpointId} found in scene.");
                return null;
            }
            
            if (_spawnedBySpawnpoint.TryGetValue(sp.ClientId, out var existing) && existing != null)
            {
                Debug.LogWarning($"[ClientSpawner] Spawnpoint {sp.ClientId} already has a client. Skipping duplicate spawn.");
                return existing;
            }
            #endregion
            
            var instance = Instantiate(slot.clientPrefab, sp.SpawnTransform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            _spawnedBySpawnpoint[sp.ClientId] = instance;
            
            TryApplyInitialIdle(instance, sp.InitialIdle);
            return instance;
        }
        
        public GameObject TrySpawnById(int spawnpointId)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].spawnpointId == spawnpointId)
                    return TrySpawn(slots[i]);
            }
            
            Debug.LogWarning($"[ClientSpawner] No slot configured for spawnpoint id {spawnpointId}.");
            return null;
        }
        
        public void Despawn(int spawnpointId)
        {
            if (_spawnedBySpawnpoint.TryGetValue(spawnpointId, out var go) && go != null)
            {
                Destroy(go);
            }
            _spawnedBySpawnpoint.Remove(spawnpointId);
        }
        
        private static void TryApplyInitialIdle(GameObject instance, ClientIdleAnim idle)
        {
            var animator = instance.GetComponent<Animator>();
            if (animator == null) return;

            // Si no funcionan las animaciones, revisar que los nombres de las anims esten igual a los enums de ClientSpawnpoint!
            animator.Play(idle.ToString(), 0, 0f);
        }
    }
}
