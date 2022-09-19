using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Line
{
    [RequireComponent(typeof(Rigidbody2D), typeof(LineRenderer))]
    public class LineView : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
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

            // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
            Dictionary<int, int> lookup = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> edge in edges.Values)
            {
                if (lookup.ContainsKey(edge.Key) == false)
                {
                    lookup.Add(edge.Key, edge.Value);
                }
            }

            // Create empty polygon collider
            PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            polygonCollider.pathCount = 0;

            // Loop through edge vertices in order
            int startVert = 0;
            int nextVert = startVert;
            int highestVert = startVert;
            List<Vector2> colliderPath = new List<Vector2>();
            while (true)
            {
                // Add vertex to collider path
                colliderPath.Add(vertices[nextVert]);

                // Get next vertex
                nextVert = lookup[nextVert];

                // Store highest vertex (to know what shape to move to next)
                if (nextVert > highestVert)
                {
                    highestVert = nextVert;
                }

                // Shape complete
                if (nextVert == startVert)
                {
                    // Add path to polygon collider
                    polygonCollider.pathCount++;
                    polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                    colliderPath.Clear();

                    // Go to next shape if one exists
                    if (lookup.ContainsKey(highestVert + 1))
                    {
                        // Set starting and next vertices
                        startVert = highestVert + 1;
                        nextVert = startVert;

                        // Continue to next loop
                        continue;
                    }

                    // No more verts
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
            _rigidbody2D.simulated = false;
        }
    }
}