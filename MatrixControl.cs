using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OpenTK;

namespace ShaderIDE
{
    public partial class MatrixControl : UserControl
    {
        //UI Controls for RotationBox
        private int LastMouseCaptureX, LastMouseCaptureY;
        private bool CapturingXY, CapturingZ;

        private oMat4 ExternalMatrix;

        private Matrix4 RotationMatrix;
        private Vector3 ExtractedScale, ExtractedTranslation;
        private float ExtractedFOV, ExtractedAR, ExtractedWidth, ExtractedHeight, ExtractedZFar, ExtractedZNear;
        
        private Vector4 OrthogonalCol3 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        private Vector4 PerspectiveCol3 = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);

        public MatrixControl()
        {
            InitializeComponent();
            RotationBox.Image = new Bitmap(64, 64);
        }

        private void SwitchToProjection()
        {
            NeartextBox.Enabled = FartextBox.Enabled = true;

            PosXtextBox.Enabled = PosYtextBox.Enabled = PosZtextBox.Enabled = false;
            ScaleXtextBox.Enabled = ScaleYtextBox.Enabled = ScaleZtextBox.Enabled = false;

            ClearRotationBox();
            RotationBox.Enabled = false;
        }
        
        private void Persp_CheckedChanged(object sender, EventArgs e)
        {
            SwitchToProjection();

            FOVtextBox.Enabled = true;
            FOVtrackBar.Enabled = true;

            WidthtextBox.Enabled = false;
            HeighttextBox.Enabled = false;
        }

        private void Ortho_CheckedChanged(object sender, EventArgs e)
        {
            SwitchToProjection();

            FOVtextBox.Enabled = false;
            FOVtrackBar.Enabled = false;

            WidthtextBox.Enabled = true;
            HeighttextBox.Enabled = true;
        }

        private void ModelView_CheckedChanged(object sender, EventArgs e)
        {
            NeartextBox.Enabled = false;
            FartextBox.Enabled = false;

            FOVtextBox.Enabled = false;
            FOVtrackBar.Enabled = false;

            WidthtextBox.Enabled = false;
            HeighttextBox.Enabled = false;

            PosXtextBox.Enabled = PosYtextBox.Enabled = PosZtextBox.Enabled = true;
            RotationBox.Enabled = true;
            UpdateRotationBox();
            ScaleXtextBox.Enabled = ScaleYtextBox.Enabled = ScaleZtextBox.Enabled = true;
        }

        private void FOVtrackBar_Scroll(object sender, EventArgs e)
        {
            FOVtextBox.Text = FOVtrackBar.Value.ToString("F1");
            if (sender != this) UpdateMatrix();
        }

        private void MatrixControl_Load(object sender, EventArgs e)
        {
            Persp_CheckedChanged(sender, e);
        }

