using System;
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
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }
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
        public static List<IUniformProperty> BuildDefaultList(PropertyGrid grid)
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

            Data.Add(new Mat2UniformProperty("Mat2_0", grid, new Matrix2()));
            Data.Add(new Mat2UniformProperty("Mat2_1", grid, Matrix2.Identity));
            Data.Add(new Mat2UniformProperty("Mat2_2", grid, Matrix2.CreateRotation(3.1415f)));
            Data.Add(new Mat2UniformProperty("Mat2_3", grid, Matrix2.CreateScale(0.5f)));

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
            Value = new oColor
            {
                R = defaultValue.R,
                G = defaultValue.G,
                B = defaultValue.B,
                A = defaultValue.A
            };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform4(UniformLocation, Value.R, Value.G, Value.B, Value.A);
        }
    }

    public class Mat2UniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private oMat2 Value;
        public string Name { get; set; }

        private Matrix2 Mat2;
        private static oVec2 Vector2Unwrapper(Vector2 vector)
        {
            return new oVec2 {X = vector.X, Y = vector.Y};
        }

        public Mat2UniformProperty(string name, PropertyGrid grid, Matrix2 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new oMat2
            {
                M00 = defaultValue.Row0.X,
                M01 = defaultValue.Row0.Y,
                M10 = defaultValue.Row1.X,
                M11 = defaultValue.Row1.Y
            };
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
        }

        public void ToOpenGL(int UniformLocation)
        {
            Mat2 = new Matrix2(Value.M00, Value.M01, Value.M10, Value.M11);
            GL.UniformMatrix2(UniformLocation, false, ref Mat2);
        }
    }
    #endregion
}
