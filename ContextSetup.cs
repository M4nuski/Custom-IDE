using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace ShaderIDE
{
    class ContextSetup
    {
        // ReSharper disable RedundantDefaultFieldInitializer
        // ReSharper disable ConvertToAutoProperty
        private byte _red = 8;
        [CategoryAttribute("Fragment Color Format"),
         Description("BPP")]
        public byte Red
        {
            get { return _red; }
            set { _red = value; }
        }
        private byte _green = 8;
        [CategoryAttribute("Fragment Color Format"),
         Description("BPP")]
        public byte Green
        {
            get { return _green; }
            set { _green = value; }
        }
        private byte _blue = 8;
        [CategoryAttribute("Fragment Color Format"),
         Description("BPP")]
        public byte Blue
        {
            get { return _blue; }
            set { _blue = value; }
        }
        private byte _alpha = 8;
        [CategoryAttribute("Fragment Color Format"),
         Description("BPP")]
        public byte Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

        private byte _ared = 8;
        [CategoryAttribute("Accumulator Color Format"),
         Description("BPP")]
        public byte AccumRed
        {
            get { return _ared; }
            set { _ared = value; }
        }
        private byte _agreen = 8;
        [CategoryAttribute("Accumulator Color Format"),
         Description("BPP")]
        public byte AccumGreen
        {
            get { return _agreen; }
            set { _agreen = value; }
        }
        private byte _ablue = 8;
        [CategoryAttribute("Accumulator Color Format"),
         Description("BPP")]
        public byte AccumBlue
        {
            get { return _ablue; }
            set { _ablue = value; }
        }
        private byte _aalpha = 0;
        [CategoryAttribute("Accumulator Color Format"),
         Description("BPP")]
        public byte AccumAlpha
        {
            get { return _aalpha; }
            set { _aalpha = value; }
        }


        private int _depth = 24;
        [CategoryAttribute("Buffers"),
         Description("Depth Buffer BPP")]
        public int Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        private int _stencil = 0;
        [CategoryAttribute("Buffers"),
         Description("Stencil Buffer BPP")]
        public int Stencil
        {
            get { return _stencil; }
            set { _stencil = value; }
        }
        
        private int _samples = 0;
        [CategoryAttribute("Anti-Aliasing"),
         Description("Samples Per Pixels")]
        public int Samples
        {
            get { return _samples; }
            set { _samples = value; }
        }

        private int _buffers = 2;
        [CategoryAttribute("Buffers"),
         Description("Number of Buffers to use.")]
        public int Buffers
        {
            get { return _buffers; }
            set { _buffers = value; }
        }

        private bool _stereo;
        [CategoryAttribute("Buffers"),
         Description("Stereoscopic Render")]
        public bool Stereo
        {
            get { return _stereo; }
            set { _stereo = value; }
        }

        private int _majorVersion = 3;
        [CategoryAttribute("Context"),
         Description("Major OpenGL Version")]
        public int MajorVersion
        {
            get { return _majorVersion; }
            set { _majorVersion = value; }
        }
        private int _minorVersion = 3;
        [CategoryAttribute("Context"),
         Description("Minor OpenGL Version")]
        public int MinorVersion
        {
            get { return _minorVersion; }
            set { _minorVersion = value; }
        }

        private bool _cullFace = true;
        [CategoryAttribute("Render"),
         Description("Enable Face Culling")]
        public bool CullFace
        {
            get { return _cullFace; }
            set { _cullFace = value; }
        }

        private bool _depthTest = true;
        [CategoryAttribute("Render"),
         Description("Enable Depth Testing")]
        public bool DepthTest
        {
            get { return _depthTest; }
            set { _depthTest = value; }
        }

        private bool _debugFlag = true;
        [CategoryAttribute("Context"),
         Description("Enable Debug Context")]
        public bool DebugFlag
        {
            get { return _debugFlag; }
            set { _debugFlag = value; }
        }

        private bool _fcFlag = true;
        [CategoryAttribute("Context"),
         Description("Enable Forward Compatible Context")]
        public bool ForwardCompatibleFlag
        {
            get { return _fcFlag; }
            set { _fcFlag = value; }
        }

        private bool _embFlag = false;
        [CategoryAttribute("Context"),
         Description("Enable Embedded Context")]
        public bool EmbeddedFlag
        {
            get { return _embFlag; }
            set { _embFlag = value; }
        }

        private Color _clearColor = Color.FromArgb(255, 64, 64, 64);
        [CategoryAttribute("Render"),
         Description("Buffer Clear Color")]
        public Color ClearColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }

        private MaterialFace _matFace = MaterialFace.FrontAndBack;
        [CategoryAttribute("Render"),
         Description("Material Face Side")]
        public MaterialFace MaterialFace
        {
            get { return _matFace; }
            set { _matFace = value; }
        }

        private PolygonMode _polyMode = PolygonMode.Fill;
        [CategoryAttribute("Render"),
         Description("Material Face Fill Mode")]
        public PolygonMode PolygonMode
        {
            get { return _polyMode; }
            set { _polyMode = value; }
        }

        private bool _clearColorBuffer = true;
        [CategoryAttribute("Render"),
         Description("Clear Color Buffer before render")]
        public bool ClearColorBuffer
        {
            get { return _clearColorBuffer; }
            set { _clearColorBuffer = value; }
        }

        private bool _clearDepthBuffer = true;
        [CategoryAttribute("Render"),
         Description("Clear Depth Buffer before render")]
        public bool ClearDepthBuffer
        {
            get { return _clearDepthBuffer; }
            set { _clearDepthBuffer = value; }
        }
        // ReSharper restore RedundantDefaultFieldInitializer
        // ReSharper restore ConvertToAutoProperty
    }
}
