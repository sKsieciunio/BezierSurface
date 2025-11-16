using System.Numerics;

namespace BezierSurface
{
    public partial class Form1 : Form
    {
        private BezierSurface bezierSurface;
        private TriangleMesh mesh;
        private LightingModel lighting;
        private TriangleFillerEdgeSort filler;
        private Bitmap renderBuffer;
        private Bitmap texture;
        private Bitmap normalMap;

        private float lightAngle = 0;
        private float lightRadius = 3.0f;

        public Form1()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            // Initialize components
            mesh = new TriangleMesh();
            lighting = new LightingModel();
            filler = new TriangleFillerEdgeSort(lighting);

            // Create render buffer
            renderBuffer = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = renderBuffer;

            // Wire up event handlers
            trackBarDivisions.ValueChanged += OnParameterChanged;
            trackBarAlpha.ValueChanged += OnParameterChanged;
            trackBarBeta.ValueChanged += OnParameterChanged;
            trackBarKd.ValueChanged += OnParameterChanged;
            trackBarKs.ValueChanged += OnParameterChanged;
            trackBarM.ValueChanged += OnParameterChanged;
            trackBarLightZ.ValueChanged += OnParameterChanged;

            checkBoxPolygon.CheckedChanged += OnParameterChanged;
            checkBoxMesh.CheckedChanged += OnParameterChanged;
            checkBoxFilled.CheckedChanged += OnParameterChanged;
            checkBoxAnimateLight.CheckedChanged += OnAnimationChanged;
            checkBoxNormalMap.CheckedChanged += OnParameterChanged;
            checkBoxShowLight.CheckedChanged += OnParameterChanged;

            radioButtonSolidColor.CheckedChanged += OnParameterChanged;
            radioButtonTexture.CheckedChanged += OnParameterChanged;

            buttonLoadControlPoints.Click += OnLoadControlPoints;
            buttonLoadTexture.Click += OnLoadTexture;
            buttonLoadNormalMap.Click += OnLoadNormalMap;
            buttonChooseColor.Click += OnChooseColor;

            animationTimer.Tick += OnAnimationTick;
            animationTimer.Start();

            // Try to load default control points
            TryLoadDefaultControlPoints();
        }

