using System;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class WorldDecorationSpawner : MonoBehaviour
    {
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
        }
    }
}
