using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PoolManager : MonoBehaviour
    {
        private readonly Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void RegisterPool(string key, GameObject prefab, int initialSize, int maxSize)
        {
            if (pools.ContainsKey(key))
            {
                return;
            }

            Transform poolRoot = new GameObject(key + " Pool").transform;
            poolRoot.SetParent(transform);
            pools.Add(key, new ObjectPool(key, prefab, initialSize, maxSize, poolRoot));
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation)
        {
            ObjectPool pool;
            if (!pools.TryGetValue(key, out pool))
            {
                Debug.LogWarning("Missing pool: " + key);
                return null;
            }

            return pool.Spawn(position, rotation);
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            if (pooledObject != null)
            {
                pooledObject.Despawn();
            }
            else
            {
                instance.SetActive(false);
            }
        }

        public int GetActiveCount(string key)
        {
            ObjectPool pool;
            return pools.TryGetValue(key, out pool) ? pool.ActiveCount : 0;
        }
    }
}
