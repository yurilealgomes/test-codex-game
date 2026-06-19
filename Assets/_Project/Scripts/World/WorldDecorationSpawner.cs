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

        public void Populate(WorldChunk chunk, WorldChunkData data)
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
            PopulateBreakables(chunk, data, random);
        }

        private void PopulateBreakables(WorldChunk chunk, WorldChunkData data, System.Random random)
        {
            if (data.BreakableObjects == null || data.BreakableObjects.Length == 0)
            {
                chunk.HideBreakablesFrom(0);
                return;
            }

            for (int i = 0; i < data.BreakablesPerChunk; i++)
            {
                GameObject breakableObject = chunk.GetBreakable(i);
                breakableObject.SetActive(true);

                BreakableObjectData objectData = data.BreakableObjects[random.Next(0, data.BreakableObjects.Length)];
                float x = ((float)random.NextDouble() - 0.5f) * chunk.Size * 0.82f;
                float z = ((float)random.NextDouble() - 0.5f) * chunk.Size * 0.82f;

                breakableObject.transform.localPosition = new Vector3(x, 0.48f, z);
                breakableObject.transform.rotation = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f);
                breakableSpawner.Configure(breakableObject, objectData, random);
            }

            chunk.HideBreakablesFrom(data.BreakablesPerChunk);
        }
    }
}
