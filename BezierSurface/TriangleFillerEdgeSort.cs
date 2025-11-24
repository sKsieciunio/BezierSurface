using System.Numerics;

namespace BezierSurface
{
    public class TriangleFillerEdgeSort
    {
        private class Edge
        {
            public float YMin { get; set; }
            public float YMax { get; set; }
            public float XAtYMin { get; set; }
            public float DxDy { get; set; }  
            public Vertex VStart { get; set; }
            public Vertex VEnd { get; set; }
        }

        private LightingModel lighting;
        private Bitmap texture;
        private Bitmap normalMap;
        private bool useTexture;
        private bool useNormalMap;
        private Vector3 solidColor;
        private float[,] zBuffer;
        private int bufferWidth;
        private int bufferHeight;

        public TriangleFillerEdgeSort(LightingModel lighting)
        {
            this.lighting = lighting;
            solidColor = new Vector3(1, 0.5f, 0);
        }

        public void InitializeZBuffer(int width, int height)
        {
            bufferWidth = width;
            bufferHeight = height;
            zBuffer = new float[height, width];
            ClearZBuffer();
        }

        public void ClearZBuffer()
        {
            if (zBuffer == null) return;
            
            for (int y = 0; y < bufferHeight; y++)
            {
                for (int x = 0; x < bufferWidth; x++)
                {
                    zBuffer[y, x] = float.MaxValue;
                }
            }
        }

        public void SetTexture(Bitmap texture)
        {
            this.texture = texture;
        }

        public void SetNormalMap(Bitmap normalMap)
        {
            this.normalMap = normalMap;
        }

        public void SetUseTexture(bool useTexture)
        {
            this.useTexture = useTexture;
        }

        public void SetUseNormalMap(bool useNormalMap)
        {
            this.useNormalMap = useNormalMap;
        }

        public void SetSolidColor(Color color)
        {
            solidColor = LightingModel.ToVector3(color);
        }

