using System.Numerics;

namespace BezierSurface
{
    /// <summary>
    /// Triangle filler using vertex sorting algorithm (for surnames L-Z)
    /// </summary>
    public class TriangleFiller
    {
        private LightingModel lighting;
        private Bitmap texture;
        private Bitmap normalMap;
        private bool useTexture;
        private bool useNormalMap;
        private Vector3 solidColor;

        public TriangleFiller(LightingModel lighting)
        {
            this.lighting = lighting;
            solidColor = new Vector3(1, 0.5f, 0); // Default orange
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

        /// <summary>
        /// Fill triangle using vertex sorting algorithm
        /// </summary>
        public unsafe void FillTriangle(Triangle triangle, Bitmap buffer)
        {
            // Get 2D projected vertices
            var v1 = triangle.V1.PTransformed;
            var v2 = triangle.V2.PTransformed;
            var v3 = triangle.V3.PTransformed;

            // Sort vertices by Y coordinate
            Vertex[] vertices = { triangle.V1, triangle.V2, triangle.V3 };
            Vector3[] positions = { v1, v2, v3 };

            // Bubble sort by Y
            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 3; j++)
                {
                    if (positions[i].Y > positions[j].Y)
                    {
                        (positions[i], positions[j]) = (positions[j], positions[i]);
                        (vertices[i], vertices[j]) = (vertices[j], vertices[i]);
                    }
                }
            }

            var p1 = positions[0];
            var p2 = positions[1];
            var p3 = positions[2];

            var vt1 = vertices[0];
            var vt2 = vertices[1];
            var vt3 = vertices[2];

            // Convert to screen coordinates
            int width = buffer.Width;
            int height = buffer.Height;
            float centerX = width / 2.0f;
            float centerY = height / 2.0f;

            // Check if triangle is degenerate
            if (Math.Abs(p3.Y - p1.Y) < 0.001f)
                return;

            System.Drawing.Imaging.BitmapData bitmapData = buffer.LockBits(
                new Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                int* ptr = (int*)bitmapData.Scan0;
                int stride = bitmapData.Stride / 4;

                // Fill triangle
                int yStart = (int)Math.Max(0, Math.Ceiling(p1.Y + centerY));
                int yEnd = (int)Math.Min(height - 1, Math.Floor(p3.Y + centerY));

                for (int y = yStart; y <= yEnd; y++)
                {
                    float yf = y - centerY;

                    // Find left and right edges
                    float xLeft, xRight;
                    Vertex vLeft, vRight;

                    // Long edge (p1 -> p3)
                    float tLong = (yf - p1.Y) / (p3.Y - p1.Y);
                    float xLong = p1.X + tLong * (p3.X - p1.X);
                    Vertex vLong = InterpolateVertex(vt1, vt3, tLong);

                    // Short edge
                    float xShort;
                    Vertex vShort;

                    if (yf < p2.Y)
                    {
                        // Upper part (p1 -> p2)
                        if (Math.Abs(p2.Y - p1.Y) < 0.001f)
                            continue;
                        
                        float tShort = (yf - p1.Y) / (p2.Y - p1.Y);
                        xShort = p1.X + tShort * (p2.X - p1.X);
                        vShort = InterpolateVertex(vt1, vt2, tShort);
                    }
                    else
                    {
                        // Lower part (p2 -> p3)
                        if (Math.Abs(p3.Y - p2.Y) < 0.001f)
                            continue;

                        float tShort = (yf - p2.Y) / (p3.Y - p2.Y);
                        xShort = p2.X + tShort * (p3.X - p2.X);
                        vShort = InterpolateVertex(vt2, vt3, tShort);
                    }

                    // Determine left and right
                    if (xLong < xShort)
                    {
                        xLeft = xLong;
                        xRight = xShort;
                        vLeft = vLong;
                        vRight = vShort;
                    }
                    else
                    {
                        xLeft = xShort;
                        xRight = xLong;
                        vLeft = vShort;
                        vRight = vLong;
                    }

                    int xStart = (int)Math.Max(0, Math.Ceiling(xLeft + centerX));
                    int xEnd = (int)Math.Min(width - 1, Math.Floor(xRight + centerX));

                    for (int x = xStart; x <= xEnd; x++)
                    {
                        float xf = x - centerX;

                        // Interpolate along scanline
                        float t = 0;
                        if (Math.Abs(xRight - xLeft) > 0.001f)
                            t = (xf - xLeft) / (xRight - xLeft);

                        Vertex vPixel = InterpolateVertex(vLeft, vRight, t);

                        // Get object color
                        Vector3 objectColor;
                        if (useTexture && texture != null)
                        {
                            objectColor = GetTextureColor(texture, vPixel.U, vPixel.V);
                        }
                        else
                        {
                            objectColor = solidColor;
                        }

                        // Calculate lighting
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
                    }
                }
            }
            finally
            {
                buffer.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// Linear interpolation between two vertices
        /// </summary>
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

            // Normalize interpolated normal
            if (result.NTransformed.LengthSquared() > 0)
                result.NTransformed = Vector3.Normalize(result.NTransformed);

            if (result.PuTransformed.LengthSquared() > 0)
                result.PuTransformed = Vector3.Normalize(result.PuTransformed);

            if (result.PvTransformed.LengthSquared() > 0)
                result.PvTransformed = Vector3.Normalize(result.PvTransformed);

            return result;
        }

        /// <summary>
        /// Get color from texture at UV coordinates
        /// </summary>
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
