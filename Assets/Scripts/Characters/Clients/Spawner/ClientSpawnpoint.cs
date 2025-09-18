using UnityEngine;

namespace Characters.Clients.Spawner
{
    public enum ClientIdleAnim
    {
        Idle, Sitting
    }
    
    public class ClientSpawnpoint : MonoBehaviour
    {
        [SerializeField] private int clientId;
        [SerializeField] private ClientIdleAnim initialIdle = ClientIdleAnim.Idle;

        public int ClientId => clientId;
        public ClientIdleAnim InitialIdle => initialIdle;
        public Transform SpawnTransform => transform;

        private void OnEnable()  => ClientSpawnpointRegistry.Register(this);
        private void OnDisable() => ClientSpawnpointRegistry.Unregister(this);
        
    }
}