        private void TryLoadDefaultControlPoints()
        {
            // Try to find a default control points file
            string[] possibleFiles = { "control_points.txt", "points.txt", "bezier.txt" };

            foreach (var file in possibleFiles)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        LoadControlPoints(file);
                        return;
                    }
                    catch { }
                }
            }

            // If no file found, create a default surface
            CreateDefaultSurface();
        }

        private void CreateDefaultSurface()
        {
            // Create a simple wave-like surface
            bezierSurface = new BezierSurface();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float x = (i - 1.5f) * 2.0f;
                    float y = (j - 1.5f) * 2.0f;
                    float z = (float)(Math.Sin(i * 0.8) * Math.Cos(j * 0.8)) * 1.5f;

                    bezierSurface.ControlPoints[i, j] = new Vector3(x, y, z);
                }
            }

            GenerateMesh();

            // Create default texture if none exists
            CreateDefaultTexture();
        }

        private void CreateDefaultTexture()
        {
            // Create a simple checkered texture
            int size = 256;
            texture = new Bitmap(size, size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Checkerboard pattern
                    bool isWhite = ((i / 32) + (j / 32)) % 2 == 0;
                    Color color = isWhite ? Color.FromArgb(255, 200, 100) : Color.FromArgb(200, 100, 50);
                    texture.SetPixel(i, j, color);
                }
            }

            filler.SetTexture(texture);

            // Create a simple normal map (flat surface pointing up)
            normalMap = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Default normal pointing up: (0, 0, 1) -> RGB (128, 128, 255)
                    normalMap.SetPixel(i, j, Color.FromArgb(128, 128, 255));
                }
            }

            filler.SetNormalMap(normalMap);
        }

        private void OnLoadControlPoints(object? sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.Title = "Load Control Points";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadControlPoints(dialog.FileName);
                }
            }
        }

        private void LoadControlPoints(string filePath)
        {
            try
            {
                bezierSurface = BezierSurface.LoadFromFile(filePath);
                GenerateMesh();
                MessageBox.Show("Control points loaded successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading control points: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnLoadTexture(object? sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All files (*.*)|*.*";
                dialog.Title = "Load Texture";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        texture = new Bitmap(dialog.FileName);
                        filler.SetTexture(texture);
                        Render();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading texture: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnLoadNormalMap(object? sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All files (*.*)|*.*";
                dialog.Title = "Load Normal Map";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        normalMap = new Bitmap(dialog.FileName);
                        filler.SetNormalMap(normalMap);
                        Render();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading normal map: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnChooseColor(object? sender, EventArgs e)
        {
            using (ColorDialog dialog = new ColorDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filler.SetSolidColor(dialog.Color);
                    Render();
                }
            }
        }

        private void OnParameterChanged(object? sender, EventArgs e)
        {
            // Update labels
            labelDivisions.Text = $"Divisions: {trackBarDivisions.Value}";
            labelAlpha.Text = $"Alpha (Z): {trackBarAlpha.Value}°";
            labelBeta.Text = $"Beta (X): {trackBarBeta.Value}°";
            labelKd.Text = $"Kd: {trackBarKd.Value / 100.0f:F2}";
            labelKs.Text = $"Ks: {trackBarKs.Value / 100.0f:F2}";
            labelM.Text = $"M: {trackBarM.Value}";
            labelLightZ.Text = $"Light Z: {trackBarLightZ.Value}";

            // Update lighting parameters
            lighting.Kd = trackBarKd.Value / 100.0f;
            lighting.Ks = trackBarKs.Value / 100.0f;
            lighting.M = trackBarM.Value;

            // Update filler settings
            filler.SetUseTexture(radioButtonTexture.Checked);
            filler.SetUseNormalMap(checkBoxNormalMap.Checked);

            // Regenerate mesh if divisions changed
            if (sender == trackBarDivisions)
            {
                GenerateMesh();
            }
            else
            {
                Render();
            }
        }

        private void OnAnimationChanged(object? sender, EventArgs e)
        {
            // Animation state is checked in OnAnimationTick
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            if (checkBoxAnimateLight.Checked)
            {
                lightAngle += 0.05f;
                UpdateLightPosition();
                Render();
            }
        }

        private void UpdateLightPosition()
        {
            float z = trackBarLightZ.Value / 10.0f;
            float x = lightRadius * (float)Math.Cos(lightAngle);
            float y = lightRadius * (float)Math.Sin(lightAngle);

            lighting.LightDirection = new Vector3(x, y, z);
        }

        private void GenerateMesh()
        {
            if (bezierSurface == null) return;

            int divisions = trackBarDivisions.Value;
            mesh.Generate(bezierSurface, divisions);
            Render();
        }

        private void Render()
        {
            if (bezierSurface == null) return;

            // Clear buffer
            using (Graphics g = Graphics.FromImage(renderBuffer))
            {
                g.Clear(Color.White);
            }

            // Get rotation angles
            float alpha = trackBarAlpha.Value * (float)Math.PI / 180.0f;
            float beta = trackBarBeta.Value * (float)Math.PI / 180.0f;

            // Update light position
            UpdateLightPosition();

            // Transform mesh
            mesh.Transform(alpha, beta);

            // Apply scaling for better visualization
            float scale = 80.0f; // Scale factor to make surface visible
            ScaleMesh(scale);

            // Render based on checkboxes
            if (checkBoxFilled.Checked)
            {
                RenderFilledTriangles();
            }

            if (checkBoxMesh.Checked)
            {
                RenderWireframe(alpha, beta, scale);
            }

            if (checkBoxPolygon.Checked)
            {
                RenderControlPolygon(alpha, beta, scale);
            }

            // Render light source visualization
            if (checkBoxShowLight.Checked)
            {
                RenderLightSource(alpha, beta, scale);
            }

            pictureBox.Invalidate();
        }

        private void ScaleMesh(float scale)
        {
            if (mesh.Vertices == null) return;

            for (int i = 0; i < mesh.Vertices.GetLength(0); i++)
            {
                for (int j = 0; j < mesh.Vertices.GetLength(1); j++)
                {
                    var v = mesh.Vertices[i, j];
                    v.PTransformed *= scale;
                }
            }
        }

        private void RenderFilledTriangles()
        {
            foreach (var triangle in mesh.Triangles)
            {
                filler.FillTriangle(triangle, renderBuffer);
            }
        }

        private void RenderWireframe(float alpha, float beta, float scale)
        {
            using (Graphics g = Graphics.FromImage(renderBuffer))
            {
                using (Pen pen = new Pen(Color.Black, 1))
                {
                    float centerX = renderBuffer.Width / 2.0f;
                    float centerY = renderBuffer.Height / 2.0f;

                    foreach (var triangle in mesh.Triangles)
                    {
                        var p1 = triangle.V1.PTransformed;
                        var p2 = triangle.V2.PTransformed;
                        var p3 = triangle.V3.PTransformed;

                        // Convert to screen coordinates
                        PointF pt1 = new PointF(p1.X + centerX, p1.Y + centerY);
                        PointF pt2 = new PointF(p2.X + centerX, p2.Y + centerY);
                        PointF pt3 = new PointF(p3.X + centerX, p3.Y + centerY);

                        // Draw triangle edges
                        g.DrawLine(pen, pt1, pt2);
                        g.DrawLine(pen, pt2, pt3);
                        g.DrawLine(pen, pt3, pt1);
                    }
                }
            }
        }

        private void RenderControlPolygon(float alpha, float beta, float scale)
        {
            using (Graphics g = Graphics.FromImage(renderBuffer))
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    float centerX = renderBuffer.Width / 2.0f;
                    float centerY = renderBuffer.Height / 2.0f;

                    // Create rotation matrices
                    Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(alpha);
                    Matrix4x4 rotX = Matrix4x4.CreateRotationX(beta);
                    Matrix4x4 rotation = rotZ * rotX;

                    // Draw control point grid
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            var p1 = Vector3.Transform(bezierSurface.ControlPoints[i, j], rotation) * scale;
                            var p2 = Vector3.Transform(bezierSurface.ControlPoints[i, j + 1], rotation) * scale;

                            PointF pt1 = new PointF(p1.X + centerX, p1.Y + centerY);
                            PointF pt2 = new PointF(p2.X + centerX, p2.Y + centerY);

                            g.DrawLine(pen, pt1, pt2);
                        }
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            var p1 = Vector3.Transform(bezierSurface.ControlPoints[i, j], rotation) * scale;
                            var p2 = Vector3.Transform(bezierSurface.ControlPoints[i + 1, j], rotation) * scale;

                            PointF pt1 = new PointF(p1.X + centerX, p1.Y + centerY);
                            PointF pt2 = new PointF(p2.X + centerX, p2.Y + centerY);

                            g.DrawLine(pen, pt1, pt2);
                        }
                    }

                    // Draw control points
                    using (Brush brush = new SolidBrush(Color.Blue))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                var p = Vector3.Transform(bezierSurface.ControlPoints[i, j], rotation) * scale;
                                PointF pt = new PointF(p.X + centerX, p.Y + centerY);
                                g.FillEllipse(brush, pt.X - 3, pt.Y - 3, 6, 6);
                            }
                        }
                    }
                }
            }
        }

        private void RenderLightSource(float alpha, float beta, float scale)
        {
            using (Graphics g = Graphics.FromImage(renderBuffer))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float centerX = renderBuffer.Width / 2.0f;
                float centerY = renderBuffer.Height / 2.0f;

                // Get light position in 3D space
                Vector3 lightPos = lighting.LightDirection;

                // Apply same rotation as the surface
                Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(alpha);
                Matrix4x4 rotX = Matrix4x4.CreateRotationX(beta);
                Matrix4x4 rotation = rotZ * rotX;

                Vector3 lightPosRotated = Vector3.TransformNormal(lightPos, rotation);

                // Scale the light position
                Vector3 lightPosScaled = lightPosRotated * scale;

                // Project to 2D screen coordinates
                PointF lightScreen = new PointF(
                    lightPosScaled.X + centerX,
                    lightPosScaled.Y + centerY
                );

                // Draw light source as a sun/star icon
                float lightSize = 20;

                // Draw rays
                using (Pen rayPen = new Pen(Color.FromArgb(200, 255, 255, 0), 2))
                {
                    int numRays = 8;
                    for (int i = 0; i < numRays; i++)
                    {
                        float angle = (float)(i * 2 * Math.PI / numRays);
                        float innerRadius = lightSize * 0.6f;
                        float outerRadius = lightSize * 1.2f;

                        PointF inner = new PointF(
                            lightScreen.X + innerRadius * (float)Math.Cos(angle),
                            lightScreen.Y + innerRadius * (float)Math.Sin(angle)
                        );
                        PointF outer = new PointF(
                            lightScreen.X + outerRadius * (float)Math.Cos(angle),
                            lightScreen.Y + outerRadius * (float)Math.Sin(angle)
                        );

                        g.DrawLine(rayPen, inner, outer);
                    }
                }

                // Draw light glow (outer circle)
                using (Brush glowBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new RectangleF(lightScreen.X - lightSize, lightScreen.Y - lightSize, lightSize * 2, lightSize * 2),
                    Color.FromArgb(100, 255, 255, 150),
                    Color.FromArgb(0, 255, 255, 150),
                    System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
                {
                    g.FillEllipse(glowBrush,
                        lightScreen.X - lightSize,
                        lightScreen.Y - lightSize,
                        lightSize * 2,
                        lightSize * 2);
                }

                // Draw light core (inner circle)
                using (Brush coreBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new RectangleF(lightScreen.X - lightSize * 0.5f, lightScreen.Y - lightSize * 0.5f, lightSize, lightSize),
                    Color.FromArgb(255, 255, 255, 100),
                    Color.FromArgb(255, 255, 255, 0),
                    System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
                {
                    g.FillEllipse(coreBrush,
                        lightScreen.X - lightSize * 0.5f,
                        lightScreen.Y - lightSize * 0.5f,
                        lightSize,
                        lightSize);
                }

                // Draw center point
                using (Brush centerBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(centerBrush,
                        lightScreen.X - 4,
                        lightScreen.Y - 4,
                        8,
                        8);
                }

                // Draw light direction indicator (arrow pointing towards origin)
                Vector3 dirToOrigin = -Vector3.Normalize(lightPosRotated);
                float arrowLength = 40;
                PointF arrowEnd = new PointF(
                    lightScreen.X + dirToOrigin.X * arrowLength,
                    lightScreen.Y + dirToOrigin.Y * arrowLength
                );

                using (Pen arrowPen = new Pen(Color.FromArgb(180, 255, 200, 0), 3))
                {
                    arrowPen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5);
                    g.DrawLine(arrowPen, lightScreen, arrowEnd);
                }

                // Draw label
                string lightLabel = $"Light\nZ={trackBarLightZ.Value}";
                using (Font font = new Font("Arial", 8, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.FromArgb(220, 255, 200, 0)))
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    PointF textPos = new PointF(lightScreen.X + lightSize + 5, lightScreen.Y - 10);

                    // Draw shadow
                    g.DrawString(lightLabel, font, shadowBrush, textPos.X + 1, textPos.Y + 1);
                    // Draw text
                    g.DrawString(lightLabel, font, textBrush, textPos);
                }
            }
        }
    }
}
