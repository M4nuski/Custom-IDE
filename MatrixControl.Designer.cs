namespace ShaderIDE
{
    partial class MatrixControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FartextBox = new System.Windows.Forms.TextBox();
            this.NeartextBox = new System.Windows.Forms.TextBox();
            this.OrthoRadioButton = new System.Windows.Forms.RadioButton();
            this.WidthtextBox = new System.Windows.Forms.TextBox();
            this.FOVtextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.HeighttextBox = new System.Windows.Forms.TextBox();
            this.PerspRadioButton = new System.Windows.Forms.RadioButton();
            this.FOVtrackBar = new System.Windows.Forms.TrackBar();
            this.ScaleZtextBox = new System.Windows.Forms.TextBox();
            this.ScaleYtextBox = new System.Windows.Forms.TextBox();
            this.ScaleXtextBox = new System.Windows.Forms.TextBox();
            this.PosZtextBox = new System.Windows.Forms.TextBox();
            this.PosYtextBox = new System.Windows.Forms.TextBox();
            this.PosXtextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ModelviewRadioButton = new System.Windows.Forms.RadioButton();
            this.RotationBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.FOVtrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FartextBox
            // 
            this.FartextBox.Location = new System.Drawing.Point(79, 187);
            this.FartextBox.Name = "FartextBox";
            this.FartextBox.Size = new System.Drawing.Size(49, 22);
            this.FartextBox.TabIndex = 34;
            this.FartextBox.Text = "256.0";
            // 
            // NeartextBox
            // 
            this.NeartextBox.Location = new System.Drawing.Point(24, 187);
            this.NeartextBox.Name = "NeartextBox";
            this.NeartextBox.Size = new System.Drawing.Size(49, 22);
            this.NeartextBox.TabIndex = 32;
            this.NeartextBox.Text = "1.0";
            // 
            // OrthoRadioButton
            // 
            this.OrthoRadioButton.AutoSize = true;
            this.OrthoRadioButton.Location = new System.Drawing.Point(3, 96);
            this.OrthoRadioButton.Name = "OrthoRadioButton";
            this.OrthoRadioButton.Size = new System.Drawing.Size(167, 21);
            this.OrthoRadioButton.TabIndex = 26;
            this.OrthoRadioButton.Text = "Orthogonal Projection";
            this.OrthoRadioButton.UseVisualStyleBackColor = true;
            this.OrthoRadioButton.CheckedChanged += new System.EventHandler(this.Ortho_CheckedChanged);
            // 
            // WidthtextBox
            // 
            this.WidthtextBox.Location = new System.Drawing.Point(24, 141);
            this.WidthtextBox.Name = "WidthtextBox";
            this.WidthtextBox.Size = new System.Drawing.Size(49, 22);
            this.WidthtextBox.TabIndex = 29;
            this.WidthtextBox.Text = "1.0";
            // 
            // FOVtextBox
            // 
            this.FOVtextBox.Enabled = false;
            this.FOVtextBox.Location = new System.Drawing.Point(65, 30);
            this.FOVtextBox.Name = "FOVtextBox";
            this.FOVtextBox.Size = new System.Drawing.Size(64, 22);
            this.FOVtextBox.TabIndex = 36;
            this.FOVtextBox.Text = "45.0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 120);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 17);
            this.label10.TabIndex = 35;
            this.label10.Text = "Width / Height";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 166);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 17);
            this.label8.TabIndex = 33;
            this.label8.Text = "zNear / zFar";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 17);
            this.label3.TabIndex = 31;
            this.label3.Text = "FOV";
            // 
            // HeighttextBox
            // 
            this.HeighttextBox.Location = new System.Drawing.Point(79, 141);
            this.HeighttextBox.Name = "HeighttextBox";
            this.HeighttextBox.Size = new System.Drawing.Size(49, 22);
            this.HeighttextBox.TabIndex = 30;
            this.HeighttextBox.Text = "1.0";
            // 
            // PerspRadioButton
            // 
            this.PerspRadioButton.AutoSize = true;
            this.PerspRadioButton.Checked = true;
            this.PerspRadioButton.Location = new System.Drawing.Point(3, 3);
            this.PerspRadioButton.Name = "PerspRadioButton";
            this.PerspRadioButton.Size = new System.Drawing.Size(170, 21);
            this.PerspRadioButton.TabIndex = 27;
            this.PerspRadioButton.TabStop = true;
            this.PerspRadioButton.Text = "Perspective Projection";
            this.PerspRadioButton.UseVisualStyleBackColor = true;
            this.PerspRadioButton.CheckedChanged += new System.EventHandler(this.Persp_CheckedChanged);
            // 
            // FOVtrackBar
            // 
            this.FOVtrackBar.AutoSize = false;
            this.FOVtrackBar.Location = new System.Drawing.Point(3, 58);
            this.FOVtrackBar.Maximum = 359;
            this.FOVtrackBar.Minimum = 1;
            this.FOVtrackBar.Name = "FOVtrackBar";
            this.FOVtrackBar.Size = new System.Drawing.Size(136, 32);
            this.FOVtrackBar.TabIndex = 28;
            this.FOVtrackBar.TickFrequency = 10;
            this.FOVtrackBar.Value = 45;
            this.FOVtrackBar.Scroll += new System.EventHandler(this.FOVtrackBar_Scroll);
            // 
            // ScaleZtextBox
            // 
            this.ScaleZtextBox.Location = new System.Drawing.Point(67, 317);
            this.ScaleZtextBox.Name = "ScaleZtextBox";
            this.ScaleZtextBox.Size = new System.Drawing.Size(49, 22);
            this.ScaleZtextBox.TabIndex = 48;
            this.ScaleZtextBox.Text = "1.0";
            // 
            // ScaleYtextBox
            // 
            this.ScaleYtextBox.Location = new System.Drawing.Point(68, 289);
            this.ScaleYtextBox.Name = "ScaleYtextBox";
            this.ScaleYtextBox.Size = new System.Drawing.Size(49, 22);
            this.ScaleYtextBox.TabIndex = 47;
            this.ScaleYtextBox.Text = "1.0";
            // 
            // ScaleXtextBox
            // 
            this.ScaleXtextBox.Location = new System.Drawing.Point(68, 261);
            this.ScaleXtextBox.Name = "ScaleXtextBox";
            this.ScaleXtextBox.Size = new System.Drawing.Size(49, 22);
            this.ScaleXtextBox.TabIndex = 46;
            this.ScaleXtextBox.Text = "1.0";
            // 
            // PosZtextBox
            // 
            this.PosZtextBox.Location = new System.Drawing.Point(12, 317);
            this.PosZtextBox.Name = "PosZtextBox";
            this.PosZtextBox.Size = new System.Drawing.Size(49, 22);
            this.PosZtextBox.TabIndex = 42;
            this.PosZtextBox.Text = "1.0";
            // 
            // PosYtextBox
            // 
            this.PosYtextBox.Location = new System.Drawing.Point(12, 289);
            this.PosYtextBox.Name = "PosYtextBox";
            this.PosYtextBox.Size = new System.Drawing.Size(49, 22);
            this.PosYtextBox.TabIndex = 41;
            this.PosYtextBox.Text = "0.0";
            // 
            // PosXtextBox
            // 
            this.PosXtextBox.Location = new System.Drawing.Point(12, 261);
            this.PosXtextBox.Name = "PosXtextBox";
            this.PosXtextBox.Size = new System.Drawing.Size(49, 22);
            this.PosXtextBox.TabIndex = 40;
            this.PosXtextBox.Text = "0.0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(73, 241);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 17);
            this.label6.TabIndex = 39;
            this.label6.Text = "Scale";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(122, 241);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 17);
            this.label5.TabIndex = 38;
            this.label5.Text = "Rotation ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 17);
            this.label4.TabIndex = 37;
            this.label4.Text = "Position";
            // 
            // ModelviewRadioButton
            // 
            this.ModelviewRadioButton.AutoSize = true;
            this.ModelviewRadioButton.Location = new System.Drawing.Point(3, 215);
            this.ModelviewRadioButton.Name = "ModelviewRadioButton";
            this.ModelviewRadioButton.Size = new System.Drawing.Size(100, 21);
            this.ModelviewRadioButton.TabIndex = 49;
            this.ModelviewRadioButton.TabStop = true;
            this.ModelviewRadioButton.Text = "Model/View";
            this.ModelviewRadioButton.UseVisualStyleBackColor = true;
            this.ModelviewRadioButton.CheckedChanged += new System.EventHandler(this.ModelView_CheckedChanged);
            // 
            // RotationBox
            // 
            this.RotationBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.RotationBox.InitialImage = null;
            this.RotationBox.Location = new System.Drawing.Point(125, 261);
            this.RotationBox.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.RotationBox.Name = "RotationBox";
            this.RotationBox.Size = new System.Drawing.Size(48, 48);
            this.RotationBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RotationBox.TabIndex = 50;
            this.RotationBox.TabStop = false;
            this.RotationBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.RotationBox.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.RotationBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.RotationBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // MatrixControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.RotationBox);
            this.Controls.Add(this.ModelviewRadioButton);
            this.Controls.Add(this.ScaleZtextBox);
            this.Controls.Add(this.ScaleYtextBox);
            this.Controls.Add(this.ScaleXtextBox);
            this.Controls.Add(this.PosZtextBox);
            this.Controls.Add(this.PosYtextBox);
            this.Controls.Add(this.PosXtextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FartextBox);
            this.Controls.Add(this.NeartextBox);
            this.Controls.Add(this.OrthoRadioButton);
            this.Controls.Add(this.WidthtextBox);
            this.Controls.Add(this.FOVtextBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.HeighttextBox);
            this.Controls.Add(this.PerspRadioButton);
            this.Controls.Add(this.FOVtrackBar);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.Name = "MatrixControl";
            this.Size = new System.Drawing.Size(188, 349);
            this.Load += new System.EventHandler(this.MatrixControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FOVtrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FartextBox;
        private System.Windows.Forms.TextBox NeartextBox;
        private System.Windows.Forms.RadioButton OrthoRadioButton;
        private System.Windows.Forms.TextBox WidthtextBox;
        private System.Windows.Forms.TextBox FOVtextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox HeighttextBox;
        private System.Windows.Forms.RadioButton PerspRadioButton;
        private System.Windows.Forms.TrackBar FOVtrackBar;
        private System.Windows.Forms.TextBox ScaleZtextBox;
        private System.Windows.Forms.TextBox ScaleYtextBox;
        private System.Windows.Forms.TextBox ScaleXtextBox;
        private System.Windows.Forms.TextBox PosZtextBox;
        private System.Windows.Forms.TextBox PosYtextBox;
        private System.Windows.Forms.TextBox PosXtextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton ModelviewRadioButton;
        private System.Windows.Forms.PictureBox RotationBox;
    }
}
