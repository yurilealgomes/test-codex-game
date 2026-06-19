using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class InfiniteWorldManager : MonoBehaviour
    {
        [SerializeField] private float chunkSize = 24f;
        [SerializeField] private int activeRadius = 2;

        private readonly List<WorldChunk> chunks = new List<WorldChunk>();
        private readonly HashSet<string> brokenBreakables = new HashSet<string>();
        private readonly Dictionary<string, BreakablePlacement> breakablePlacements = new Dictionary<string, BreakablePlacement>();
        private Transform player;
        private Vector2Int currentCenter;
        private WorldChunkData chunkData;
        private WorldDecorationSpawner decorationSpawner;
        private Camera mainCamera;

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

            mainCamera = Camera.main;

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
                    decorationSpawner.Populate(chunk, chunkData, this);
                }
            }
        }

        public bool IsBreakableBroken(Vector2Int coordinate, int slotIndex)
        {
            return brokenBreakables.Contains(GetBreakableKey(coordinate, slotIndex));
        }

        public void MarkBreakableBroken(string breakableKey)
        {
            if (!string.IsNullOrEmpty(breakableKey))
            {
                brokenBreakables.Add(breakableKey);
            }
        }

        public bool TryGetBreakablePlacement(string breakableKey, out BreakablePlacement placement)
        {
            if (string.IsNullOrEmpty(breakableKey))
            {
                placement = default(BreakablePlacement);
                return false;
            }

            return breakablePlacements.TryGetValue(breakableKey, out placement);
        }

        public void StoreBreakablePlacement(string breakableKey, BreakablePlacement placement)
        {
            if (!string.IsNullOrEmpty(breakableKey))
            {
                breakablePlacements[breakableKey] = placement;
            }
        }

        public string GetBreakableKey(Vector2Int coordinate, int slotIndex)
        {
            return coordinate.x + ":" + coordinate.y + ":" + slotIndex;
        }

        public bool IsSafeBreakablePosition(Vector3 worldPosition, WorldChunkData data)
        {
            if (player != null)
            {
                float minDistance = data != null ? data.MinBreakableDistanceFromPlayer : 18f;
                if (MathUtils.DistanceXZ(worldPosition, player.position) < minDistance)
                {
                    return false;
                }
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera != null && data != null)
            {
                Vector3 viewport = mainCamera.WorldToViewportPoint(worldPosition + Vector3.up * 0.8f);
                float margin = data.CameraBreakableMargin;
                if (viewport.z > 0f && viewport.x > -margin && viewport.x < 1f + margin && viewport.y > -margin && viewport.y < 1f + margin)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            DebugGodModeController debugTools;
            if (!Application.isPlaying || !ServiceLocator.TryGet(out debugTools) || !debugTools.ChunkDebugEnabled)
            {
                return;
            }

            Gizmos.color = new Color(0.4f, 1f, 0.45f, 0.35f);
            for (int i = 0; i < chunks.Count; i++)
            {
                WorldChunk chunk = chunks[i];
                if (chunk != null)
                {
                    Gizmos.DrawWireCube(chunk.transform.position, new Vector3(chunk.Size, 0.2f, chunk.Size));
                }
            }
        }

        public struct BreakablePlacement
        {
            public int DataIndex;
            public Vector3 LocalPosition;
            public float Yaw;
            public float ScaleRoll;
            public float Tint;
        }
    }
}
