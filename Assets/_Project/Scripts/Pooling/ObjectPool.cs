using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class ObjectPool
    {
        private readonly string key;
        private readonly GameObject prefab;
        private readonly Queue<GameObject> available = new Queue<GameObject>();
        private readonly Transform parent;
        private readonly int maxSize;
        private int createdCount;

        public int ActiveCount { get; private set; }

        public ObjectPool(string key, GameObject prefab, int initialSize, int maxSize, Transform parent)
        {
            this.key = key;
            this.prefab = prefab;
            this.maxSize = Mathf.Max(initialSize, maxSize);
            this.parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                available.Enqueue(CreateInstance());
            }
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject instance = available.Count > 0 ? available.Dequeue() : CreateInstance();
            if (instance == null)
            {
                return null;
            }

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            ActiveCount++;

            IPoolable[] poolables = instance.GetComponentsInChildren<IPoolable>(true);
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnSpawnedFromPool();
            }

            return instance;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null || !instance.activeSelf)
            {
                return;
            }

            IPoolable[] poolables = instance.GetComponentsInChildren<IPoolable>(true);
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnReturnedToPool();
            }

            instance.transform.SetParent(parent, true);
            instance.SetActive(false);
            ActiveCount = Mathf.Max(0, ActiveCount - 1);
            available.Enqueue(instance);
        }

        private GameObject CreateInstance()
        {
            if (createdCount >= maxSize)
            {
                return null;
            }

            GameObject instance = Object.Instantiate(prefab, parent);
            instance.name = key + "_Pooled";
            instance.SetActive(false);

            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            if (pooledObject == null)
            {
                pooledObject = instance.AddComponent<PooledObject>();
            }

            pooledObject.Bind(this, key);
            createdCount++;
            return instance;
        }
    }
}
