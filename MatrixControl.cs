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
        private int LastMouseCaptureX, LastMouseCaptureY;
        private bool CapturingXY, CapturingZ;
        private float CaptureRotationX, CaptureRotationY, CaptureRotationZ;

        private Matrix4 RotationMatrix;
        private Vector3 ExtractedScale, ExtractedTranslation;
        private oMat4 ExternalMatrix;

        private Vector4 OrthogonalCol3 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        private Vector4 PerspectiveCol3 = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);

        public MatrixControl()
        {
            InitializeComponent();
            RotationBox.Image = new Bitmap(64, 64);
        }

        private void SwitchToProjection()
        {
            NeartextBox.Enabled = true;
            FartextBox.Enabled = true;

            PosXtextBox.Enabled = PosYtextBox.Enabled = PosZtextBox.Enabled = false;
            RotationBox.Enabled = false;
            ScaleXtextBox.Enabled = ScaleYtextBox.Enabled = ScaleZtextBox.Enabled = false;

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
            ScaleXtextBox.Enabled = ScaleYtextBox.Enabled = ScaleZtextBox.Enabled = true;
        }

        private void FOVtrackBar_Scroll(object sender, EventArgs e)
        {
            FOVtextBox.Text = FOVtrackBar.Value.ToString("F1");
        }

        private void MatrixControl_Load(object sender, EventArgs e)
        {
            Persp_CheckedChanged(sender, e);
        }

        public void SelectMatrix4(oMat4 oMatrix)
        {
            ExternalMatrix = oMatrix;

            var A = -ExternalMatrix.matrix.M33;
            var B = -ExternalMatrix.matrix.M43;
            if ((ExternalMatrix.matrix.Column3 == OrthogonalCol3) & (Math.Abs(ExternalMatrix.matrix.M41) < float.Epsilon) & (Math.Abs(ExternalMatrix.matrix.M42) < float.Epsilon) & (Math.Abs(B) > float.Epsilon))
            {
                //orthogonal projection
                var C = (B + 1.0f) / (B - 1.0f);
                var zFar = (2.0f / A) * 1 / ((1.0f - (1.0f / C)));
                var zNear = zFar / C;
                NeartextBox.Text = zNear.ToString("F2");
                FartextBox.Text = zFar.ToString("F2");
                //Width and Height for symetrical orthogonal matrices only
                WidthtextBox.Text = (2.0f * (1.0f / ExternalMatrix.matrix.M11)).ToString("F2");
                HeighttextBox.Text = (2.0f * (1.0f / ExternalMatrix.matrix.M22)).ToString("F2");
                OrthoRadioButton.Checked = true;
            }
            else if (ExternalMatrix.matrix.Column3 == PerspectiveCol3)
            {
                //Perpective projection
                var FL = ExternalMatrix.matrix.M11; // M11 = Focal Length = 1 / tan(FOV / 2)
                // M22 = e / a, a = Projection Aspect Ratio = H / W
                //var AR = FL/Matrix.M22;
                FOVtrackBar.Value = (int)Math.Round(Math.Atan(1.0f / FL) * 2 * 57.2957f);

                var C = (A - 1.0f) / (A + 1.0f);
                var zFar = (B / 2.0f) * ((1.0f / C) - 1.0f);
                var zNear = zFar * C;
                NeartextBox.Text = zNear.ToString("F2");
                FartextBox.Text = zFar.ToString("F2");
                PerspRadioButton.Checked = true;
                FOVtrackBar_Scroll(this, new EventArgs());
            }
            else
            {
                //not a projection
                var pos = ExternalMatrix.matrix.ExtractTranslation();
                PosXtextBox.Text = pos.X.ToString("F2");
                PosYtextBox.Text = pos.Y.ToString("F2");
                PosZtextBox.Text = pos.Z.ToString("F2");

                var sca = ExternalMatrix.matrix.ExtractScale();
                ScaleXtextBox.Text = sca.X.ToString("F2");
                ScaleYtextBox.Text = sca.Y.ToString("F2");
                ScaleZtextBox.Text = sca.Z.ToString("F2");

                /*
                Z heading = atan2(-m20,m00)
                x attitude = asin(m10)
                y bank = atan2(-m12,m11)

                except when M10=1 (north pole)
                which gives:
                z heading = atan2(M02,M22)
                y bank = 0

                and when M10=-1 (south pole)
                which gives:
                z heading = atan2(M02,M22)
                y bank = 0
                */

                var X_Ang = 0.0d;
                var Y_Ang = 0.0d;
                var Z_Ang = 0.0d;


                if (Math.Abs(ExternalMatrix.matrix.M13) > float.Epsilon)
                {
                    //X_Ang = 0;
                    Y_Ang = -Math.Asin(ExternalMatrix.matrix.M13);
                    //Z_Ang = 0;
                    var qMatrix = ExternalMatrix.matrix.ExtractRotation(true);
                    //qMatrix.ToAxisAngle()
                    ExternalMatrix.matrix *= Matrix4.CreateRotationY((float)-Y_Ang);
                }

                if (Math.Abs(ExternalMatrix.matrix.M23) > float.Epsilon)
                {
                    X_Ang = -Math.Asin(ExternalMatrix.matrix.M32);
                    //Y_Ang = Math.Atan2(-Matrix.M32, Matrix.M22);
                    //Z_Ang = Math.Atan2(-Matrix.M13, Matrix.M11);
                    ExternalMatrix.matrix *= Matrix4.CreateRotationY((float)-X_Ang);
                }

                if (Math.Abs(ExternalMatrix.matrix.M12) > float.Epsilon)
                {
                    //X_Ang = 0;
                    //Y_Ang = 0; 
                    Z_Ang = -Math.Asin(ExternalMatrix.matrix.M21);
                    //Matrix *= Matrix4.CreateRotationZ((float)Z_Ang);
                }


                ModelviewRadioButton.Checked = true;
                //MessageBox.Show(Matrix.ToString(), "Matrix Data", MessageBoxButtons.OK);
            }
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            LastMouseCaptureX = e.X;
            LastMouseCaptureY = e.Y;
            if (e.Button == MouseButtons.Left)
            {
                CapturingXY = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                CapturingZ = true;
            }
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
            UpdateMatrix();

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
                CaptureRotationX += (LastMouseCaptureY - e.Y) / 4.0f;
                CaptureRotationY += (LastMouseCaptureX - e.X) / 4.0f;
                LastMouseCaptureX = e.X;
                LastMouseCaptureY = e.Y;

            }
            else if (CapturingZ)
            {
                CaptureRotationZ -= (LastMouseCaptureY - e.Y) / 4.0f;
                LastMouseCaptureY = e.Y;
            }

            UpdateRotation();

            label5.Text = string.Format("X {0} Y {1} Z {2}", CaptureRotationX, CaptureRotationY, CaptureRotationZ);
        }



        private void UpdateMatrix()
        {
            RotationMatrix = Matrix4.Identity;
            RotationMatrix *= Matrix4.CreateScale(TryParseText(ScaleXtextBox.Text), TryParseText(ScaleYtextBox.Text), TryParseText(ScaleZtextBox.Text));


            CaptureRotationX = OverflowClamp(CaptureRotationX);
            CaptureRotationY = OverflowClamp(CaptureRotationY);
            CaptureRotationZ = OverflowClamp(CaptureRotationZ);

            RotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(CaptureRotationZ));
            RotationMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(CaptureRotationX));
            RotationMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-CaptureRotationY));

            RotationMatrix *= Matrix4.CreateTranslation(TryParseText(PosXtextBox.Text), TryParseText(PosYtextBox.Text), TryParseText(PosZtextBox.Text));
            ExternalMatrix.matrix = RotationMatrix;
        }

        private static float TryParseText(string number)
        {
            float float_number;
            return float.TryParse(number, out float_number) ? float_number : 0.0f;
        }

        private static float OverflowClamp(float value)
        {
            if (value > 360.0f) value -= 360.0f;
            if (value < 0.0f) value += 360.0f;
            return value;
        }

        private void UpdateRotation()
        {
            CaptureRotationX = OverflowClamp(CaptureRotationX);
            CaptureRotationY = OverflowClamp(CaptureRotationY);
            CaptureRotationZ = OverflowClamp(CaptureRotationZ);

            var rMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(CaptureRotationZ));
            rMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(CaptureRotationX));
            rMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-CaptureRotationY));

            var vx = Vector3.Transform(Vector3.UnitX * 20, rMatrix);
            var vy = Vector3.Transform(-Vector3.UnitY * 20, rMatrix);
            var vz = Vector3.Transform(Vector3.UnitZ * 20, rMatrix);

            //Perspective 
            vx *= (1.0f + (vx.Z * 0.025f));
            vy *= (1.0f + (vy.Z * 0.025f));
            vz *= (1.0f + (vz.Z * 0.025f));

            //Translate to View Center
            var vt = new Vector3(32, 32, 0);
            vx += vt;
            vy += vt;
            vz += vt;

            using (var g = Graphics.FromImage(RotationBox.Image))
            {
                g.Clear(Color.DimGray);
                g.DrawLine(new Pen(Color.Red), new Point(32, 32), new Point((int)vx.X, (int)vx.Y));
                g.DrawLine(new Pen(Color.Lime), new Point(32, 32), new Point((int)vy.X, (int)vy.Y));
                g.DrawLine(new Pen(Color.Blue), new Point(32, 32), new Point((int)vz.X, (int)vz.Y));
                g.DrawImage(RotationBox.Image, 0, 0, 64, 64);
                RotationBox.Invalidate();
                RotationBox.Update();
            }
        }
    }
}
