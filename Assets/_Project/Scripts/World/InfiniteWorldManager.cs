using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class InfiniteWorldManager : MonoBehaviour
    {
        [SerializeField] private float chunkSize = 24f;
        [SerializeField] private int activeRadius = 2;

        private readonly List<WorldChunk> chunks = new List<WorldChunk>();
        private Transform player;
        private Vector2Int currentCenter;
        private WorldChunkData chunkData;
        private WorldDecorationSpawner decorationSpawner;

        private void Awake()
        {
            ServiceLocator.Register(this);
            decorationSpawner = gameObject.AddComponent<WorldDecorationSpawner>();
        }

        private void Start()
        {
            PlayerController playerController;
            if (ServiceLocator.TryGet(out playerController))
            {
                player = playerController.transform;
            }

            GameDatabase database;
            if (ServiceLocator.TryGet(out database))
            {
                chunkData = database.WorldChunkData;
            }

            CreateChunks();
            Recenter(true);
        }

        private void Update()
        {
            Recenter(false);
        }

        private void CreateChunks()
        {
            int total = (activeRadius * 2 + 1) * (activeRadius * 2 + 1);
            for (int i = 0; i < total; i++)
            {
                GameObject chunkObject = new GameObject("World Chunk");
                chunkObject.transform.SetParent(transform);
                WorldChunk chunk = chunkObject.AddComponent<WorldChunk>();
                chunk.Initialize(chunkSize);
                chunks.Add(chunk);
            }
        }

        private void Recenter(bool force)
        {
            if (player == null)
            {
                return;
            }

            Vector2Int center = new Vector2Int(
                Mathf.FloorToInt((player.position.x + chunkSize * 0.5f) / chunkSize),
                Mathf.FloorToInt((player.position.z + chunkSize * 0.5f) / chunkSize));

            if (!force && center == currentCenter)
            {
                return;
            }

            currentCenter = center;
            int index = 0;
            for (int x = -activeRadius; x <= activeRadius; x++)
            {
                for (int z = -activeRadius; z <= activeRadius; z++)
                {
                    Vector2Int coordinate = new Vector2Int(center.x + x, center.y + z);
                    WorldChunk chunk = chunks[index++];
                    chunk.SetCoordinate(coordinate, chunkData);
                    decorationSpawner.Populate(chunk, chunkData);
                }
            }
        }
    }
}
