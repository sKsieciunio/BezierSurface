namespace BezierSurface
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox = new PictureBox();
            trackBarDivisions = new TrackBar();
            trackBarAlpha = new TrackBar();
            trackBarBeta = new TrackBar();
            trackBarKd = new TrackBar();
            trackBarKs = new TrackBar();
            trackBarM = new TrackBar();
            trackBarLightZ = new TrackBar();
            checkBoxPolygon = new CheckBox();
            checkBoxMesh = new CheckBox();
            checkBoxFilled = new CheckBox();
            checkBoxAnimateLight = new CheckBox();
            checkBoxShowLight = new CheckBox();
            radioButtonSolidColor = new RadioButton();
            radioButtonTexture = new RadioButton();
            checkBoxNormalMap = new CheckBox();
            buttonLoadTexture = new Button();
            buttonLoadNormalMap = new Button();
            buttonChooseColor = new Button();
            buttonLoadControlPoints = new Button();
            buttonChooseLightColor = new Button();
            labelDivisions = new Label();
            labelAlpha = new Label();
            labelBeta = new Label();
            labelKd = new Label();
            labelKs = new Label();
            labelM = new Label();
            labelLightZ = new Label();
            labelEnergyConservation = new Label();
            labelKdNormalized = new Label();
            labelKsNormalized = new Label();
            animationTimer = new System.Windows.Forms.Timer(components);
            groupBoxDisplay = new GroupBox();
            groupBoxRotation = new GroupBox();
            groupBoxLighting = new GroupBox();
            groupBoxObjectColor = new GroupBox();
            groupBoxLight = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarDivisions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBeta).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarKd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarKs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarM).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLightZ).BeginInit();
            groupBoxDisplay.SuspendLayout();
            groupBoxRotation.SuspendLayout();
            groupBoxLighting.SuspendLayout();
            groupBoxObjectColor.SuspendLayout();
            groupBoxLight.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.BackColor = Color.White;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.Location = new Point(12, 12);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(600, 600);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // trackBarDivisions
            // 
            trackBarDivisions.Location = new Point(6, 38);
            trackBarDivisions.Maximum = 50;
            trackBarDivisions.Minimum = 1;
            trackBarDivisions.Name = "trackBarDivisions";
            trackBarDivisions.Size = new Size(200, 45);
            trackBarDivisions.TabIndex = 0;
            trackBarDivisions.Value = 10;
            // 
            // trackBarAlpha
            // 
            trackBarAlpha.Location = new Point(6, 35);
            trackBarAlpha.Maximum = 90;
            trackBarAlpha.Minimum = -90;
            trackBarAlpha.Name = "trackBarAlpha";
            trackBarAlpha.Size = new Size(200, 45);
            trackBarAlpha.TabIndex = 0;
            // 
            // trackBarBeta
            // 
            trackBarBeta.Location = new Point(6, 95);
            trackBarBeta.Maximum = 90;
            trackBarBeta.Minimum = -90;
            trackBarBeta.Name = "trackBarBeta";
            trackBarBeta.Size = new Size(200, 45);
            trackBarBeta.TabIndex = 1;
            // 
            // trackBarKd
            // 
            trackBarKd.Location = new Point(6, 35);
            trackBarKd.Maximum = 100;
            trackBarKd.Name = "trackBarKd";
            trackBarKd.Size = new Size(200, 45);
            trackBarKd.TabIndex = 0;
            trackBarKd.Value = 50;
            trackBarKd.Scroll += trackBarKd_Scroll;
            // 
            // trackBarKs
            // 
            trackBarKs.Location = new Point(6, 95);
            trackBarKs.Maximum = 100;
            trackBarKs.Name = "trackBarKs";
            trackBarKs.Size = new Size(200, 45);
            trackBarKs.TabIndex = 1;
            trackBarKs.Value = 50;
            // 
            // trackBarM
            // 
            trackBarM.Location = new Point(6, 155);
            trackBarM.Maximum = 100;
            trackBarM.Minimum = 1;
            trackBarM.Name = "trackBarM";
            trackBarM.Size = new Size(200, 45);
            trackBarM.TabIndex = 2;
            trackBarM.Value = 50;
            // 
            // trackBarLightZ
            // 
            trackBarLightZ.Location = new Point(6, 35);
            trackBarLightZ.Maximum = 100;
            trackBarLightZ.Minimum = -100;
            trackBarLightZ.Name = "trackBarLightZ";
            trackBarLightZ.Size = new Size(200, 45);
            trackBarLightZ.TabIndex = 0;
            trackBarLightZ.Value = 20;
            // 
            // checkBoxPolygon
            // 
            checkBoxPolygon.AutoSize = true;
            checkBoxPolygon.Location = new Point(6, 89);
            checkBoxPolygon.Name = "checkBoxPolygon";
            checkBoxPolygon.Size = new Size(104, 19);
            checkBoxPolygon.TabIndex = 0;
            checkBoxPolygon.Text = "Bézier Polygon";
            checkBoxPolygon.UseVisualStyleBackColor = true;
            // 
            // checkBoxMesh
            // 
            checkBoxMesh.AutoSize = true;
            checkBoxMesh.Checked = true;
            checkBoxMesh.CheckState = CheckState.Checked;
            checkBoxMesh.Location = new Point(6, 114);
            checkBoxMesh.Name = "checkBoxMesh";
            checkBoxMesh.Size = new Size(100, 19);
            checkBoxMesh.TabIndex = 1;
            checkBoxMesh.Text = "Triangle Mesh";
            checkBoxMesh.UseVisualStyleBackColor = true;
            // 
            // checkBoxFilled
            // 
            checkBoxFilled.AutoSize = true;
            checkBoxFilled.Checked = true;
            checkBoxFilled.CheckState = CheckState.Checked;
            checkBoxFilled.Location = new Point(6, 140);
            checkBoxFilled.Name = "checkBoxFilled";
            checkBoxFilled.Size = new Size(104, 19);
            checkBoxFilled.TabIndex = 2;
            checkBoxFilled.Text = "Filled Triangles";
            checkBoxFilled.UseVisualStyleBackColor = true;
            // 
            // checkBoxAnimateLight
            // 
            checkBoxAnimateLight.AutoSize = true;
            checkBoxAnimateLight.Checked = true;
            checkBoxAnimateLight.CheckState = CheckState.Checked;
            checkBoxAnimateLight.Location = new Point(6, 95);
            checkBoxAnimateLight.Name = "checkBoxAnimateLight";
            checkBoxAnimateLight.Size = new Size(101, 19);
            checkBoxAnimateLight.TabIndex = 1;
            checkBoxAnimateLight.Text = "Animate Light";
            checkBoxAnimateLight.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowLight
            // 
            checkBoxShowLight.AutoSize = true;
            checkBoxShowLight.Checked = true;
            checkBoxShowLight.CheckState = CheckState.Checked;
            checkBoxShowLight.Location = new Point(6, 120);
            checkBoxShowLight.Name = "checkBoxShowLight";
            checkBoxShowLight.Size = new Size(85, 19);
            checkBoxShowLight.TabIndex = 2;
            checkBoxShowLight.Text = "Show Light";
            checkBoxShowLight.UseVisualStyleBackColor = true;
            // 
            // radioButtonSolidColor
            // 
            radioButtonSolidColor.AutoSize = true;
            radioButtonSolidColor.Checked = true;
            radioButtonSolidColor.Location = new Point(6, 22);
            radioButtonSolidColor.Name = "radioButtonSolidColor";
            radioButtonSolidColor.Size = new Size(83, 19);
            radioButtonSolidColor.TabIndex = 0;
            radioButtonSolidColor.TabStop = true;
            radioButtonSolidColor.Text = "Solid Color";
            radioButtonSolidColor.UseVisualStyleBackColor = true;
            // 
            // radioButtonTexture
            // 
            radioButtonTexture.AutoSize = true;
            radioButtonTexture.Location = new Point(6, 47);
            radioButtonTexture.Name = "radioButtonTexture";
            radioButtonTexture.Size = new Size(63, 19);
            radioButtonTexture.TabIndex = 1;
            radioButtonTexture.Text = "Texture";
            radioButtonTexture.UseVisualStyleBackColor = true;
            // 
            // checkBoxNormalMap
            // 
            checkBoxNormalMap.AutoSize = true;
            checkBoxNormalMap.Location = new Point(6, 72);
            checkBoxNormalMap.Name = "checkBoxNormalMap";
            checkBoxNormalMap.Size = new Size(93, 19);
            checkBoxNormalMap.TabIndex = 2;
            checkBoxNormalMap.Text = "Normal Map";
            checkBoxNormalMap.UseVisualStyleBackColor = true;
            // 
            // buttonLoadTexture
            // 
            buttonLoadTexture.Location = new Point(6, 97);
            buttonLoadTexture.Name = "buttonLoadTexture";
            buttonLoadTexture.Size = new Size(100, 23);
            buttonLoadTexture.TabIndex = 3;
            buttonLoadTexture.Text = "Load Texture";
            buttonLoadTexture.UseVisualStyleBackColor = true;
            // 
            // buttonLoadNormalMap
            // 
            buttonLoadNormalMap.Location = new Point(112, 97);
            buttonLoadNormalMap.Name = "buttonLoadNormalMap";
            buttonLoadNormalMap.Size = new Size(94, 23);
            buttonLoadNormalMap.TabIndex = 4;
            buttonLoadNormalMap.Text = "Load Normal";
            buttonLoadNormalMap.UseVisualStyleBackColor = true;
            // 
            // buttonChooseColor
            // 
            buttonChooseColor.Location = new Point(99, 20);
            buttonChooseColor.Name = "buttonChooseColor";
            buttonChooseColor.Size = new Size(107, 23);
            buttonChooseColor.TabIndex = 5;
            buttonChooseColor.Text = "Choose Color";
            buttonChooseColor.UseVisualStyleBackColor = true;
            // 
            // buttonLoadControlPoints
            // 
            buttonLoadControlPoints.Location = new Point(630, 12);
            buttonLoadControlPoints.Name = "buttonLoadControlPoints";
            buttonLoadControlPoints.Size = new Size(140, 30);
            buttonLoadControlPoints.TabIndex = 20;
            buttonLoadControlPoints.Text = "Load Control Points";
            buttonLoadControlPoints.UseVisualStyleBackColor = true;
            // 
            // buttonChooseLightColor
            // 
            buttonChooseLightColor.Location = new Point(6, 145);
            buttonChooseLightColor.Name = "buttonChooseLightColor";
            buttonChooseLightColor.Size = new Size(120, 23);
            buttonChooseLightColor.TabIndex = 3;
            buttonChooseLightColor.Text = "Light Color";
            buttonChooseLightColor.UseVisualStyleBackColor = true;
            // 
            // labelDivisions
            // 
            labelDivisions.AutoSize = true;
            labelDivisions.Location = new Point(6, 20);
            labelDivisions.Name = "labelDivisions";
            labelDivisions.Size = new Size(72, 15);
            labelDivisions.TabIndex = 1;
            labelDivisions.Text = "Divisions: 10";
            // 
            // labelAlpha
            // 
            labelAlpha.AutoSize = true;
            labelAlpha.Location = new Point(6, 20);
            labelAlpha.Name = "labelAlpha";
            labelAlpha.Size = new Size(73, 15);
            labelAlpha.TabIndex = 2;
            labelAlpha.Text = "Alpha (Z): 0°";
            // 
            // labelBeta
            // 
            labelBeta.AutoSize = true;
            labelBeta.Location = new Point(6, 80);
            labelBeta.Name = "labelBeta";
            labelBeta.Size = new Size(65, 15);
            labelBeta.TabIndex = 3;
            labelBeta.Text = "Beta (X): 0°";
            // 
            // labelKd
            // 
            labelKd.AutoSize = true;
            labelKd.Location = new Point(6, 20);
            labelKd.Name = "labelKd";
            labelKd.Size = new Size(48, 15);
            labelKd.TabIndex = 3;
            labelKd.Text = "Kd: 0.50";
            // 
            // labelKs
            // 
            labelKs.AutoSize = true;
            labelKs.Location = new Point(6, 80);
            labelKs.Name = "labelKs";
            labelKs.Size = new Size(46, 15);
            labelKs.TabIndex = 4;
            labelKs.Text = "Ks: 0.50";
            // 
            // labelM
            // 
            labelM.AutoSize = true;
            labelM.Location = new Point(6, 140);
            labelM.Name = "labelM";
            labelM.Size = new Size(36, 15);
            labelM.TabIndex = 5;
            labelM.Text = "M: 50";
            // 
            // labelLightZ
            // 
            labelLightZ.AutoSize = true;
            labelLightZ.Location = new Point(6, 20);
            labelLightZ.Name = "labelLightZ";
            labelLightZ.Size = new Size(62, 15);
            labelLightZ.TabIndex = 1;
            labelLightZ.Text = "Light Z: 20";
            // 
            // labelEnergyConservation
            // 
            labelEnergyConservation.AutoSize = true;
            labelEnergyConservation.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelEnergyConservation.ForeColor = Color.DarkOrange;
            labelEnergyConservation.Location = new Point(17, 203);
            labelEnergyConservation.Name = "labelEnergyConservation";
            labelEnergyConservation.Size = new Size(0, 15);
            labelEnergyConservation.TabIndex = 6;
            labelEnergyConservation.Visible = false;
            // 
            // labelKdNormalized
            // 
            labelKdNormalized.AutoSize = true;
            labelKdNormalized.ForeColor = Color.Gray;
            labelKdNormalized.Location = new Point(110, 20);
            labelKdNormalized.Name = "labelKdNormalized";
            labelKdNormalized.Size = new Size(0, 15);
            labelKdNormalized.TabIndex = 7;
            labelKdNormalized.Visible = false;
            // 
            // labelKsNormalized
            // 
            labelKsNormalized.AutoSize = true;
            labelKsNormalized.ForeColor = Color.Gray;
            labelKsNormalized.Location = new Point(110, 80);
            labelKsNormalized.Name = "labelKsNormalized";
            labelKsNormalized.Size = new Size(0, 15);
            labelKsNormalized.TabIndex = 8;
            labelKsNormalized.Visible = false;
            // 
            // animationTimer
            // 
            animationTimer.Interval = 30;
            // 
            // groupBoxDisplay
            // 
            groupBoxDisplay.Controls.Add(labelDivisions);
            groupBoxDisplay.Controls.Add(trackBarDivisions);
            groupBoxDisplay.Controls.Add(checkBoxPolygon);
            groupBoxDisplay.Controls.Add(checkBoxMesh);
            groupBoxDisplay.Controls.Add(checkBoxFilled);
            groupBoxDisplay.Location = new Point(630, 50);
            groupBoxDisplay.Name = "groupBoxDisplay";
            groupBoxDisplay.Size = new Size(220, 166);
            groupBoxDisplay.TabIndex = 21;
            groupBoxDisplay.TabStop = false;
            groupBoxDisplay.Text = "Display";
            // 
            // groupBoxRotation
            // 
            groupBoxRotation.Controls.Add(labelAlpha);
            groupBoxRotation.Controls.Add(trackBarAlpha);
            groupBoxRotation.Controls.Add(labelBeta);
            groupBoxRotation.Controls.Add(trackBarBeta);
            groupBoxRotation.Location = new Point(630, 225);
            groupBoxRotation.Name = "groupBoxRotation";
            groupBoxRotation.Size = new Size(220, 150);
            groupBoxRotation.TabIndex = 22;
            groupBoxRotation.TabStop = false;
            groupBoxRotation.Text = "Rotation";
            // 
            // groupBoxLighting
            // 
            groupBoxLighting.Controls.Add(labelKd);
            groupBoxLighting.Controls.Add(labelKdNormalized);
            groupBoxLighting.Controls.Add(trackBarKd);
            groupBoxLighting.Controls.Add(labelKs);
            groupBoxLighting.Controls.Add(labelKsNormalized);
            groupBoxLighting.Controls.Add(trackBarKs);
            groupBoxLighting.Controls.Add(labelEnergyConservation);
            groupBoxLighting.Controls.Add(labelM);
            groupBoxLighting.Controls.Add(trackBarM);
            groupBoxLighting.Location = new Point(630, 381);
            groupBoxLighting.Name = "groupBoxLighting";
            groupBoxLighting.Size = new Size(220, 235);
            groupBoxLighting.TabIndex = 23;
            groupBoxLighting.TabStop = false;
            groupBoxLighting.Text = "Lighting Parameters";
            // 
            // groupBoxObjectColor
            // 
            groupBoxObjectColor.Controls.Add(radioButtonSolidColor);
            groupBoxObjectColor.Controls.Add(buttonChooseColor);
            groupBoxObjectColor.Controls.Add(radioButtonTexture);
            groupBoxObjectColor.Controls.Add(checkBoxNormalMap);
            groupBoxObjectColor.Controls.Add(buttonLoadTexture);
            groupBoxObjectColor.Controls.Add(buttonLoadNormalMap);
            groupBoxObjectColor.Location = new Point(860, 50);
            groupBoxObjectColor.Name = "groupBoxObjectColor";
            groupBoxObjectColor.Size = new Size(220, 130);
            groupBoxObjectColor.TabIndex = 24;
            groupBoxObjectColor.TabStop = false;
            groupBoxObjectColor.Text = "Object Color";
            // 
            // groupBoxLight
            // 
            groupBoxLight.Controls.Add(labelLightZ);
            groupBoxLight.Controls.Add(trackBarLightZ);
            groupBoxLight.Controls.Add(checkBoxAnimateLight);
            groupBoxLight.Controls.Add(checkBoxShowLight);
            groupBoxLight.Controls.Add(buttonChooseLightColor);
            groupBoxLight.Location = new Point(860, 190);
            groupBoxLight.Name = "groupBoxLight";
            groupBoxLight.Size = new Size(220, 180);
            groupBoxLight.TabIndex = 25;
            groupBoxLight.TabStop = false;
            groupBoxLight.Text = "Light Source";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 625);
            Controls.Add(groupBoxLight);
            Controls.Add(groupBoxObjectColor);
            Controls.Add(groupBoxLighting);
            Controls.Add(groupBoxRotation);
            Controls.Add(groupBoxDisplay);
            Controls.Add(buttonLoadControlPoints);
            Controls.Add(pictureBox);
            Name = "Form1";
            Text = "Bézier Surface Renderer";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarDivisions).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBeta).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarKd).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarKs).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarM).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLightZ).EndInit();
            groupBoxDisplay.ResumeLayout(false);
            groupBoxDisplay.PerformLayout();
            groupBoxRotation.ResumeLayout(false);
            groupBoxRotation.PerformLayout();
            groupBoxLighting.ResumeLayout(false);
            groupBoxLighting.PerformLayout();
            groupBoxObjectColor.ResumeLayout(false);
            groupBoxObjectColor.PerformLayout();
            groupBoxLight.ResumeLayout(false);
            groupBoxLight.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox;
        private TrackBar trackBarDivisions;
        private TrackBar trackBarAlpha;
        private TrackBar trackBarBeta;
        private TrackBar trackBarKd;
        private TrackBar trackBarKs;
        private TrackBar trackBarM;
        private TrackBar trackBarLightZ;
        private CheckBox checkBoxPolygon;
        private CheckBox checkBoxMesh;
        private CheckBox checkBoxFilled;
        private CheckBox checkBoxAnimateLight;
        private CheckBox checkBoxShowLight;
        private RadioButton radioButtonSolidColor;
        private RadioButton radioButtonTexture;
        private CheckBox checkBoxNormalMap;
        private Button buttonLoadTexture;
        private Button buttonLoadNormalMap;
        private Button buttonChooseColor;
        private Button buttonLoadControlPoints;
        private Button buttonChooseLightColor;
        private Label labelDivisions;
        private Label labelAlpha;
        private Label labelBeta;
        private Label labelKd;
        private Label labelKs;
        private Label labelM;
        private Label labelLightZ;
        private Label labelEnergyConservation;
        private Label labelKdNormalized;
        private Label labelKsNormalized;
        private System.Windows.Forms.Timer animationTimer;
        private GroupBox groupBoxDisplay;
        private GroupBox groupBoxRotation;
        private GroupBox groupBoxLighting;
        private GroupBox groupBoxObjectColor;
        private GroupBox groupBoxLight;
    }
}
