using System.Numerics;

namespace BezierSurface
{
    public class BezierSurface
    {
        private Vector3[,] controlPoints = new Vector3[4, 4];

        public Vector3[,] ControlPoints => controlPoints;

        public static BezierSurface LoadFromFile(string filePath)
        {
            var surface = new BezierSurface();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length < 16)
                throw new InvalidDataException("File must contain 16 control points");

            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var parts = lines[index].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 3)
                        throw new InvalidDataException($"Invalid line {index + 1}");

                    float x = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);

                    surface.controlPoints[i, j] = new Vector3(x, y, z);
                    index++;
                }
            }

            return surface;
        }

        private static float Bernstein(int i, float t)
        {
            float[] B = new float[4];
            float t1 = 1 - t;

            B[0] = t1 * t1 * t1;
            B[1] = 3 * t * t1 * t1;
            B[2] = 3 * t * t * t1;
            B[3] = t * t * t;

            return B[i];
        }

        private static float BernsteinDerivative(int i, float t)
        {
            float[] dB = new float[4];
            float t1 = 1 - t;

            dB[0] = -3 * t1 * t1;
            dB[1] = 3 * t1 * t1 - 6 * t * t1;
            dB[2] = 6 * t * t1 - 3 * t * t;
            dB[3] = 3 * t * t;

            return dB[i];
        }

        public Vector3 Evaluate(float u, float v)
        {
            Vector3 point = Vector3.Zero;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    point += controlPoints[i, j] * Bernstein(i, u) * Bernstein(j, v);
                }
            }

            return point;
        }

        public Vector3 EvaluateTangentU(float u, float v)
        {
            Vector3 tangent = Vector3.Zero;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tangent += controlPoints[i, j] * BernsteinDerivative(i, u) * Bernstein(j, v);
                }
            }

            return tangent;
        }

        public Vector3 EvaluateTangentV(float u, float v)
        {
            Vector3 tangent = Vector3.Zero;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tangent += controlPoints[i, j] * Bernstein(i, u) * BernsteinDerivative(j, v);
                }
            }

            return tangent;
        }

        public Vector3 EvaluateNormal(float u, float v)
        {
            Vector3 Pu = EvaluateTangentU(u, v);
            Vector3 Pv = EvaluateTangentV(u, v);
            Vector3 normal = Vector3.Cross(Pu, Pv);
            
            if (normal.LengthSquared() > 0)
                return Vector3.Normalize(normal);
            
            return new Vector3(0, 0, 1);
        }
    }
}
