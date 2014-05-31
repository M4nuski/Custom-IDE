using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace ShaderIDE
{
    public partial class MatrixControl : UserControl
    {
        private Vector4 OrthogonalCol3 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        private Vector4 PerspectiveCol3 = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);

        public MatrixControl()
        {
            InitializeComponent();
        }

        private void SwitchToProjection()
        {
            NeartextBox.Enabled = true;
            FartextBox.Enabled = true;

            PosXtextBox.Enabled = PosYtextBox.Enabled = PosZtextBox.Enabled = false;
            RotXtextBox.Enabled = RotYtextBox.Enabled = RotZtextBox.Enabled = false;
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
            RotXtextBox.Enabled = RotYtextBox.Enabled = RotZtextBox.Enabled = true;
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

        public void SelectMatrix4(ref Matrix4 Matrix)
        {
            if (Matrix.Column3 == OrthogonalCol3)
            {   
                //orthogonal projection //todo actual calculation
                var A = -Matrix.M33;
                var B = -Matrix.M43;
                var C = (A - 1.0f) / (A + 1.0f);
                var zFar = (B / 2.0f) * ((1.0f / C) - 1.0f);
                var zNear = zFar * C;
                NeartextBox.Text = zNear.ToString("F2");
                FartextBox.Text = zFar.ToString("F2");
            }
            else if (Matrix.Column3 == PerspectiveCol3)
            {   
                //Perpective projection
                var FL = Matrix.M11; // M11 = Focal Length = 1 / tan(FOV / 2)
                // M22 = e / a, a = Projection Aspect Ratio = H / W
                //var AR = FL/Matrix.M22;
                FOVtrackBar.Value = (int)Math.Round(Math.Atan(1.0f / FL) * 2 * 57.2957f);

                var A = -Matrix.M33;
                var B = -Matrix.M43;
                var C = (A - 1.0f) / (A + 1.0f);
                var zFar = (B / 2.0f) * ((1.0f / C) - 1.0f);
                var zNear = zFar * C;
                NeartextBox.Text = zNear.ToString("F2");
                FartextBox.Text = zFar.ToString("F2");

                FOVtrackBar_Scroll(this, new EventArgs());
            }
            //else
            //{   
                //not a projection //todo fill out only if both perspectives fail
                var pos = Matrix.ExtractTranslation();
                PosXtextBox.Text = pos.X.ToString("F2");
                PosYtextBox.Text = pos.Y.ToString("F2");
                PosZtextBox.Text = pos.Z.ToString("F2");

                var sca = Matrix.ExtractScale();
                ScaleXtextBox.Text = sca.X.ToString("F2");
                ScaleYtextBox.Text = sca.Y.ToString("F2");
                ScaleZtextBox.Text = sca.Z.ToString("F2");

                var ang = Matrix.ExtractRotation().ToAxisAngle();
                RotXtextBox.Text = ang.X.ToString("F2");
                RotYtextBox.Text = ang.Y.ToString("F2");
                RotZtextBox.Text = ang.Z.ToString("F2");
            //}
        }
    }
}
