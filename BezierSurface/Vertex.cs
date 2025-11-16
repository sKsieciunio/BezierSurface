using System.Numerics;

namespace BezierSurface
{
    /// <summary>
    /// Represents a vertex in the triangle mesh
    /// </summary>
    public class Vertex
    {
        // Before rotation
        public Vector3 P { get; set; }
        public Vector3 Pu { get; set; }
        public Vector3 Pv { get; set; }
        public Vector3 N { get; set; }

        // After rotation
        public Vector3 PTransformed { get; set; }
        public Vector3 PuTransformed { get; set; }
        public Vector3 PvTransformed { get; set; }
        public Vector3 NTransformed { get; set; }

        // Parameters
        public float U { get; set; }
        public float V { get; set; }

        public Vertex(float u, float v, BezierSurface surface)
        {
            U = u;
            V = v;
            P = surface.Evaluate(u, v);
            Pu = surface.EvaluateTangentU(u, v);
            Pv = surface.EvaluateTangentV(u, v);
            N = surface.EvaluateNormal(u, v);
        }

        // Constructor for interpolated vertices
        public Vertex()
        {
            U = 0;
            V = 0;
            P = Vector3.Zero;
            Pu = Vector3.Zero;
            Pv = Vector3.Zero;
            N = new Vector3(0, 0, 1);
        }

        /// <summary>
        /// Apply rotation transformations
        /// </summary>
        public void Transform(float alpha, float beta)
        {
            // Create rotation matrices
            Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(alpha);
            Matrix4x4 rotX = Matrix4x4.CreateRotationX(beta);
            Matrix4x4 rotation = rotZ * rotX;

            // Transform point
            PTransformed = Vector3.Transform(P, rotation);

            // Transform tangent vectors
            PuTransformed = Vector3.TransformNormal(Pu, rotation);
            PvTransformed = Vector3.TransformNormal(Pv, rotation);

            // Transform normal
            NTransformed = Vector3.TransformNormal(N, rotation);
            if (NTransformed.LengthSquared() > 0)
                NTransformed = Vector3.Normalize(NTransformed);
        }
    }
}
