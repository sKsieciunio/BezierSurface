using System.Numerics;

namespace BezierSurface
{
    /// <summary>
    /// Generates triangle mesh from Bézier surface
    /// </summary>
    public class TriangleMesh
    {
        public List<Triangle> Triangles { get; private set; } = new List<Triangle>();
        public Vertex[,] Vertices { get; private set; }

        /// <summary>
        /// Generate mesh by triangulating the Bézier surface
        /// </summary>
        public void Generate(BezierSurface surface, int divisions)
        {
            Triangles.Clear();

            // Create grid of vertices
            int gridSize = divisions + 1;
            Vertices = new Vertex[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    float u = i / (float)divisions;
                    float v = j / (float)divisions;
                    Vertices[i, j] = new Vertex(u, v, surface);
                }
            }

            // Create triangles
            for (int i = 0; i < divisions; i++)
            {
                for (int j = 0; j < divisions; j++)
                {
                    // Two triangles per quad
                    // Triangle 1
                    Triangles.Add(new Triangle(
                        Vertices[i, j],
                        Vertices[i + 1, j],
                        Vertices[i, j + 1]
                    ));

                    // Triangle 2
                    Triangles.Add(new Triangle(
                        Vertices[i + 1, j],
                        Vertices[i + 1, j + 1],
                        Vertices[i, j + 1]
                    ));
                }
            }
        }

        /// <summary>
        /// Transform all vertices in the mesh
        /// </summary>
        public void Transform(float alpha, float beta)
        {
            if (Vertices == null) return;

            for (int i = 0; i < Vertices.GetLength(0); i++)
            {
                for (int j = 0; j < Vertices.GetLength(1); j++)
                {
                    Vertices[i, j].Transform(alpha, beta);
                }
            }
        }

        /// <summary>
        /// Get control points for drawing the Bézier polygon
        /// </summary>
        public List<Vector3> GetControlPolygon(BezierSurface surface)
        {
            var points = new List<Vector3>();

            // Add control points in grid order
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    points.Add(surface.ControlPoints[i, j]);
                }
            }

            return points;
        }
    }
}
