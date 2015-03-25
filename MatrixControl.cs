using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;

namespace ShaderIDE
{
    public partial class MatrixControl : UserControl
    {
        //UI Controls for RotationBox
        private int LastMouseCaptureX, LastMouseCaptureY;
        private bool CapturingXY, CapturingZ;

        private IBoxMatrix ExternalMatrix;
        private MatrixData ExternalData;

        private Pen RedPen, BluePen, GreenPen;
        private const float MidPoint = 32.0f;

        private struct ColoredVector
        {
            public Vector3 Vc;
            public Pen Pn;
        }

        public MatrixControl()
        {
            InitializeComponent();
            //TODO update with OnPaint instead of buffering into bitmap
            RotationBox.Image = new Bitmap(64, 64);
            RedPen = new Pen(Color.Red, 3.0f);
            GreenPen = new Pen(Color.Lime, 3.0f);
            BluePen = new Pen(Color.Blue, 3.0f);
        }

        private void SwitchToZero()
        {
            FOVtextBox.Enabled = FOVtextBox.Enabled = ARtextBox.Enabled = false;
            WidthtextBox.Enabled = HeighttextBox.Enabled = false;
            NeartextBox.Enabled = FartextBox.Enabled = false;
            PosXtextBox.Enabled = PosYtextBox.Enabled = PosZtextBox.Enabled = false;
            ScaleXtextBox.Enabled = ScaleYtextBox.Enabled = ScaleZtextBox.Enabled = false;
            ClearRotationBox();
            RotationBox.Enabled = false;
            PerspRadioButton.Checked = OrthoRadioButton.Checked = ModelviewRadioButton.Checked = false;
            ZeroButton.ForeColor = Color.Red;
        }

        private void SwitchToProjection()
        {
            NeartextBox.Enabled = FartextBox.Enabled = true;

            PosXtextBox.Enabled = PosYtextBox.Enabled = PosZtextBox.Enabled = false;
            ScaleXtextBox.Enabled = ScaleYtextBox.Enabled = ScaleZtextBox.Enabled = false;

            ClearRotationBox();
            RotationBox.Enabled = false;

            ZeroButton.ForeColor = Color.Black;
        }

        private void Persp_CheckedChanged(object sender, EventArgs e)
        {
            SwitchToProjection();

            FOVtextBox.Enabled = true;
            FOVtrackBar.Enabled = true;

            ARtextBox.Enabled = true;

            WidthtextBox.Enabled = false;
            HeighttextBox.Enabled = false;

            UpdateMatrix();
        }

        private void Ortho_CheckedChanged(object sender, EventArgs e)
        {
            SwitchToProjection();

            FOVtextBox.Enabled = false;
            FOVtrackBar.Enabled = false;

            ARtextBox.Enabled = false;

            WidthtextBox.Enabled = true;
            HeighttextBox.Enabled = true;

            UpdateMatrix();
        }

