using System.Numerics;

namespace BezierSurface
{
    public class LightingModel
    {
        public float Kd { get; set; } = 0.5f;  
        public float Ks { get; set; } = 0.5f;  
        public int M { get; set; } = 50;       

        public Vector3 LightColor { get; set; } = Vector3.One; 
        public Vector3 LightDirection { get; set; } = new Vector3(0, 0, 1);

        public Vector3 CalculateColor(Vector3 normal, Vector3 objectColor, Bitmap? normalMap = null, 
            Vertex? vertex = null, Vector3? tangentPu = null, Vector3? tangentPv = null)
        {
            Vector3 N = normal;

            if (normalMap != null && vertex != null && tangentPu.HasValue && tangentPv.HasValue)
            {
                N = ApplyNormalMapping(normal, tangentPu.Value, tangentPv.Value, normalMap, vertex.U, vertex.V);
            }

            if (N.LengthSquared() > 0)
                N = Vector3.Normalize(N);

            Vector3 L = Vector3.Normalize(LightDirection);
            Vector3 V = new Vector3(0, 0, 1);

            float NdotL = Vector3.Dot(N, L);
            Vector3 R = 2 * NdotL * N - L;
            if (R.LengthSquared() > 0)
                R = Vector3.Normalize(R);

            float kd = Kd;
            float ks = Ks;
            float sum = kd + ks;
            if (sum > 1.0f)
            {
                float invSum = 1.0f / sum;
                kd *= invSum;
                ks *= invSum;
            }

            Vector3 colorMixed = LightColor * objectColor;

            float diffuse = Math.Max(0, NdotL);
            Vector3 diffuseColor = kd * colorMixed * diffuse;

            float VdotR = Vector3.Dot(V, R);
            float specular = (float)Math.Pow(Math.Max(0, VdotR), M);
            Vector3 specularColor = ks * colorMixed * specular;

            Vector3 finalColor = diffuseColor + specularColor;

            finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);

            return finalColor;
        }

        private Vector3 ApplyNormalMapping(Vector3 surfaceNormal, Vector3 Pu, Vector3 Pv, 
            Bitmap normalMap, float u, float v)
        {
            Vector3 normalFromMap = GetNormalFromMap(normalMap, u, v);

            Vector3 N = Vector3.Normalize(surfaceNormal);
            Vector3 T = Vector3.Normalize(-Pu);
            Vector3 B = -Vector3.Normalize(Vector3.Cross(T, N));

            Matrix4x4 TBN = new Matrix4x4(
                T.X, B.X, N.X, 0,
                T.Y, B.Y, N.Y, 0,
                T.Z, B.Z, N.Z, 0,
                0, 0, 0, 1
            );

            Vector3 transformedNormal = Vector3.TransformNormal(normalFromMap, TBN);

            if (transformedNormal.LengthSquared() > 0)
                return Vector3.Normalize(transformedNormal);

            return N;
        }

        private Vector3 GetNormalFromMap(Bitmap normalMap, float u, float v)
        {
            u = Math.Clamp(u, 0, 1);
            v = Math.Clamp(v, 0, 1);

            int x = (int)(u * (normalMap.Width - 1));
            int y = (int)(v * (normalMap.Height - 1));

            Color c = normalMap.GetPixel(x, y);

            float nx = (c.R / 255.0f) * 2.0f - 1.0f;
            float ny = (c.G / 255.0f) * 2.0f - 1.0f;
            float nz = (c.B / 255.0f) * 2.0f - 1.0f;

            Vector3 normal = new Vector3(nx, ny, nz);
            
            if (normal.LengthSquared() > 0)
                return Vector3.Normalize(normal);

            return new Vector3(0, 0, 1);
        }

        public static Color ToColor(Vector3 color)
        {
            int r = (int)Math.Clamp(color.X * 255, 0, 255);
            int g = (int)Math.Clamp(color.Y * 255, 0, 255);
            int b = (int)Math.Clamp(color.Z * 255, 0, 255);
            return Color.FromArgb(r, g, b);
        }

        public static Vector3 ToVector3(Color color)
        {
            return new Vector3(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
        }
    }
}
