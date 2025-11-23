using System.Numerics;

namespace BezierSurface
{
    public class TriangleMesh
    {
        public List<Triangle> Triangles { get; private set; } = new List<Triangle>();
        public Vertex[,] Vertices { get; private set; }

        public void Generate(BezierSurface surface, int divisions)
        {
            Triangles.Clear();

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

            for (int i = 0; i < divisions; i++)
            {
                for (int j = 0; j < divisions; j++)
                {
                    Triangles.Add(new Triangle(
                        Vertices[i, j],
                        Vertices[i + 1, j],
                        Vertices[i, j + 1]
                    ));

                    Triangles.Add(new Triangle(
                        Vertices[i + 1, j],
                        Vertices[i + 1, j + 1],
                        Vertices[i, j + 1]
                    ));
                }
            }
        }

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

        public List<Vector3> GetControlPolygon(BezierSurface surface)
        {
            var points = new List<Vector3>();

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