        public unsafe void FillTriangle(Triangle triangle, Bitmap buffer)
        {
            float centerX = buffer.Width / 2.0f;
            float centerY = buffer.Height / 2.0f;

            var edges = new List<Edge>();
            CreateEdge(triangle.V1, triangle.V2, edges);
            CreateEdge(triangle.V2, triangle.V3, edges);
            CreateEdge(triangle.V3, triangle.V1, edges);

            if (edges.Count == 0) return;

            float yMin = edges.Min(e => e.YMin);
            float yMax = edges.Max(e => e.YMax);

            int scanYMin = (int)Math.Max(0, Math.Ceiling(yMin + centerY));
            int scanYMax = (int)Math.Min(buffer.Height - 1, Math.Floor(yMax + centerY));

            var edgeTable = new List<Edge>[buffer.Height];
            for (int i = 0; i < buffer.Height; i++)
                edgeTable[i] = new List<Edge>();

            foreach (var edge in edges)
            {
                int yIndex = (int)Math.Ceiling(edge.YMin + centerY);
                if (yIndex >= 0 && yIndex < buffer.Height)
                    edgeTable[yIndex].Add(edge);
            }

            System.Drawing.Imaging.BitmapData bitmapData = buffer.LockBits(
                new Rectangle(0, 0, buffer.Width, buffer.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                int* ptr = (int*)bitmapData.Scan0;
                int stride = bitmapData.Stride / 4;

                var activeEdges = new List<Edge>();

                for (int y = scanYMin; y <= scanYMax; y++)
                {
                    float yf = y - centerY;

                    activeEdges.AddRange(edgeTable[y]);
                    activeEdges.RemoveAll(e => yf > e.YMax);

                    if (activeEdges.Count < 2)
                        continue;

                    activeEdges.Sort((a, b) =>
                    {
                        float xa = a.XAtYMin + (yf - a.YMin) * a.DxDy;
                        float xb = b.XAtYMin + (yf - b.YMin) * b.DxDy;
                        return xa.CompareTo(xb);
                    });

                    for (int i = 0; i < activeEdges.Count - 1; i += 2)
                    {
                        var edge1 = activeEdges[i];
                        var edge2 = activeEdges[i + 1];

                        float x1 = edge1.XAtYMin + (yf - edge1.YMin) * edge1.DxDy;
                        float x2 = edge2.XAtYMin + (yf - edge2.YMin) * edge2.DxDy;

                        float t1 = Math.Abs(edge1.YMax - edge1.YMin) > 0.001f ?
                            (yf - edge1.YMin) / (edge1.YMax - edge1.YMin) : 0;
                        float t2 = Math.Abs(edge2.YMax - edge2.YMin) > 0.001f ?
                            (yf - edge2.YMin) / (edge2.YMax - edge2.YMin) : 0;

                        Vertex v1 = InterpolateVertex(edge1.VStart, edge1.VEnd, t1);
                        Vertex v2 = InterpolateVertex(edge2.VStart, edge2.VEnd, t2);

                        int xStart = (int)Math.Max(0, Math.Ceiling(x1 + centerX));
                        int xEnd = (int)Math.Min(buffer.Width - 1, Math.Floor(x2 + centerX));

                        for (int x = xStart; x <= xEnd; x++)
                        {
                            float xf = x - centerX;
                            float t = Math.Abs(x2 - x1) > 0.001f ? (xf - x1) / (x2 - x1) : 0;

                            Vertex vPixel = InterpolateVertex(v1, v2, t);

                            float z = vPixel.PTransformed.Z;
                            if (zBuffer != null && z >= zBuffer[y, x])
                            {
                                continue; 
                            }

                            Vector3 objectColor;
                            if (useTexture && texture != null)
                            {
                                objectColor = GetTextureColor(texture, vPixel.U, vPixel.V);
                            }
                            else
                            {
                                objectColor = solidColor;
                            }

                            Bitmap? normalMapToUse = useNormalMap ? normalMap : null;
                            Vector3 color = lighting.CalculateColor(
                                vPixel.NTransformed,
                                objectColor,
                                normalMapToUse,
                                vPixel,
                                vPixel.PuTransformed,
                                vPixel.PvTransformed);

                            Color finalColor = LightingModel.ToColor(color);
                            ptr[y * stride + x] = finalColor.ToArgb();

                            if (zBuffer != null)
                            {
                                zBuffer[y, x] = z;
                            }
                        }
                    }
                }
            }
            finally
            {
                buffer.UnlockBits(bitmapData);
            }
        }

        private void CreateEdge(Vertex v1, Vertex v2, List<Edge> edges)
        {
            var p1 = v1.PTransformed;
            var p2 = v2.PTransformed;

            if (Math.Abs(p2.Y - p1.Y) < 0.001f)
                return; 

            var edge = new Edge();

            if (p1.Y < p2.Y)
            {
                edge.YMin = p1.Y;
                edge.YMax = p2.Y;
                edge.XAtYMin = p1.X;
                edge.VStart = v1;
                edge.VEnd = v2;
            }
            else
            {
                edge.YMin = p2.Y;
                edge.YMax = p1.Y;
                edge.XAtYMin = p2.X;
                edge.VStart = v2;
                edge.VEnd = v1;
            }

            edge.DxDy = (p2.X - p1.X) / (p2.Y - p1.Y);

            edges.Add(edge);
        }

        private Vertex InterpolateVertex(Vertex v1, Vertex v2, float t)
        {
            t = Math.Clamp(t, 0, 1);

            var result = new Vertex()
            {
                U = v1.U + t * (v2.U - v1.U),
                V = v1.V + t * (v2.V - v1.V),
                P = Vector3.Lerp(v1.P, v2.P, t),
                Pu = Vector3.Lerp(v1.Pu, v2.Pu, t),
                Pv = Vector3.Lerp(v1.Pv, v2.Pv, t),
                N = Vector3.Lerp(v1.N, v2.N, t),
                PTransformed = Vector3.Lerp(v1.PTransformed, v2.PTransformed, t),
                PuTransformed = Vector3.Lerp(v1.PuTransformed, v2.PuTransformed, t),
                PvTransformed = Vector3.Lerp(v1.PvTransformed, v2.PvTransformed, t),
                NTransformed = Vector3.Lerp(v1.NTransformed, v2.NTransformed, t)
            };

            if (result.NTransformed.LengthSquared() > 0)
                result.NTransformed = Vector3.Normalize(result.NTransformed);

            if (result.PuTransformed.LengthSquared() > 0)
                result.PuTransformed = Vector3.Normalize(result.PuTransformed);

            if (result.PvTransformed.LengthSquared() > 0)
                result.PvTransformed = Vector3.Normalize(result.PvTransformed);

            return result;
        }

        private Vector3 GetTextureColor(Bitmap texture, float u, float v)
        {
            u = Math.Clamp(u, 0, 1);
            v = Math.Clamp(v, 0, 1);

            int x = (int)(u * (texture.Width - 1));
            int y = (int)(v * (texture.Height - 1));

            Color c = texture.GetPixel(x, y);
            return LightingModel.ToVector3(c);
        }
    }
}