        public void SelectMatrix4(oMat4 oMatrix)
        {
            //Ortho Projection (zfar znear width height)
            //Perspective Projection (zfar znear FieldOfView)
            //ModelView Projection (scale rotation translation)
         
            ExternalMatrix = oMatrix;

            var A = -ExternalMatrix.matrix.M33;
            var B = -ExternalMatrix.matrix.M43;

            if ((ExternalMatrix.matrix.Column3 == OrthogonalCol3) & (Math.Abs(ExternalMatrix.matrix.M41) < float.Epsilon) & (Math.Abs(ExternalMatrix.matrix.M42) < float.Epsilon) & (Math.Abs(B) > float.Epsilon))
            {
                //orthogonal projection

                var C = (B + 1.0f) / (B - 1.0f);

                ExtractedZFar = (2.0f / A) * 1 / ((1.0f - (1.0f / C)));
                ExtractedZNear = ExtractedZFar / C;

                //Width and Height for symetrical orthogonal matrices only
                ExtractedWidth = 2.0f*(1.0f/ExternalMatrix.matrix.M11);
                ExtractedHeight = 2.0f*(1.0f/ExternalMatrix.matrix.M22); 
                
                NeartextBox.Text = ExtractedZNear.ToString("F2");
                FartextBox.Text = ExtractedZFar.ToString("F2");
                WidthtextBox.Text = ExtractedWidth.ToString("F2");
                HeighttextBox.Text = ExtractedHeight.ToString("F2");

                OrthoRadioButton.Checked = true;
            }
            else if (ExternalMatrix.matrix.Column3 == PerspectiveCol3)
            {
                //Perpective projection

                // M11 = Focal Length = 1 / tan(FOV / 2)
                var FocalLength = ExternalMatrix.matrix.M11; 

                // M22 = FL / AR
                // AR = H / W
                ExtractedAR = FocalLength / ExternalMatrix.matrix.M22;
                ExtractedFOV = (float)Math.Atan(1.0f/FocalLength) * 2.0f;

                var C = (A - 1.0f) / (A + 1.0f);
                ExtractedZFar = (B / 2.0f) * ((1.0f / C) - 1.0f);
                ExtractedZNear = ExtractedZFar * C;

                NeartextBox.Text = ExtractedZNear.ToString("F2");
                FartextBox.Text = ExtractedZFar.ToString("F2");
                FOVtrackBar.Value = (int)Math.Round(MathHelper.RadiansToDegrees(ExtractedFOV));

                PerspRadioButton.Checked = true;
                FOVtrackBar_Scroll(this, new EventArgs());
            }
            else
            {
                //not a projection
                ExtractedTranslation = ExternalMatrix.matrix.ExtractTranslation();
                ExtractedScale = ExternalMatrix.matrix.ExtractScale();

                RotationMatrix = ExternalMatrix.matrix.ClearScale().ClearTranslation();

                PosXtextBox.Text = ExtractedTranslation.X.ToString("F2");
                PosYtextBox.Text = ExtractedTranslation.Y.ToString("F2");
                PosZtextBox.Text = ExtractedTranslation.Z.ToString("F2");

                ScaleXtextBox.Text = ExtractedScale.X.ToString("F2");
                ScaleYtextBox.Text = ExtractedScale.Y.ToString("F2");
                ScaleZtextBox.Text = ExtractedScale.Z.ToString("F2");

                UpdateRotationBox();

                ModelviewRadioButton.Checked = true;
            }
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            LastMouseCaptureX = e.X;
            LastMouseCaptureY = e.Y;
            CapturingXY = ((e.Button == MouseButtons.Left) & !CapturingZ);
            CapturingZ = ((e.Button == MouseButtons.Right) & !CapturingXY);
        }


        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) & (CapturingXY))
            {
                CapturingXY = false;
                UpdateMatrix();
            }
            else if ((e.Button == MouseButtons.Right) & (CapturingZ))
            {
                CapturingZ = false;
                UpdateMatrix();
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (CapturingXY | CapturingZ)
            {
                CapturingXY = false;
                CapturingZ = false;
                UpdateMatrix();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (CapturingXY)
            {
                RotationMatrix *= Matrix4.CreateRotationX((LastMouseCaptureY - e.Y)/ 20.0f);
                RotationMatrix *= Matrix4.CreateRotationY((LastMouseCaptureX - e.X) / -20.0f);
                UpdateRotationBox();
              
                LastMouseCaptureX = e.X;
                LastMouseCaptureY = e.Y;

            }
            else if (CapturingZ)
            {
                RotationMatrix *= Matrix4.CreateRotationZ((LastMouseCaptureY - e.Y) / -20.0f);
                UpdateRotationBox();

                LastMouseCaptureY = e.Y;
            }
        }



        private void UpdateMatrix()
        {
            if (PerspRadioButton.Checked)
            {   //TODO Convert actual textboxes data
                ExternalMatrix.matrix = Matrix4.CreatePerspectiveFieldOfView(
                    ExtractedFOV, ExtractedAR, ExtractedZNear, ExtractedZFar);
            }
            else if (OrthoRadioButton.Checked)
            {   //TODO Convert actual textboxes data
                ExternalMatrix.matrix = Matrix4.CreateOrthographic(
                    ExtractedWidth, ExtractedHeight, ExtractedZNear, ExtractedZFar);
            }
                //TODO ModelViewMatrix
            else
            {
                
            }
        }

        private void UpdateRotationBox()
        {
            var vx = Vector3.Transform(Vector3.UnitX * 20, RotationMatrix);
            var vy = Vector3.Transform(-Vector3.UnitY * 20, RotationMatrix);
            var vz = Vector3.Transform(Vector3.UnitZ * 20, RotationMatrix);

            //Perspective 
            vx *= (1.0f + (vx.Z * 0.025f));
            vy *= (1.0f + (vy.Z * 0.025f));
            vz *= (1.0f + (vz.Z * 0.025f));

            //Translate to View Center
            var vt = new Vector3(32, 32, 0);
            vx += vt;
            vy += vt;
            vz += vt;

            using (var GraphicFromRotationBoxImage = Graphics.FromImage(RotationBox.Image))
            {
                GraphicFromRotationBoxImage.Clear(Color.DimGray);
                GraphicFromRotationBoxImage.DrawLine(new Pen(Color.Red), new Point(32, 32), new Point((int)vx.X, (int)vx.Y));
                GraphicFromRotationBoxImage.DrawLine(new Pen(Color.Lime), new Point(32, 32), new Point((int)vy.X, (int)vy.Y));
                GraphicFromRotationBoxImage.DrawLine(new Pen(Color.Blue), new Point(32, 32), new Point((int)vz.X, (int)vz.Y));
                GraphicFromRotationBoxImage.DrawImage(RotationBox.Image, 0, 0, 64, 64);
                RotationBox.Invalidate();
                RotationBox.Update();
            }
        }

        private void ClearRotationBox()
        {
            using (var GraphicFromRotationBoxImage = Graphics.FromImage(RotationBox.Image))
            {
                GraphicFromRotationBoxImage.Clear(Color.DimGray);
                GraphicFromRotationBoxImage.DrawImage(RotationBox.Image, 0, 0, 64, 64);
                RotationBox.Invalidate();
                RotationBox.Update();
            }
        }




    }
}
