using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ShaderIDE
{
    public interface IUniformProperty
    {
        string Name { get; set; }
        void EditProperty();
        void ToOpenGL(int UniformLocation);
    }

    public static class UniformPropertyTester
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

            Data.Add(new Matrix3UniformProperty("Mat3_0(empty)", ctrl, new Matrix3()));
            Data.Add(new Matrix3UniformProperty("Mat3_1(identity)", ctrl, Matrix3.Identity));
            Data.Add(new Matrix3UniformProperty("Mat3_2(scale)", ctrl, Matrix3.CreateScale(1.1f, 2.2f, 3.3f)));
            Data.Add(new Matrix3UniformProperty("Mat3_3(rotation)", ctrl, Matrix3.CreateFromAxisAngle(new Vector3(1.0f, 1.0f, 1.0f), 45)));
            var mBuffer3 = Matrix3.CreateRotationZ(.43f);
            mBuffer3 *= Matrix3.CreateRotationX(0.41f);
            mBuffer3 *= Matrix3.CreateRotationY(0.42f);
            mBuffer3 *= Matrix3.CreateScale(1.1f, 1.2f, 1.3f);
            Data.Add(new Matrix3UniformProperty("Mat4_4(mix)", ctrl,  mBuffer3));

            Data.Add(new Matrix4UniformProperty("Mat4_0(empty)", ctrl, new Matrix4()));
            Data.Add(new Matrix4UniformProperty("Mat4_1(identity)", ctrl, Matrix4.Identity));
            Data.Add(new Matrix4UniformProperty("Mat4_2(translate)", ctrl,Matrix4.CreateTranslation(0.1f, 0.2f, 0.3f)));
            Data.Add(new Matrix4UniformProperty("Mat4_3(rotateX)", ctrl, Matrix4.CreateRotationX(0.41f)));
            Data.Add(new Matrix4UniformProperty("Mat4_4(rotateY)", ctrl, Matrix4.CreateRotationY(0.42f)));
            Data.Add(new Matrix4UniformProperty("Mat4_5(rotateZ)", ctrl, Matrix4.CreateRotationZ(0.43f)));
            var mBuffer4 = Matrix4.CreateRotationZ(.43f);
            mBuffer4 *= Matrix4.CreateRotationX(0.41f);
            mBuffer4 *= Matrix4.CreateRotationY(0.42f);
            Data.Add(new Matrix4UniformProperty("Mat4_6(rotateZXY)", ctrl, mBuffer4));
            Data.Add(new Matrix4UniformProperty("Mat4_7(scale.5)", ctrl, Matrix4.CreateScale(0.5f)));
            Data.Add(new Matrix4UniformProperty("Mat4_8(ortho)", ctrl, Matrix4.CreateOrthographic(640,480,1,256)));
            Data.Add(new Matrix4UniformProperty("Mat4_9(45degpersp)", ctrl, Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75), 1920.0f/1080.0f ,1 ,256)));
            return Data;
        }
    }

    #region Classes
    //Properties Classes for Uniforms
    public class BoolUniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private BoxBool Value { get; set; }
        public string Name { get; set; }

        public BoolUniformProperty(string name, PropertyGrid grid, bool defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxBool { Bool = defaultValue };
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
        public BoxInt Value;
        public string Name { get; set; }

        public IntUniformProperty(string name, PropertyGrid grid, int defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxInt { Int = defaultValue };
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
        private BoxFloat Value;
        public string Name { get; set; }

        public FloatUniformProperty(string name, PropertyGrid grid, float defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxFloat { Float = defaultValue };
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
        private BoxVec2 Value;
        public string Name { get; set; }

        public Vec2UniformProperty(string name, PropertyGrid grid, Vector2 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxVec2
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
        private BoxVec3 Value;
        public string Name { get; set; }

        public Vec3UniformProperty(string name, PropertyGrid grid, Vector3 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxVec3
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
        private BoxVec4 Value;
        public string Name { get; set; }

        public Vec4UniformProperty(string name, PropertyGrid grid, Vector4 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxVec4
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
        private BoxColor Value;
        public string Name { get; set; }

        public ColorUniformProperty(string name, PropertyGrid grid, Color defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxColor { color = defaultValue };
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

    public class Matrix3UniformProperty : IUniformProperty
    {
        private readonly MatrixControl Grid;
        private BoxMatrix3 Value;
        public string Name { get; set; }

        public Matrix3UniformProperty(string name, MatrixControl grid, Matrix3 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxMatrix3
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

    public class Matrix4UniformProperty : IUniformProperty
    {
        private readonly MatrixControl Grid;
        private BoxMatrix4 Value;
        public string Name { get; set; }

        public Matrix4UniformProperty(string name, MatrixControl grid, Matrix4 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new BoxMatrix4
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
