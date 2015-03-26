using System;
using System.Drawing;
using OpenTK;

namespace ShaderIDE
{
        public class BoxBool
        {
            public bool Bool { get; set; }
        }
        public class BoxInt
        {
            public int Int { get; set; }
        }
        public class BoxFloat
        {
            public float Float { get; set; }
        }
        public class BoxVec2
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
        public class BoxVec3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }
        public class BoxVec4
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }
        }
        public class BoxColor
        {
            public Color color { get; set; }
        }
        public enum MatrixType
        {
            Zero, Ortho, Persp, ModelView
        }
        public class MatrixData
        {
            public MatrixType Type = MatrixType.Zero;
            public Matrix4 Rotation;
            public Vector3 Scale, Translation;
            public float FieldOfView, AspectRatio, Width, Height, ZFar, ZNear;
        }
        public interface IBoxMatrix
        {
            MatrixData ExtractData();
            void CreateMatrix(MatrixData Data);
        }
        public class BoxMatrix3 : IBoxMatrix
        {
            public Matrix3 matrix { get; set; }
            public void zero()
            {
                matrix = Matrix3.Zero;
            }
            public void CreateMatrix(MatrixData Data)
            {
                if (Data.Type == MatrixType.Zero)
                {
                    matrix = Matrix3.Zero;
                }
                else if (Data.Type == MatrixType.Persp)
                {
                    matrix = Matrix3.Identity;
                }
                else if (Data.Type == MatrixType.Ortho)
                {
                    matrix = Matrix3.Identity;
                }
                else if (Data.Type == MatrixType.ModelView)
                {
                    matrix = Matrix3.Identity;
                    matrix *= Matrix3.CreateScale(Data.Scale);
                    matrix *= new Matrix3(Data.Rotation);
                }
            }
            public MatrixData ExtractData()
            {
                var matrixData = new MatrixData();
                if (matrix == Matrix3.Zero)
                {
                    matrixData.Type = MatrixType.Zero;
                }
                else
                {
                    matrixData.Type = MatrixType.ModelView;
                    matrixData.Translation = new Vector3(0, 0, 0);
                    matrixData.Scale = matrix.ExtractScale();
                    var m3Buffer = matrix.ClearScale();
                    matrixData.Rotation = new Matrix4(new Vector4(m3Buffer.Row0, 0), new Vector4(m3Buffer.Row1, 0), new Vector4(m3Buffer.Row2, 0), new Vector4(0));
                    if (matrixData.Rotation == Matrix4.Zero) matrixData.Rotation = Matrix4.Identity;
                }
                return matrixData;
            }
        }
        public class BoxMatrix4 : IBoxMatrix
        {
            private Vector4 OrthogonalCol3 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            private Vector4 PerspectiveCol3 = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);

            public Matrix4 matrix { get; set; }
            public void CreateMatrix(MatrixData Data)
            {
                if (Data.Type == MatrixType.Zero)
                {
                    matrix = Matrix4.Zero;
                }
                else if (Data.Type == MatrixType.Persp)
                {
                    matrix = Matrix4.CreatePerspectiveFieldOfView(
                        MathHelper.DegreesToRadians(Data.FieldOfView), Data.AspectRatio, Data.ZNear, Data.ZFar);
                }
                else if (Data.Type == MatrixType.Ortho)
                {
                    matrix = Matrix4.CreateOrthographic(
                        Data.Width, Data.Height, Data.ZNear, Data.ZFar);
                }
                else if (Data.Type == MatrixType.ModelView)
                {
                    matrix = Matrix4.Identity;
                    matrix *= Matrix4.CreateScale(Data.Scale);
                    matrix *= Data.Rotation;
                    matrix *= Matrix4.CreateTranslation(Data.Translation);
                }
            }

            public MatrixData ExtractData()
            {
                var matrixData = new MatrixData();

                var A = -matrix.M33;
                var B = -matrix.M43;

                if ((matrix.Column3 == OrthogonalCol3) & (Math.Abs(matrix.M41) < float.Epsilon) & (Math.Abs(matrix.M42) < float.Epsilon) & (Math.Abs(B) > float.Epsilon))
                {
                    matrixData.Type = MatrixType.Ortho;
                    var C = (B + 1.0f) / (B - 1.0f);
                    matrixData.ZFar = (2.0f / A) * 1 / ((1.0f - (1.0f / C)));
                    matrixData.ZNear = matrixData.ZFar / C;

                    //Width and Height for symetrical orthogonal matrices only
                    matrixData.Width = 2.0f * (1.0f / matrix.M11);
                    matrixData.Height = 2.0f * (1.0f / matrix.M22);
                }
                else if (matrix.Column3 == PerspectiveCol3)
                {
                    matrixData.Type = MatrixType.Persp;
                    // M11 = Focal Length = 1 / tan(FOV / 2)
                    var FocalLength = matrix.M11;
                    // M22 = FL / 1/AR
                    // AR = W / H
                    matrixData.AspectRatio = matrix.M22 / FocalLength;
                    matrixData.FieldOfView = (float)Math.Atan(1.0f / (FocalLength * matrixData.AspectRatio)) * 2.0f;

                    var C = (A - 1.0f) / (A + 1.0f);
                    matrixData.ZFar = (B / 2.0f) * ((1.0f / C) - 1.0f);
                    matrixData.ZNear = matrixData.ZFar * C;
                }
                else if (matrix != Matrix4.Zero)
                {
                    matrixData.Type = MatrixType.ModelView;
                    matrixData.Translation = matrix.ExtractTranslation();
                    matrixData.Scale = matrix.ExtractScale();
                    matrixData.Rotation = matrix.ClearScale().ClearTranslation();

                    if (matrixData.Rotation == Matrix4.Zero) matrixData.Rotation = Matrix4.Identity;
                }
                return matrixData;
            }
        }
    }

