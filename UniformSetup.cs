using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
    public interface IUniformProperty
    {
        string Name { get; set; }
        void EditProperty();
        void ToOpenGL(int UniformLocation);
    }

    public static class UniformPropertyHelper
    {
        public class oInt
        {
            public int Int { get; set; }
        }

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
            return Data;
        }
    }

    public class BoolUniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private bool Value { get; set; }
        public string Name { get; set; }

        public BoolUniformProperty(string name, PropertyGrid grid, bool defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = defaultValue;
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform1(UniformLocation, (Value) ? 1 : 0);
        }
    }

    public class IntUniformProperty : IUniformProperty
    {
        private PropertyGrid Grid;
        public UniformPropertyHelper.oInt Value;
        public string Name { get; set; }

        public IntUniformProperty(string name, PropertyGrid grid, int defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = new UniformPropertyHelper.oInt {Int = defaultValue};
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
        private float Value;
        public string Name { get; set; }

        public FloatUniformProperty(string name, PropertyGrid grid, float defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = defaultValue;
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform1(UniformLocation, Value);
        }
    }

    public class Vec4UniformProperty : IUniformProperty
    {
        private readonly PropertyGrid Grid;
        private Vector4 Value;
        public string Name { get; set; }

        public Vec4UniformProperty(string name, PropertyGrid grid, Vector4 defaultValue)
        {
            Name = name;
            Grid = grid;
            Value = defaultValue;
        }

        public void EditProperty()
        {
            Grid.SelectedObject = Value;
        }

        public void ToOpenGL(int UniformLocation)
        {
            GL.Uniform2(UniformLocation, new Vector2());
        }
    }

    /*
        Projection Matrix 
        View Matrix 
        Model Matrix 
        Normal Matrix 
        Projection - View
        Matrix
        Model - View
        Matrix
        Model - View - Projection
        Matrix
        Color[]
        Vextex[]
        Normal[]
        Tangent[]
        Bitangent[]
        UV[]
            Texture0 
        Texture1
            Texture2 
        Texture3
            Bool0 
        Bool1
            Bool2 
        Bool3
            Float0 
        Float1
            Float2 
        Float3
            Int0 
        Int1
            Int2 
        Int3
            Vec20 
        Vec21
            Vec22 
        Vec23
            Vec30 
        Vec31
            Vec32 
        Vec33
            Vec40 
        Vec41
            Vec42 
        Vec43
            Color0 
        Color1
            Color2 
        Color3
         * */
}
