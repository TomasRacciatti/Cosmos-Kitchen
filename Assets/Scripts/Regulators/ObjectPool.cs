using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Regulators
{
    public class PooledObjectInfo
    {
        public Transform Folder;
        public readonly Queue<GameObject> InactiveObjects = new();
    }
    
    public class ObjectPool : MonoBehaviour
    {
        [Serializable]
        private struct Spawn
        {
            public int initialSpawn;
            public GameObject prefab;
        }
        
        [SerializeField] private List<Spawn> initialObjects;
        private static readonly Dictionary<string, PooledObjectInfo> ObjectPools = new();
        private static ObjectPool _instance;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            StartCoroutine(InitialSpawnsAsync());
        }
        
        private void OnDestroy()
        {
            if (_instance != this) return;
            ObjectPools.Clear();
            _instance = null;
        }

        private static PooledObjectInfo NewPool(string poolName)
        {
            PooledObjectInfo pool = new PooledObjectInfo();
            ObjectPools[poolName] = pool;
            GameObject folder = new GameObject(poolName);
            folder.transform.SetParent(_instance.transform, false);
            pool.Folder = folder.transform;
            return pool;
        }

        private static PooledObjectInfo GetPool(string poolName)
        {
            if (!ObjectPools.TryGetValue(poolName, out PooledObjectInfo pool))
            {
                pool = NewPool(poolName);
            }
            return pool;
        }
        
        private static GameObject NewObject(GameObject prefab, Vector3 position, Quaternion rotation, bool enqueue = true)
        {
            GameObject spawnableObject = Instantiate(prefab, position, rotation);
            PooledObjectInfo pool = GetPool(prefab.name);
            if (enqueue)
            {
                pool.InactiveObjects.Enqueue(spawnableObject);
                spawnableObject.SetActive(false);
            }
            spawnableObject.transform.SetParent(pool.Folder.transform, false);
            return spawnableObject;
        }

        /* Lo dejo aca de onda
        private void InitialSpawns()
        {
            foreach (var obj in initialObjects)
            {
                for (int i = 0; i < obj.initialSpawn; i++)
                {
                    NewObject(obj.prefab, Vector3.zero, Quaternion.identity);
                }
            }
        }*/

        private IEnumerator InitialSpawnsAsync()
        {
            const float maxFrameTime = 0.005f;

            var stopwatch = new System.Diagnostics.Stopwatch();

            foreach (var obj in initialObjects)
            {
                stopwatch.Reset();
                stopwatch.Start();

                for (int i = 0; i < obj.initialSpawn; i++)
                {
                    NewObject(obj.prefab, Vector3.zero, Quaternion.identity);
                    
                    if (stopwatch.Elapsed.TotalSeconds >= maxFrameTime)
                    {
                        stopwatch.Reset();
                        yield return null;
                        stopwatch.Start();
                    }
                }

                stopwatch.Stop();
            }
        }

        public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, bool active = true)
        {
            PooledObjectInfo pool = GetPool(prefab.name);
        
            GameObject spawnableObject;

            if (pool.InactiveObjects.Count > 0)
            {
                spawnableObject = pool.InactiveObjects.Dequeue();
                spawnableObject.transform.position = position;
                spawnableObject.transform.rotation = rotation;
                spawnableObject.SetActive(active);
            }
            else
            {
                spawnableObject = NewObject(prefab, position, rotation, false);
            }
            return spawnableObject;
        }

        public static void ReturnObjectToPool(GameObject obj)
        {
            string objName = obj.name.Split('(')[0].Replace(" ", "");
            PooledObjectInfo pool = GetPool(objName);
            obj.transform.SetParent(pool.Folder.transform);
            obj.SetActive(false);
            pool.InactiveObjects.Enqueue(obj);
        }
    }
}
