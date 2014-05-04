using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace ShaderIDE
{
    static class ShaderLoader
    {
        static public int Load_Shader(string shaderText, ShaderType type)
        {
            var shaderOutput = GL.CreateShader(type);
            int _shaderResult;

            GL.ShaderSource(shaderOutput, shaderText);
            GL.CompileShader(shaderOutput);
            GL.GetShader(shaderOutput, ShaderParameter.CompileStatus, out _shaderResult);

            if (_shaderResult == 0)
            {
                Console.Message("Shader Compile Fail:");
                Console.Message("ERROR:" + GL.GetShaderInfoLog(shaderOutput));
                shaderOutput = -1;
            }
            return shaderOutput;
        }

        public static int Link_Program(List<int> shaderList)
        {
            Console.Message("Linking Program...");
            var programOutput = GL.CreateProgram();

            foreach (var shadersInt in shaderList)
            {
                GL.AttachShader(programOutput, shadersInt);
            }

            GL.LinkProgram(programOutput);

            int programResult;
            GL.GetProgram(programOutput, GetProgramParameterName.LinkStatus, out programResult);

            if (programResult == 0)
            {
                Console.Message("Program Link Fail:");
                Console.Message("ERROR:" + GL.GetProgramInfoLog(programOutput));
                programOutput = -1;
            }

            foreach (var shadersInt in shaderList)
            {
                GL.DetachShader(programOutput, shadersInt);
            }

            return programOutput;
        }

        static public int Load_Program(List<string> shaderList)
        {
            var _shaderList = new List<int>(shaderList.Count);
            var program = -1;
            var loadedShaders = 0;

            //Load shaders
            foreach (var currentShaderFilename in shaderList)
            {
                string shaderText;
                try
                {
                    shaderText = File.ReadAllText(currentShaderFilename);
                }
                catch (Exception oe)
                {
                    Console.Message("File Load Error: " + "\r\n" + oe.Message);
                    shaderText = "";
                }

                var currentShader = Load_Shader(shaderText, GetShaderType(currentShaderFilename));
                if (currentShader != -1)
                {
                    _shaderList.Add(currentShader);
                    loadedShaders++;
                }
            }

            //Link program
            if (loadedShaders == shaderList.Count)
            {
                program = Link_Program(_shaderList);
            }
            return program;
        }

        static public ShaderType GetShaderType(string s)
        {
            var extension = Path.GetExtension(s);
            if (extension != null)
            {
                var sExt = extension.ToUpper();
                if (sExt == ".COMP") return ShaderType.ComputeShader;
                if (sExt == ".FRAG") return ShaderType.FragmentShader;
                if (sExt == ".GEO") return ShaderType.GeometryShader;
                if (sExt == ".GEOEXT") return ShaderType.GeometryShaderExt;
                if (sExt == ".TESSCTRL") return ShaderType.TessControlShader;
                if (sExt == ".TESSEVAL") return ShaderType.TessEvaluationShader;
                if (sExt == ".VERT") return ShaderType.VertexShader;
                Console.Message("Unsupported ShaderType: " + extension);
            }
            else Console.Message("Error loading " + s);
            return ShaderType.VertexShader;
        }

        static public string GetShaderExt(ShaderType shaderType)
        {
            if (shaderType == ShaderType.ComputeShader) return ".comp";
            if (shaderType == ShaderType.FragmentShader) return ".frag";
            if (shaderType == ShaderType.GeometryShader) return ".geo";
            if (shaderType == ShaderType.GeometryShaderExt) return ".geoext";
            if (shaderType == ShaderType.TessControlShader) return ".tessctrl";
            if (shaderType == ShaderType.TessEvaluationShader) return ".tesseval";
            if (shaderType == ShaderType.VertexShader) return ".vert";
            Console.Message("Unsupported ShaderType: " + shaderType);
            return "";
        }


    }
}
