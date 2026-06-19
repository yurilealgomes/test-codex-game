using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class WorldChunk : MonoBehaviour
    {
        private readonly List<GameObject> decorations = new List<GameObject>();
        private Renderer groundRenderer;
        private Transform decorationRoot;

        public Vector2Int Coordinate { get; private set; }
        public float Size { get; private set; }

        public void Initialize(float size)
        {
            Size = size;
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.SetParent(transform);
            ground.transform.localPosition = Vector3.zero;
            ground.transform.localScale = new Vector3(size, 0.12f, size);
            ground.layer = LayerMask.NameToLayer("World");
            groundRenderer = ground.GetComponent<Renderer>();

            decorationRoot = new GameObject("Decorations").transform;
            decorationRoot.SetParent(transform);
        }

        public void SetCoordinate(Vector2Int coordinate, WorldChunkData data)
        {
            Coordinate = coordinate;
            transform.position = new Vector3(coordinate.x * Size, -0.08f, coordinate.y * Size);

            if (groundRenderer != null && data != null)
            {
                bool alternate = Mathf.Abs(coordinate.x + coordinate.y) % 2 == 0;
                groundRenderer.material.color = alternate ? data.PrimaryGroundColor : data.SecondaryGroundColor;
            }
        }

        public Transform DecorationRoot
        {
            get { return decorationRoot; }
        }

        public GameObject GetDecoration(int index)
        {
            while (decorations.Count <= index)
            {
                GameObject decoration = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                decoration.name = "Chunk Decoration";
                decoration.transform.SetParent(decorationRoot);
                decorations.Add(decoration);
            }

            return decorations[index];
        }

        public void HideDecorationsFrom(int startIndex)
        {
            for (int i = startIndex; i < decorations.Count; i++)
            {
                decorations[i].SetActive(false);
            }
        }
    }
}