        private void ModelView_CheckedChanged(object sender, EventArgs e)
        {
            NeartextBox.Enabled = false;
            FartextBox.Enabled = false;

            FOVtextBox.Enabled = false;
            FOVtrackBar.Enabled = false;

            ARtextBox.Enabled = false;

            WidthtextBox.Enabled = false;
            HeighttextBox.Enabled = false;

            PosXtextBox.Enabled = PosYtextBox.Enabled = PosZtextBox.Enabled = true;
            if (ExternalData.Rotation == Matrix4.Zero) ExternalData.Rotation = Matrix4.Identity;
            RotationBox.Enabled = true;
            UpdateRotationBox();
            ScaleXtextBox.Enabled = ScaleYtextBox.Enabled = ScaleZtextBox.Enabled = true;
            ZeroButton.ForeColor = Color.Black;

            UpdateMatrix();
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

        public void SelectMatrix(IBoxMatrix oMatrix)
        {
            //Ortho Projection (zfar znear width height)
            //Perspective Projection (zfar znear FieldOfView)
            //ModelView Projection (scale rotation translation)

            ExternalMatrix = oMatrix;
            ExternalData = ExternalMatrix.ExtractData();

            if (ExternalData.Type == MatrixType.Zero)
            {
                SwitchToZero();
            }
            else if (ExternalData.Type == MatrixType.Ortho)
            {
                NeartextBox.Text = ExternalData.ZNear.ToString("F2");
                FartextBox.Text = ExternalData.ZFar.ToString("F2");
                WidthtextBox.Text = ExternalData.Width.ToString("F2");
                HeighttextBox.Text = ExternalData.Height.ToString("F2");
                OrthoRadioButton.Checked = true;
                Ortho_CheckedChanged(this, new EventArgs());
            }
            else if (ExternalData.Type == MatrixType.Persp)
            {
                NeartextBox.Text = ExternalData.ZNear.ToString("F2");
                FartextBox.Text = ExternalData.ZFar.ToString("F2");
                FOVtrackBar.Value = (int)Math.Round(MathHelper.RadiansToDegrees(ExternalData.FieldOfView));
                ARtextBox.Text = ExternalData.AspectRatio.ToString("F3");

                PerspRadioButton.Checked = true;
                Persp_CheckedChanged(this, new EventArgs());
                FOVtrackBar_Scroll(this, new EventArgs());
            }
            else if(ExternalData.Type == MatrixType.ModelView)
            {
                PosXtextBox.Text = ExternalData.Translation.X.ToString("F2");
                PosYtextBox.Text = ExternalData.Translation.Y.ToString("F2");
                PosZtextBox.Text = ExternalData.Translation.Z.ToString("F2");

                ScaleXtextBox.Text = ExternalData.Scale.X.ToString("F2");
                ScaleYtextBox.Text = ExternalData.Scale.Y.ToString("F2");
                ScaleZtextBox.Text = ExternalData.Scale.Z.ToString("F2");

                UpdateRotationBox();
                ModelviewRadioButton.Checked = true;
                ModelView_CheckedChanged(this, new EventArgs());
            }
        }


        private void RotationBox_MouseDown(object sender, MouseEventArgs e)
        {
            LastMouseCaptureX = e.X;
            LastMouseCaptureY = e.Y;
            CapturingXY = ((e.Button == MouseButtons.Left) & !CapturingZ);
            CapturingZ = ((e.Button == MouseButtons.Right) & !CapturingXY);
        }


        private void RotationBox_MouseUp(object sender, MouseEventArgs e)
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

        private void RotationBox_MouseLeave(object sender, EventArgs e)
        {
            if (CapturingXY | CapturingZ)
            {
                CapturingXY = false;
                CapturingZ = false;
                UpdateMatrix();
            }
        }

        private void RotationBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (CapturingXY)
            {
                ExternalData.Rotation *= Matrix4.CreateRotationX((LastMouseCaptureY - e.Y) / 20.0f);
                ExternalData.Rotation *= Matrix4.CreateRotationY((LastMouseCaptureX - e.X) / -20.0f);
                UpdateRotationBox();

                LastMouseCaptureX = e.X;
                LastMouseCaptureY = e.Y;

            }
            else if (CapturingZ)
            {
                ExternalData.Rotation *= Matrix4.CreateRotationZ((LastMouseCaptureY - e.Y) / -20.0f);
                UpdateRotationBox();

                LastMouseCaptureY = e.Y;
            }
        }



        private void UpdateMatrix()
        {
            if (ExternalData != null)
            {
                if (PerspRadioButton.Checked)
                {
                    ExternalData.FieldOfView = FOVtrackBar.Value;
                    TryExtractZ();
                    ExternalData.AspectRatio = TryParseTextBox(ARtextBox, ExternalData.AspectRatio);

                    ExternalData.Type = MatrixType.Persp;
                    ExternalMatrix.CreateMatrix(ExternalData);
                }
                else if (OrthoRadioButton.Checked)
                {
                    TryExtractSize();
                    TryExtractZ();

                    ExternalData.Type = MatrixType.Ortho;
                    ExternalMatrix.CreateMatrix(ExternalData);
                }
                else if (ModelviewRadioButton.Checked)
                {
                    ExternalData.Scale = new Vector3(TryParseTextBox(ScaleXtextBox, 1.0f),
                        TryParseTextBox(ScaleYtextBox, 1.0f), TryParseTextBox(ScaleZtextBox, 1.0f));
                    ExternalData.Translation = new Vector3(TryParseTextBox(PosXtextBox, 0.0f),
                        TryParseTextBox(PosYtextBox, 0.0f), TryParseTextBox(PosZtextBox, 0.0f));

                    ExternalData.Type = MatrixType.ModelView;
                    ExternalMatrix.CreateMatrix(ExternalData);
                }
                else
                {
                    ExternalData.Type = MatrixType.Zero;
                    ExternalMatrix.CreateMatrix(ExternalData);
                }
            }
        }

