using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class WorldDecorationSpawner : MonoBehaviour
    {
        private BreakableObjectSpawner breakableSpawner;

        private void Awake()
        {
            breakableSpawner = GetComponent<BreakableObjectSpawner>();
            if (breakableSpawner == null)
            {
                breakableSpawner = gameObject.AddComponent<BreakableObjectSpawner>();
            }
        }

        public void Populate(WorldChunk chunk, WorldChunkData data, InfiniteWorldManager worldManager)
        {
            if (chunk == null || data == null)
            {
                return;
            }

            int seed = chunk.Coordinate.x * 73856093 ^ chunk.Coordinate.y * 19349663;
            System.Random random = new System.Random(seed);

            for (int i = 0; i < data.DecorationsPerChunk; i++)
            {
                GameObject decoration = chunk.GetDecoration(i);
                decoration.SetActive(true);

                float x = ((float)random.NextDouble() - 0.5f) * chunk.Size * 0.78f;
                float z = ((float)random.NextDouble() - 0.5f) * chunk.Size * 0.78f;
                float height = 0.4f + (float)random.NextDouble() * 1.4f;
                float width = 0.25f + (float)random.NextDouble() * 0.55f;

                decoration.transform.localPosition = new Vector3(x, height * 0.5f, z);
                decoration.transform.localScale = new Vector3(width, height, width);
                decoration.transform.rotation = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f);

                Renderer renderer = decoration.GetComponent<Renderer>();
                if (renderer != null)
                {
                    float tint = 0.85f + (float)random.NextDouble() * 0.3f;
                    renderer.material.color = data.DecorationColor * tint;
                }
            }

            chunk.HideDecorationsFrom(data.DecorationsPerChunk);
            PopulateBreakables(chunk, data, random, worldManager);
        }

        private void PopulateBreakables(WorldChunk chunk, WorldChunkData data, System.Random random, InfiniteWorldManager worldManager)
        {
            if (data.BreakableObjects == null || data.BreakableObjects.Length == 0)
            {
                chunk.HideBreakablesFrom(0);
                return;
            }

            for (int i = 0; i < data.BreakablesPerChunk; i++)
            {
                GameObject breakableObject = chunk.GetBreakable(i);
                string breakableKey = worldManager != null ? worldManager.GetBreakableKey(chunk.Coordinate, i) : chunk.Coordinate.x + ":" + chunk.Coordinate.y + ":" + i;
                if (worldManager != null && worldManager.IsBreakableBroken(chunk.Coordinate, i))
                {
                    breakableObject.SetActive(false);
                    continue;
                }

                InfiniteWorldManager.BreakablePlacement placement;
                bool validPosition = false;
                if (worldManager != null && worldManager.TryGetBreakablePlacement(breakableKey, out placement))
                {
                    validPosition = IsPlacementValidForData(placement, data);
                }
                else
                {
                    placement = CreatePlacement(chunk, data, random, worldManager, out validPosition);
                    if (validPosition && worldManager != null)
                    {
                        worldManager.StoreBreakablePlacement(breakableKey, placement);
                    }
                }

                if (!validPosition)
                {
                    breakableObject.SetActive(false);
                    continue;
                }

                BreakableObjectData objectData = data.BreakableObjects[placement.DataIndex];
                breakableObject.SetActive(true);
                breakableObject.transform.localPosition = placement.LocalPosition;
                breakableObject.transform.rotation = Quaternion.Euler(0f, placement.Yaw, 0f);
                breakableSpawner.Configure(breakableObject, objectData, placement.ScaleRoll, placement.Tint, breakableKey, worldManager);
            }

            chunk.HideBreakablesFrom(data.BreakablesPerChunk);
        }

        private InfiniteWorldManager.BreakablePlacement CreatePlacement(WorldChunk chunk, WorldChunkData data, System.Random random, InfiniteWorldManager worldManager, out bool validPosition)
        {
            InfiniteWorldManager.BreakablePlacement placement = new InfiniteWorldManager.BreakablePlacement
            {
                DataIndex = random.Next(0, data.BreakableObjects.Length),
                ScaleRoll = (float)random.NextDouble(),
                Tint = 0.85f + (float)random.NextDouble() * 0.3f,
                Yaw = (float)random.NextDouble() * 360f
            };

            validPosition = false;
            int attempts = Mathf.Max(1, data.BreakablePlacementAttempts);
            for (int attempt = 0; attempt < attempts; attempt++)
            {
                float x = ((float)random.NextDouble() - 0.5f) * chunk.Size * 0.82f;
                float z = ((float)random.NextDouble() - 0.5f) * chunk.Size * 0.82f;
                placement.LocalPosition = new Vector3(x, 0.48f, z);
                Vector3 worldPosition = chunk.transform.TransformPoint(placement.LocalPosition);
                if (worldManager == null || worldManager.IsSafeBreakablePosition(worldPosition, data))
                {
                    validPosition = true;
                    break;
                }
            }

            return placement;
        }

        private bool IsPlacementValidForData(InfiniteWorldManager.BreakablePlacement placement, WorldChunkData data)
        {
            return data != null
                && data.BreakableObjects != null
                && placement.DataIndex >= 0
                && placement.DataIndex < data.BreakableObjects.Length;
        }
    }
}
