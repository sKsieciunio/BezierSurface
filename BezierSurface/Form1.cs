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
        private Vector3 baseLightDirection;

        private float surfaceAnimationTime = 0;
        private Vector3[,] originalControlPoints = new Vector3[4, 4];

        public Form1()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            mesh = new TriangleMesh();
            lighting = new LightingModel();
            filler = new TriangleFillerEdgeSort(lighting);

            renderBuffer = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = renderBuffer;

            filler.InitializeZBuffer(pictureBox.Width, pictureBox.Height);

            trackBarDivisions.ValueChanged += OnParameterChanged;
            trackBarAlpha.ValueChanged += OnParameterChanged;
            trackBarBeta.ValueChanged += OnParameterChanged;
            trackBarKd.ValueChanged += OnParameterChanged;
            trackBarKs.ValueChanged += OnParameterChanged;
            trackBarM.ValueChanged += OnParameterChanged;
            trackBarLightZ.ValueChanged += OnParameterChanged;
            trackBarSpotlightAngle.ValueChanged += OnParameterChanged;
            trackBarSpotlightExponent.ValueChanged += OnParameterChanged;

            checkBoxPolygon.CheckedChanged += OnParameterChanged;
            checkBoxMesh.CheckedChanged += OnParameterChanged;
            checkBoxFilled.CheckedChanged += OnParameterChanged;
            checkBoxAnimateLight.CheckedChanged += OnAnimationChanged;
            checkBoxAnimateSurface.CheckedChanged += OnAnimationChanged;
            checkBoxNormalMap.CheckedChanged += OnParameterChanged;
            checkBoxShowLight.CheckedChanged += OnParameterChanged;

            radioButtonSolidColor.CheckedChanged += OnParameterChanged;
            radioButtonTexture.CheckedChanged += OnParameterChanged;
            radioButtonDirectional.CheckedChanged += OnLightTypeChanged;
            radioButtonPoint.CheckedChanged += OnLightTypeChanged;
            radioButtonSpotlight.CheckedChanged += OnLightTypeChanged;

            buttonLoadControlPoints.Click += OnLoadControlPoints;
            buttonLoadTexture.Click += OnLoadTexture;
            buttonLoadNormalMap.Click += OnLoadNormalMap;
            buttonChooseColor.Click += OnChooseColor;
            buttonChooseLightColor.Click += OnChooseLightColor;

            trackBarSpotlightAngle.Enabled = false;
            trackBarSpotlightExponent.Enabled = false;
            
            Color defaultLightColor = Color.FromArgb(255, 230, 153); 
            buttonChooseLightColor.BackColor = defaultLightColor;
            buttonChooseLightColor.ForeColor = GetContrastColor(defaultLightColor);
            
            trackBarKd.Value = 70; 
            trackBarKs.Value = 30; 

            animationTimer.Tick += OnAnimationTick;
            animationTimer.Start();

            CreateDefaultSurface();

            OnParameterChanged(this, EventArgs.Empty);
        }

        private void CreateDefaultSurface()
        {
            bezierSurface = new BezierSurface();

            float spacing = 1.2f;
            float radius = 3.0f; 
            float r2 = radius * radius;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float x = (i - 1.5f) * spacing;
                    float y = (j - 1.5f) * spacing;

                    float d2 = x * x + y * y;
                    float z = 0;
                    if (d2 <= r2)
                    {
                        z = (float)Math.Sqrt(r2 - d2) - radius * 0.8f; 
                    }

                    bezierSurface.ControlPoints[i, j] = new Vector3(x, y, z);
                }
            }

            StoreOriginalControlPoints();

            GenerateMesh();

            CreateDefaultTexture();
        }

        private void CreateDefaultTexture()
        {
            int size = 256;
            texture = new Bitmap(size, size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    bool isWhite = ((i / 32) + (j / 32)) % 2 == 0;
                    Color color = isWhite ? Color.FromArgb(255, 200, 100) : Color.FromArgb(200, 100, 50);
                    texture.SetPixel(i, j, color);
                }
            }

            filler.SetTexture(texture);

            normalMap = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
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
                StoreOriginalControlPoints();
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

        private void OnChooseLightColor(object? sender, EventArgs e)
        {
            using (ColorDialog dialog = new ColorDialog())
            {
                Vector3 currentLightColor = lighting.LightColor;
                int r = (int)(currentLightColor.X * 255);
                int g = (int)(currentLightColor.Y * 255);
                int b = (int)(currentLightColor.Z * 255);
                dialog.Color = Color.FromArgb(r, g, b);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    lighting.LightColor = new Vector3(
                        dialog.Color.R / 255.0f,
                        dialog.Color.G / 255.0f,
                        dialog.Color.B / 255.0f
                    );
                    
                    buttonChooseLightColor.BackColor = dialog.Color;
                    buttonChooseLightColor.ForeColor = GetContrastColor(dialog.Color);
                    
                    Render();
                }
            }
        }

        private Color GetContrastColor(Color backgroundColor)
        {
            float luminance = (0.299f * backgroundColor.R + 
                             0.587f * backgroundColor.G + 
                             0.114f * backgroundColor.B) / 255f;
            
            return luminance > 0.5f ? Color.Black : Color.White;
        }

        private void OnParameterChanged(object? sender, EventArgs e)
        {
            labelDivisions.Text = $"Divisions: {trackBarDivisions.Value}";
            labelAlpha.Text = $"Alpha (Z): {trackBarAlpha.Value}°";
            labelBeta.Text = $"Beta (X): {trackBarBeta.Value}°";
            labelSpotlightAngle.Text = $"Spotlight Angle: {trackBarSpotlightAngle.Value}°";
            labelSpotlightExponent.Text = $"Spotlight Exponent (N): {trackBarSpotlightExponent.Value}";

            float kd = trackBarKd.Value / 100.0f;
            float ks = trackBarKs.Value / 100.0f;

            labelKd.Text = $"Kd: {kd:F2}";
            labelKs.Text = $"Ks: {ks:F2}";
            labelM.Text = $"M: {trackBarM.Value}";
            labelLightZ.Text = $"Light Z: {trackBarLightZ.Value}";
            labelEnergyConservation.Visible = false;
            labelKdNormalized.Visible = false;
            labelKsNormalized.Visible = false;

            float sum = kd + ks;
            if (sum > 1.0f)
            {
                float invSum = 1.0f / sum;
                float kdNormalized = kd * invSum;
                float ksNormalized = ks * invSum;

                labelEnergyConservation.Text = "⚠ Energy Conservation Applied";
                labelEnergyConservation.Visible = true;

                labelKdNormalized.Text = $"(actual: {kdNormalized:F2})";
                labelKdNormalized.Visible = true;

                labelKsNormalized.Text = $"(actual: {ksNormalized:F2})";
                labelKsNormalized.Visible = true;
            }

            lighting.Kd = kd;
            lighting.Ks = ks;
            lighting.M = trackBarM.Value;
            
            float spotAngleRadians = trackBarSpotlightAngle.Value * (float)Math.PI / 180.0f;
            lighting.SpotlightCutoffAngle = spotAngleRadians;
            lighting.SpotlightExponent = trackBarSpotlightExponent.Value;

            filler.SetUseTexture(radioButtonTexture.Checked);
            filler.SetUseNormalMap(checkBoxNormalMap.Checked);

            if (sender == trackBarDivisions)
            {
                GenerateMesh();
            }
            else
            {
                Render();
            }
        }

        private void OnLightTypeChanged(object? sender, EventArgs e)
        {
            if (radioButtonDirectional.Checked)
            {
                lighting.CurrentLightType = LightType.Directional;
                trackBarSpotlightAngle.Enabled = false;
                trackBarSpotlightExponent.Enabled = false;
            }
            else if (radioButtonPoint.Checked)
            {
                lighting.CurrentLightType = LightType.Point;
                trackBarSpotlightAngle.Enabled = false;
                trackBarSpotlightExponent.Enabled = false;
            }
            else if (radioButtonSpotlight.Checked)
            {
                lighting.CurrentLightType = LightType.Spotlight;
                trackBarSpotlightAngle.Enabled = true;
                trackBarSpotlightExponent.Enabled = true;
            }
            
            Render();
        }

        private void OnAnimationChanged(object? sender, EventArgs e)
        {
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            bool needsRender = false;

            if (checkBoxAnimateLight.Checked)
            {
                lightAngle += 0.05f;
                UpdateLightPosition();
                needsRender = true;
            }

            if (checkBoxAnimateSurface.Checked)
            {
                surfaceAnimationTime += 0.05f;
                AnimateSurface();
                needsRender = true;
            }

            if (needsRender)
            {
                Render();
            }
        }

        private void UpdateLightPosition()
        {
            float z = trackBarLightZ.Value / 10.0f;
            float x = lightRadius * (float)Math.Cos(lightAngle);
            float y = lightRadius * (float)Math.Sin(lightAngle);

            baseLightDirection = new Vector3(x, y, z);
        }

        private void StoreOriginalControlPoints()
        {
            if (bezierSurface == null) return;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    originalControlPoints[i, j] = bezierSurface.ControlPoints[i, j];
                }
            }
        }

        private void AnimateSurface()
        {
            if (bezierSurface == null) return;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 original = originalControlPoints[i, j];
                    
                    float wave1 = (float)Math.Sin(surfaceAnimationTime + i * 0.5f) * 0.3f;
                    float wave2 = (float)Math.Cos(surfaceAnimationTime + j * 0.5f) * 0.3f;
                    float zOffset = wave1 + wave2;

                    bezierSurface.ControlPoints[i, j] = new Vector3(
                        original.X, 
                        original.Y, 
                        original.Z + zOffset
                    );
                }
            }

            GenerateMesh();
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

            using (Graphics g = Graphics.FromImage(renderBuffer))
            {
                g.Clear(Color.White);
            }

            filler.ClearZBuffer();

            float alpha = trackBarAlpha.Value * (float)Math.PI / 180.0f;
            float beta = trackBarBeta.Value * (float)Math.PI / 180.0f;

            UpdateLightPosition();

            Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(alpha);
            Matrix4x4 rotX = Matrix4x4.CreateRotationX(beta);
            Matrix4x4 rotation = rotZ * rotX;

            Vector3 transformedLightDirection = Vector3.TransformNormal(baseLightDirection, rotation);
            
            lighting.LightDirection = transformedLightDirection;
            
            lighting.LightPosition = baseLightDirection;
            
            if (lighting.CurrentLightType == LightType.Spotlight)
            {
                Vector3 centerOfScene = Vector3.Zero;
                Vector3 spotDir = Vector3.Normalize(centerOfScene - baseLightDirection);
                lighting.SpotlightDirection = spotDir;
            }

            mesh.Transform(alpha, beta);

            float scale = 100.0f; 
            ScaleMesh(scale);

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

                        PointF pt1 = new PointF(p1.X + centerX, p1.Y + centerY);
                        PointF pt2 = new PointF(p2.X + centerX, p2.Y + centerY);
                        PointF pt3 = new PointF(p3.X + centerX, p3.Y + centerY);

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

                    Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(alpha);
                    Matrix4x4 rotX = Matrix4x4.CreateRotationX(beta);
                    Matrix4x4 rotation = rotZ * rotX;

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

                Vector3 lightPos = baseLightDirection;

                Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(alpha);
                Matrix4x4 rotX = Matrix4x4.CreateRotationX(beta);
                Matrix4x4 rotation = rotZ * rotX;

                Vector3 lightPosRotated = Vector3.TransformNormal(lightPos, rotation);

                Vector3 lightPosScaled = lightPosRotated * scale;

                PointF lightScreen = new PointF(
                    lightPosScaled.X + centerX,
                    lightPosScaled.Y + centerY
                );

                Color lightColor = Color.FromArgb(
                    (int)(lighting.LightColor.X * 255),
                    (int)(lighting.LightColor.Y * 255),
                    (int)(lighting.LightColor.Z * 255)
                );

                if (lighting.CurrentLightType == LightType.Spotlight)
                {
                    Vector3 centerOfScene = Vector3.Zero;
                    Vector3 spotDir = Vector3.Normalize(centerOfScene - lightPos);
                    Vector3 spotDirRotated = Vector3.TransformNormal(spotDir, rotation);
                    
                    float coneLength = 150f;
                    PointF coneEnd = new PointF(
                        lightScreen.X + spotDirRotated.X * coneLength,
                        lightScreen.Y + spotDirRotated.Y * coneLength
                    );
                    
                    float coneRadius = (float)Math.Tan(lighting.SpotlightCutoffAngle) * coneLength;
                    
                    Vector3 perpendicular1 = Vector3.Normalize(Vector3.Cross(spotDirRotated, new Vector3(0, 1, 0)));
                    if (perpendicular1.LengthSquared() < 0.01f)
                        perpendicular1 = Vector3.Normalize(Vector3.Cross(spotDirRotated, new Vector3(1, 0, 0)));
                    Vector3 perpendicular2 = Vector3.Normalize(Vector3.Cross(spotDirRotated, perpendicular1));
                    
                    int numSegments = 12;
                    using (Brush coneBrush = new SolidBrush(Color.FromArgb(30, lightColor)))
                    {
                        for (int i = 0; i < numSegments; i++)
                        {
                            float angle1 = (float)(i * 2 * Math.PI / numSegments);
                            float angle2 = (float)((i + 1) * 2 * Math.PI / numSegments);
                            
                            PointF edge1 = new PointF(
                                coneEnd.X + (perpendicular1.X * (float)Math.Cos(angle1) + perpendicular2.X * (float)Math.Sin(angle1)) * coneRadius,
                                coneEnd.Y + (perpendicular1.Y * (float)Math.Cos(angle1) + perpendicular2.Y * (float)Math.Sin(angle1)) * coneRadius
                            );
                            

                            PointF edge2 = new PointF(
                                coneEnd.X + (perpendicular1.X * (float)Math.Cos(angle2) + perpendicular2.X * (float)Math.Sin(angle2)) * coneRadius,
                                coneEnd.Y + (perpendicular1.Y * (float)Math.Cos(angle2) + perpendicular2.Y * (float)Math.Sin(angle2)) * coneRadius
                            );
                            
                            PointF[] triangle = { lightScreen, edge1, edge2 };
                            g.FillPolygon(coneBrush, triangle);
                        }
                    }
                    
                    using (Pen conePen = new Pen(Color.FromArgb(80, lightColor), 1))
                    {
                        for (int i = 0; i < numSegments; i++)
                        {
                            float angle = (float)(i * 2 * Math.PI / numSegments);
                            PointF edge = new PointF(
                                coneEnd.X + (perpendicular1.X * (float)Math.Cos(angle) + perpendicular2.X * (float)Math.Sin(angle)) * coneRadius,
                                coneEnd.Y + (perpendicular1.Y * (float)Math.Cos(angle) + perpendicular2.Y * (float)Math.Sin(angle)) * coneRadius
                            );
                            g.DrawLine(conePen, lightScreen, edge);
                        }
                    }
                }

                float lightSize = 20;

                if (lighting.CurrentLightType == LightType.Directional)
                {
                    using (Pen rayPen = new Pen(Color.FromArgb(200, lightColor), 2))
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
                }

                using (Brush glowBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new RectangleF(lightScreen.X - lightSize, lightScreen.Y - lightSize, lightSize * 2, lightSize * 2),
                    Color.FromArgb(100, lightColor),
                    Color.FromArgb(0, lightColor),
                    System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
                {
                    g.FillEllipse(glowBrush,
                        lightScreen.X - lightSize,
                        lightScreen.Y - lightSize,
                        lightSize * 2,
                        lightSize * 2);
                }

                Color coreColor = Color.FromArgb(
                    Math.Min(255, lightColor.R + 100),
                    Math.Min(255, lightColor.G + 100),
                    Math.Min(255, lightColor.B + 100)
                );
                using (Brush coreBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new RectangleF(lightScreen.X - lightSize * 0.5f, lightScreen.Y - lightSize * 0.5f, lightSize, lightSize),
                    Color.FromArgb(255, coreColor),
                    Color.FromArgb(255, lightColor),
                    System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
                {
                    g.FillEllipse(coreBrush,
                        lightScreen.X - lightSize * 0.5f,
                        lightScreen.Y - lightSize * 0.5f,
                        lightSize,
                        lightSize);
                }

                using (Brush centerBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(centerBrush,
                        lightScreen.X - 4,
                        lightScreen.Y - 4,
                        8,
                        8);
                }

                if (lighting.CurrentLightType == LightType.Directional)
                {
                    Vector3 dirToOrigin = -Vector3.Normalize(lightPosRotated);
                    float arrowLength = 40;
                    PointF arrowEnd = new PointF(
                        lightScreen.X + dirToOrigin.X * arrowLength,
                        lightScreen.Y + dirToOrigin.Y * arrowLength
                    );

                    using (Pen arrowPen = new Pen(Color.FromArgb(180, lightColor), 3))
                    {
                        arrowPen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5);
                        g.DrawLine(arrowPen, lightScreen, arrowEnd);
                    }
                }

                string lightTypeText = lighting.CurrentLightType switch
                {
                    LightType.Directional => "Directional",
                    LightType.Point => "Point",
                    LightType.Spotlight => $"Spotlight {trackBarSpotlightAngle.Value}°",
                    _ => "Light"
                };
                
                string lightLabel = $"{lightTypeText}\nZ={trackBarLightZ.Value}";
                using (Font font = new Font("Arial", 8, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.FromArgb(220, lightColor)))
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    PointF textPos = new PointF(lightScreen.X + lightSize + 5, lightScreen.Y - 10);

                    g.DrawString(lightLabel, font, shadowBrush, textPos.X + 1, textPos.Y + 1);
                    g.DrawString(lightLabel, font, textBrush, textPos);
                }
            }
        }

        private void trackBarKd_Scroll(object sender, EventArgs e)
        {

        }
    }
}
