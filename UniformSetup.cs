using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ShaderIDE
{

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
    public interface IUniformProperty
    {
        string Name { get; set; }
        void EditProperty();
        void ToOpenGL(int UniformLocation);
    }

    #region Boxing Classes
    //Class to box Value Types inside Object and allow editing in PropertyEditor
    public class oBool
    {
        public bool Bool { get; set; }
    }
    public class oInt
    {
        public int Int { get; set; }
    }
    public class oFloat
    {
        public float Float { get; set; }
    }
    public class oVec2
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class oVec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
    public class oVec4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }
    public class oColor
    {
        public Color color { get; set; }
    }

    public interface oMat
    {            
        MatrixData ExtractData();
        void CreateMatrix(MatrixData Data);
    }
    public class oMat3 : oMat
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
                matrixData.Translation = new Vector3(0,0,0);
                matrixData.Scale = matrix.ExtractScale();
                var m3Buffer = matrix.ClearScale();
                matrixData.Rotation = new Matrix4(new Vector4(m3Buffer.Row0, 0), new Vector4(m3Buffer.Row1, 0), new Vector4(m3Buffer.Row2, 0), new Vector4(0));
                if (matrixData.Rotation == Matrix4.Zero) matrixData.Rotation = Matrix4.Identity;
            }
            return matrixData;
        }
    }
    public class oMat4 : oMat
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

    #endregion

    public static class UniformPropertyHelper
    {
        public static List<IUniformProperty> BuildDefaultList(PropertyGrid grid, MatrixControl ctrl)
        {
            var Data = new List<IUniformProperty>();
            Data.Add(new BoolUniformProperty("Bool_0", grid, false));
            Data.Add(new BoolUniformProperty("Bool_1", grid, false));
            Data.Add(new BoolUniformProperty("Bool_2", grid, false));
            Data.Add(new BoolUniformProperty("Bool_3", grid, false));

            Data.Add(new IntUniformProperty("Int_0", grid, 0));
            Data.Add(new IntUniformProperty("Int_1", grid, 0));
            Data.Add(new IntUniformProperty("Int_2", grid, 0));
            Data.Add(new IntUniformProperty("Int_3", grid, 0));

            Data.Add(new FloatUniformProperty("Float_0", grid, 0.0f));
            Data.Add(new FloatUniformProperty("Float_1", grid, 0.0f));
            Data.Add(new FloatUniformProperty("Float_2", grid, 0.0f));
            Data.Add(new FloatUniformProperty("Float_3", grid, 0.0f));

            Data.Add(new Vec4UniformProperty("Vec4_0", grid, new Vector4(0, 0, 0, 0)));
            Data.Add(new Vec4UniformProperty("Vec4_1", grid, new Vector4(0, 0, 0, 0)));
            Data.Add(new Vec4UniformProperty("Vec4_2", grid, new Vector4(0, 0, 0, 0)));
            Data.Add(new Vec4UniformProperty("Vec4_3", grid, new Vector4(0, 0, 0, 0)));

            Data.Add(new IntUniformProperty("Texture_0", grid, 0));
            Data.Add(new IntUniformProperty("Texture_1", grid, 0));
            Data.Add(new IntUniformProperty("Texture_2", grid, 0));
            Data.Add(new IntUniformProperty("Texture_3", grid, 0));

            Data.Add(new ColorUniformProperty("Color_0", grid, Color.Black));
            Data.Add(new ColorUniformProperty("Color_1", grid, Color.Red));
            Data.Add(new ColorUniformProperty("Color_2", grid, Color.Green));
            Data.Add(new ColorUniformProperty("Color_3", grid, Color.Blue));


            Data.Add(new Mat3UniformProperty("Mat3_0(empty)", ctrl, new Matrix3()));
            Data.Add(new Mat3UniformProperty("Mat3_1(identity)", ctrl, Matrix3.Identity));
            Data.Add(new Mat3UniformProperty("Mat3_2(scale)", ctrl, Matrix3.CreateScale(1.1f, 2.2f, 3.3f)));
            Data.Add(new Mat3UniformProperty("Mat3_3(rotation)", ctrl, Matrix3.CreateFromAxisAngle(new Vector3(1.0f, 1.0f, 1.0f), 45)));
            var mBuffer3 = Matrix3.CreateRotationZ(.43f);
            mBuffer3 *= Matrix3.CreateRotationX(0.41f);
            mBuffer3 *= Matrix3.CreateRotationY(0.42f);
            mBuffer3 *= Matrix3.CreateScale(1.1f, 1.2f, 1.3f);
            Data.Add(new Mat3UniformProperty("Mat4_4(mix)", ctrl,  mBuffer3));

            Data.Add(new Mat4UniformProperty("Mat4_0(empty)", ctrl, new Matrix4()));
            Data.Add(new Mat4UniformProperty("Mat4_1(identity)", ctrl, Matrix4.Identity));
            Data.Add(new Mat4UniformProperty("Mat4_2(translate)", ctrl,Matrix4.CreateTranslation(0.1f, 0.2f, 0.3f)));
            Data.Add(new Mat4UniformProperty("Mat4_3(rotateX)", ctrl, Matrix4.CreateRotationX(0.41f)));
            Data.Add(new Mat4UniformProperty("Mat4_4(rotateY)", ctrl, Matrix4.CreateRotationY(0.42f)));
            Data.Add(new Mat4UniformProperty("Mat4_5(rotateZ)", ctrl, Matrix4.CreateRotationZ(0.43f)));
            var mBuffer4 = Matrix4.CreateRotationZ(.43f);
            mBuffer4 *= Matrix4.CreateRotationX(0.41f);
            mBuffer4 *= Matrix4.CreateRotationY(0.42f);
            Data.Add(new Mat4UniformProperty("Mat4_6(rotateZXY)", ctrl, mBuffer4));
            Data.Add(new Mat4UniformProperty("Mat4_7(scale.5)", ctrl, Matrix4.CreateScale(0.5f)));
            Data.Add(new Mat4UniformProperty("Mat4_8(ortho)", ctrl, Matrix4.CreateOrthographic(640,480,1,256)));
            Data.Add(new Mat4UniformProperty("Mat4_9(45degpersp)", ctrl, Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75), 1920.0f/1080.0f ,1 ,256)));
            return Data;
        }
    }

    #region Classes
    //Properties Classes for Uniforms
    public class BoolUniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private oBool Value { get; set; }
        public string Name { get; set; }

        public BoolUniformProperty(string name, PropertyGrid grid, bool defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oBool { Bool = defaultValue };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform1(UniformLocation, (Value.Bool) ? 1 : 0);
        }
    }

    public class IntUniformProperty : IUniformProperty
    {
        private PropertyGrid Grid;
        public oInt Value;
        public string Name { get; set; }

        public IntUniformProperty(string name, PropertyGrid grid, int defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oInt { Int = defaultValue };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform1(UniformLocation, Value.Int);
        }
    }

    public class FloatUniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private oFloat Value;
        public string Name { get; set; }

        public FloatUniformProperty(string name, PropertyGrid grid, float defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oFloat { Float = defaultValue };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform1(UniformLocation, Value.Float);
        }
    }

    public class Vec2UniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private oVec2 Value;
        public string Name { get; set; }

        public Vec2UniformProperty(string name, PropertyGrid grid, Vector2 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oVec2
            {
                X = defaultValue.X,
                Y = defaultValue.Y
            };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform2(UniformLocation, Value.X, Value.Y);
        }
    }

    public class Vec3UniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private oVec3 Value;
        public string Name { get; set; }

        public Vec3UniformProperty(string name, PropertyGrid grid, Vector3 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oVec3
            {
                X = defaultValue.X,
                Y = defaultValue.Y,
                Z = defaultValue.Z
            };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform3(UniformLocation, Value.X, Value.Y, Value.Z);
        }
    }

    public class Vec4UniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private oVec4 Value;
        public string Name { get; set; }

        public Vec4UniformProperty(string name, PropertyGrid grid, Vector4 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oVec4
            {
                X = defaultValue.X,
                Y = defaultValue.Y,
                Z = defaultValue.Z,
                W = defaultValue.W
            };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform4(UniformLocation, Value.X, Value.Y, Value.Z, Value.W);
        }
    }

    public class ColorUniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private oColor Value;
        public string Name { get; set; }

        public ColorUniformProperty(string name, PropertyGrid grid, Color defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oColor { color = defaultValue };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform4(UniformLocation, Value.color.R, Value.color.G, Value.color.B, Value.color.A);
        }
    }

    public class Mat3UniformProperty : IUniformProperty
    {
        private readonly MatrixControl Grid;
        private oMat3 Value;
        public string Name { get; set; }

        public Mat3UniformProperty(string name, MatrixControl grid, Matrix3 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oMat3
            {
                matrix = defaultValue
            };
        }

        public void EditProperty()
        {
            Grid.SelectMatrix(Value);
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            var matrix3 = Value.matrix;
            GL.UniformMatrix3(UniformLocation, false, ref matrix3);
        }
    }
    public class Mat4UniformProperty : IUniformProperty
    {
        private readonly MatrixControl Grid;
        private oMat4 Value;
        public string Name { get; set; }

        public Mat4UniformProperty(string name, MatrixControl grid, Matrix4 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oMat4
            {
                matrix = defaultValue
            };
        }

        public void EditProperty()
        {
            Grid.SelectMatrix(Value);
            Grid.Enabled = true;
        }

        public void ToOpenGL(int UniformLocation)
        {
            var matrix4 = Value.matrix;
            GL.UniformMatrix4(UniformLocation, false, ref matrix4);
        }
    }
    #endregion
}