        private void TryExtractZ()
        {
            var zfar = TryParseTextBox(FartextBox, ExternalData.ZNear);
            var znear = TryParseTextBox(NeartextBox, ExternalData.ZFar);
            if ((zfar > znear) & (Math.Abs(znear) > float.Epsilon) & (Math.Abs(zfar) > float.Epsilon))
            {
                ExternalData.ZNear = znear;
                ExternalData.ZFar = zfar;
            }
        }

        private void TryExtractSize()
        {
            var width = TryParseTextBox(WidthtextBox, ExternalData.Width);
            var height = TryParseTextBox(HeighttextBox, ExternalData.Height);
            if ((width > 0.0f) & (height > 0.0f))
            {
                ExternalData.Width = width;
                ExternalData.Height = height;
            }

        }


        private static float TryParseTextBox(TextBox box, float fallback)
        {
            float output;
            return float.TryParse(box.Text, out output) ? output : fallback;
        }

        private void UpdateRotationBox()
        {
            var axis = new[]
            {
            new ColoredVector{
                Vc = Vector3.Transform(Vector3.UnitX * 22, ExternalData.Rotation),
                Pn = RedPen
            },
            new ColoredVector
            {
                Vc = Vector3.Transform(-Vector3.UnitY * 22, ExternalData.Rotation),
                Pn = GreenPen
            },
            new ColoredVector
            {
                Vc = Vector3.Transform(Vector3.UnitZ * 22, ExternalData.Rotation),
                Pn = BluePen
            }
            };
            
            for (var i = 0; i < axis.Length; i++)
            {
                //Perspective 
                axis[i].Vc *= (1.0f + (axis[i].Vc.Z * 0.025f));
                //Translate to View Center
                axis[i].Vc.X += 32.0f;
                axis[i].Vc.Y += 32.0f;
            }

            axis = ZOrderVectors(axis);

            using (var GraphicFromRotationBoxImage = Graphics.FromImage(RotationBox.Image))
            {
                GraphicFromRotationBoxImage.Clear(Color.DimGray);
                for (var i = 0; i < axis.Length; i++)
                {
                    GraphicFromRotationBoxImage.DrawLine(axis[i].Pn, MidPoint, MidPoint, axis[i].Vc.X, axis[i].Vc.Y);
                }
                GraphicFromRotationBoxImage.DrawImage(RotationBox.Image, 0, 0, 64, 64);
                RotationBox.Invalidate();
                RotationBox.Update();
            }
        }

        private ColoredVector[] ZOrderVectors(ColoredVector[] vectors)
        {   //Bubble sort FTW !
            if (vectors.Length > 1)
            {
                for (var i = vectors.Length - 1; i > 0; i--)
                {
                    for (var j = 0; j < i; j++)
                    {
                        if (vectors[j + 1].Vc.Z < vectors[j].Vc.Z)
                        {
                            var bufferVector = vectors[j];
                            vectors[j] = vectors[j + 1];
                            vectors[j + 1] = bufferVector;
                        }
                    }
                }
            }
            return vectors;
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

        private void ZeroButton_Click(object sender, EventArgs e)
        {
            SwitchToZero();
            UpdateMatrix();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var senderTextBox = sender as TextBox;
            if (senderTextBox != null)
            {
                float dummy;
                if (float.TryParse(senderTextBox.Text, out dummy))
                {
                    UpdateMatrix();
                }
            }
        }


    }
}
