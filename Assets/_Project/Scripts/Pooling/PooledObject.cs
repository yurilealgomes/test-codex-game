using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PooledObject : MonoBehaviour
    {
        public string PoolKey { get; private set; }
        public ObjectPool OriginPool { get; private set; }

        public void Bind(ObjectPool originPool, string poolKey)
        {
            OriginPool = originPool;
            PoolKey = poolKey;
        }

        public void Despawn()
        {
            if (OriginPool != null)
            {
                OriginPool.Despawn(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
