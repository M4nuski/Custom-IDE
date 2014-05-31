﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ShaderIDE
{
    //Interface for Uniform Property
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
    public class oMat2
    {
        public float M00 { get; set; }
        public float M01 { get; set; }
        public float M10 { get; set; }
        public float M11 { get; set; }
    }
    public class oMat3
    {
        public oVec3 Row0 { get; set; }
        public oVec3 Row1 { get; set; }
        public oVec3 Row2 { get; set; }
    }
    public class oMat4
    {
        public oVec4 Row0 { get; set; }
        public oVec4 Row1 { get; set; }
        public oVec4 Row2 { get; set; }
        public oVec4 Row3 { get; set; }
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

            Data.Add(new Mat4UniformProperty("Mat4_0(empty)", ctrl, new Matrix4()));
            Data.Add(new Mat4UniformProperty("Mat4_1(identity)", ctrl, Matrix4.Identity));
            Data.Add(new Mat4UniformProperty("Mat4_2(translate)", ctrl,Matrix4.CreateTranslation(0.1f, 0.2f, 0.3f)));
            Data.Add(new Mat4UniformProperty("Mat4_3(rotateZ)", ctrl, Matrix4.CreateRotationZ(3.1415f)));
            Data.Add(new Mat4UniformProperty("Mat4_4(scale.5)", ctrl, Matrix4.CreateScale(0.5f)));
            Data.Add(new Mat4UniformProperty("Mat4_5(ortho)", ctrl, Matrix4.CreateOrthographic(20,20,1,10)));
            Data.Add(new Mat4UniformProperty("Mat4_6(45degpersp)", ctrl, Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 640/480,1,256)));
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
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform4(UniformLocation, Value.color.R, Value.color.G, Value.color.B, Value.color.A);
        }
    }

    public class Mat4UniformProperty : IUniformProperty
    {
        private readonly MatrixControl Grid;
        private Matrix4 Value;
        public string Name { get; set; }

        public Mat4UniformProperty(string name, MatrixControl grid, Matrix4 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = defaultValue;
        }

        public void EditProperty()
        {
            Grid.SelectMatrix4(ref Value);
        }

        public void ToOpenGL(int UniformLocation)
        {
            var Mat4 = new Matrix4(Value.M11, Value.M12, Value.M13, Value.M14,
                Value.M21, Value.M22, Value.M23, Value.M24,
                Value.M31, Value.M32, Value.M33, Value.M34,
                Value.M41, Value.M42, Value.M43, Value.M44);
            GL.UniformMatrix4(UniformLocation, false, ref Mat4);
        }
    }
    #endregion
}
