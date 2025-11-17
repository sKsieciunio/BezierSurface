using System.Numerics;

namespace BezierSurface
{
    /// <summary>
    /// Lighting model implementation
    /// </summary>
    public class LightingModel
    {
        public float Kd { get; set; } = 0.5f;  // Diffuse coefficient
        public float Ks { get; set; } = 0.5f;  // Specular coefficient
        public int M { get; set; } = 50;       // Shininess

        public Vector3 LightColor { get; set; } = Vector3.One; // White light (R, G, B in 0-1)
        public Vector3 LightDirection { get; set; } = new Vector3(0, 0, 1);

        /// <summary>
        /// Calculate color using Phong lighting model
        /// </summary>
        public Vector3 CalculateColor(Vector3 normal, Vector3 objectColor, Bitmap? normalMap = null, 
            Vertex? vertex = null, Vector3? tangentPu = null, Vector3? tangentPv = null)
        {
            Vector3 N = normal;

            // Apply normal mapping if enabled
            if (normalMap != null && vertex != null && tangentPu.HasValue && tangentPv.HasValue)
            {
                N = ApplyNormalMapping(normal, tangentPu.Value, tangentPv.Value, normalMap, vertex.U, vertex.V);
            }

            // Ensure normal is normalized
            if (N.LengthSquared() > 0)
                N = Vector3.Normalize(N);

            // Light direction (should be normalized)
            Vector3 L = Vector3.Normalize(LightDirection);

            // View direction (always along Z axis in our case)
            Vector3 V = new Vector3(0, 0, 1);

            // Calculate reflection vector: R = 2<N,L>N - L
            float NdotL = Vector3.Dot(N, L);
            Vector3 R = 2 * NdotL * N - L;
            if (R.LengthSquared() > 0)
                R = Vector3.Normalize(R);

            // Enforce energy conservation
            float kd = Kd;
            float ks = Ks;
            float sum = kd + ks;
            if (sum > 1.0f)
            {
                float invSum = 1.0f / sum;
                kd *= invSum;
                ks *= invSum;
            }

            // Use component-wise multiplication but add some color mixing
            Vector3 colorMixed = LightColor * objectColor;

            // Diffuse component - blend light color with object color
            float diffuse = Math.Max(0, NdotL);
            Vector3 diffuseColor = kd * colorMixed * diffuse;

            // Specular component - use light color more directly for highlights
            float VdotR = Vector3.Dot(V, R);
            float specular = (float)Math.Pow(Math.Max(0, VdotR), M);
            Vector3 specularColor = ks * colorMixed * specular;

            // Final color (in 0-1 range)
            Vector3 finalColor = diffuseColor + specularColor;

            // Clamp to [0, 1]
            finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);

            return finalColor;
        }

        /// <summary>
        /// Apply normal mapping
        /// </summary>
        private Vector3 ApplyNormalMapping(Vector3 surfaceNormal, Vector3 Pu, Vector3 Pv, 
            Bitmap normalMap, float u, float v)
        {
            // Get normal from normal map
            Vector3 normalFromMap = GetNormalFromMap(normalMap, u, v);

            // Build orthonormal TBN basis
            Vector3 N = Vector3.Normalize(surfaceNormal);
            Vector3 T = Vector3.Normalize(-Pu);
            Vector3 B = -Vector3.Normalize(Vector3.Cross(T, N));

            // Create TBN matrix (tangent space to world space)
            // Matrix columns are the basis vectors: [T | B | N]
            Matrix4x4 TBN = new Matrix4x4(
                T.X, B.X, N.X, 0,
                T.Y, B.Y, N.Y, 0,
                T.Z, B.Z, N.Z, 0,
                0, 0, 0, 1
            );

            // Transform normal from tangent space to world space
            Vector3 transformedNormal = Vector3.TransformNormal(normalFromMap, TBN);

            if (transformedNormal.LengthSquared() > 0)
                return Vector3.Normalize(transformedNormal);

            return N;
        }

        /// <summary>
        /// Read normal from normal map texture
        /// </summary>
        private Vector3 GetNormalFromMap(Bitmap normalMap, float u, float v)
        {
            // Clamp UV coordinates
            u = Math.Clamp(u, 0, 1);
            v = Math.Clamp(v, 0, 1);

            // Convert to texture coordinates
            int x = (int)(u * (normalMap.Width - 1));
            int y = (int)(v * (normalMap.Height - 1));

            // Get color from normal map
            Color c = normalMap.GetPixel(x, y);

            // Convert RGB [0, 255] to normal vector [-1, 1]
            float nx = (c.R / 255.0f) * 2.0f - 1.0f;
            float ny = (c.G / 255.0f) * 2.0f - 1.0f;
            float nz = (c.B / 255.0f) * 2.0f - 1.0f;

            Vector3 normal = new Vector3(nx, ny, nz);
            
            if (normal.LengthSquared() > 0)
                return Vector3.Normalize(normal);

            return new Vector3(0, 0, 1);
        }

        /// <summary>
        /// Convert Vector3 color (0-1) to Color (0-255)
        /// </summary>
        public static Color ToColor(Vector3 color)
        {
            int r = (int)Math.Clamp(color.X * 255, 0, 255);
            int g = (int)Math.Clamp(color.Y * 255, 0, 255);
            int b = (int)Math.Clamp(color.Z * 255, 0, 255);
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Convert Color to Vector3 (0-1)
        /// </summary>
        public static Vector3 ToVector3(Color color)
        {
            return new Vector3(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
        }
    }
}
