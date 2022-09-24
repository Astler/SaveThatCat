using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Line
{
    [RequireComponent(typeof(Rigidbody2D), typeof(LineRenderer), typeof(PolygonCollider2D))]
    public class LineView : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private PolygonCollider2D _polygonCollider2D;
        private Rigidbody2D _rigidbody2D;

        public void UpdateLine(List<Vector2> drawPoints)
        {
            _lineRenderer.positionCount = drawPoints.Count;
            _lineRenderer.SetPositions(drawPoints.Select(it => new Vector3(it.x, it.y, 0f)).ToArray());
        }

        public void EnablePhysicsSimulation()
        {
            Mesh lineBakedMesh = new();
            _lineRenderer.BakeMesh(lineBakedMesh, true);

            int[] triangles = lineBakedMesh.triangles;
            Vector3[] vertices = lineBakedMesh.vertices;

            Dictionary<string, KeyValuePair<int, int>> edges = new();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                for (int e = 0; e < 3; e++)
                {
                    int vert1 = triangles[i + e];
                    int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                    string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);

                    if (edges.ContainsKey(edge))
                    {
                        edges.Remove(edge);
                    }
                    else
                    {
                        edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                    }
                }
            }

            Dictionary<int, int> lookup = new();
            
            foreach (KeyValuePair<int, int> edge in edges.Values
                         .Where(edge => lookup.ContainsKey(edge.Key) == false))
            {
                lookup.Add(edge.Key, edge.Value);
            }

            _polygonCollider2D.pathCount = 0;

            int startVert = 0;
            int nextVert = startVert;
            int highestVert = startVert;

            List<Vector2> colliderPath = new();

            while (true)
            {
                colliderPath.Add(vertices[nextVert]);
                nextVert = lookup[nextVert];

                if (nextVert > highestVert)
                {
                    highestVert = nextVert;
                }

                if (nextVert == startVert)
                {
                    _polygonCollider2D.pathCount++;
                    _polygonCollider2D.SetPath(_polygonCollider2D.pathCount - 1, colliderPath.ToArray());
                    colliderPath.Clear();

                    if (lookup.ContainsKey(highestVert + 1))
                    {
                        startVert = highestVert + 1;
                        nextVert = startVert;

                        continue;
                    }

                    break;
                }
            }

            _rigidbody2D.simulated = true;
            _rigidbody2D.velocity = Vector2.zero;
        }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
            _rigidbody2D.simulated = false;
        }
    }
}