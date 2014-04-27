using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ShaderIDE
{
    #region Structs
    public struct FontAndColorStruct
    {
        public Font StyleFont;
        public Color StyleColor;

        public FontAndColorStruct(Font font, Color color)
        {
            StyleFont = font;
            StyleColor = color;
        }
    }

    public struct ThemeStruct // Main struct
    {
        public string Name;
        public DelimiterStruct[] Delimiters;
        public WordStruct[] Words;
        public SpanStruct[] Spans;
        public FontAndColorStruct TextStyle, ValueStyle;
        public Color BackgroundColor, CurrentLineColor;
    }

    public struct DelimiterStruct
    {
        public string Name;
        public char[] Keychars;
        public FontAndColorStruct Style;

        public DelimiterStruct(Font prototypeFont)
        {
            Name = "<new>";
            Keychars = new char[] {};
            Style = new FontAndColorStruct {StyleFont = prototypeFont};
        }
    }

    public struct WordStruct
    {
        public string Name;
        public string[] Keywords;
        public FontAndColorStruct Style;

        public WordStruct(Font prototypeFont)
        {
            Name = "<new>";
            Keywords = new string[] { };
            Style = new FontAndColorStruct {StyleFont = prototypeFont};
        }
    }

    public struct SpanStruct
    {
        public string Name;
        public string StartKeyword;
        public string StopKeyword; //or EOL
        public char EscapeChar;
        public FontAndColorStruct Style;

        public SpanStruct(Font prototypeFont)
        {
            Name = "<new>";
            StartKeyword = "";
            StopKeyword = "";
            EscapeChar = '\n';
            Style = new FontAndColorStruct {StyleFont = prototypeFont};
        }
    }

    public struct HighlightStruct
    {
        public int LineNumber;
        public string LineHint;
        public Color LineColor;

        public HighlightStruct(int lineNumber, string lineHint, Color lineColor)
        {
            LineNumber = lineNumber;
            LineColor = lineColor;
            LineHint = lineHint;
        }
        public HighlightStruct(int lineNumber, Color lineColor)
        {
            LineNumber = lineNumber;
            LineColor = lineColor;
            LineHint = "";
        }
    }

    public struct TokenStruct
    {
        public int Offset;
        public string Text;
        public FontAndColorStruct Style;
    }
    #endregion

    static class ThemeHelper
    {
        #region Helpers Methods
        static public bool StyleEqual(FontAndColorStruct a, FontAndColorStruct b)
        {
            return  (a.StyleColor == b.StyleColor) & (a.StyleFont.Equals(b.StyleFont));
        }

        static public bool TokenEqual(TokenStruct a, TokenStruct b)
        {
            return (a.Text == b.Text) & (StyleEqual(a.Style, b.Style));
        }

        static private void WriteColor(Color color, BinaryWriter writer)
        {
            writer.Write(color.A);
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        static private Color ReadColor(BinaryReader reader)
        {
            return Color.FromArgb(  reader.ReadByte(),
                                    reader.ReadByte(),
                                    reader.ReadByte(),
                                    reader.ReadByte());
        }

        static private void WriteStyle(FontAndColorStruct style, BinaryWriter writer)
        {
            WriteColor(style.StyleColor, writer);
            writer.Write(style.StyleFont.FontFamily.Name);
            writer.Write(style.StyleFont.Size);
            writer.Write(style.StyleFont.Bold);
            writer.Write(style.StyleFont.Italic);
            writer.Write(style.StyleFont.Strikeout);
            writer.Write(style.StyleFont.Underline);
        }

        static private FontAndColorStruct ReadStyle(BinaryReader reader)
        {
            var style = new FontAndColorStruct
            {
                StyleColor = ReadColor(reader)
            };

            var fontName = reader.ReadString();
            var fontEmSize = reader.ReadSingle();

            var styleBuffer = FontStyle.Regular;
            if (reader.ReadBoolean()) styleBuffer = styleBuffer | FontStyle.Bold;
            if (reader.ReadBoolean()) styleBuffer = styleBuffer | FontStyle.Italic;
            if (reader.ReadBoolean()) styleBuffer = styleBuffer | FontStyle.Strikeout;
            if (reader.ReadBoolean()) styleBuffer = styleBuffer | FontStyle.Underline;

            style.StyleFont = new Font(fontName, fontEmSize, styleBuffer);
            return style;
        }

        public static ThemeStruct ChangeFont(ThemeStruct theme, Font newfont)
        {
            var newTheme = theme;

            newTheme.TextStyle.StyleFont = newfont;
            var backupStyle = newTheme.ValueStyle.StyleFont.Style;
            newTheme.ValueStyle.StyleFont = new Font(newfont, backupStyle);

            //Update all tokens to use this new font.
            for (var i = 0; i < newTheme.Delimiters.Length; i++)
            {
                backupStyle = newTheme.Delimiters[i].Style.StyleFont.Style;
                newTheme.Delimiters[i].Style.StyleFont = new Font(newfont, backupStyle);
            }

            for (var i = 0; i < newTheme.Words.Length; i++)
            {
                backupStyle = newTheme.Words[i].Style.StyleFont.Style;
                newTheme.Words[i].Style.StyleFont = new Font(newfont, backupStyle);
            }

            for (var i = 0; i < newTheme.Spans.Length; i++)
            {
                backupStyle = newTheme.Spans[i].Style.StyleFont.Style;
                newTheme.Spans[i].Style.StyleFont = new Font(newfont, backupStyle);
            }
            return newTheme;
        }

        #endregion

        static public void SaveTheme(ThemeStruct theme, string filename)
        {
            var fileStream = File.Create(filename);
            var writer = new BinaryWriter(fileStream);
            
            writer.Write("CustomIDETheme"); //File header
            writer.Write(theme.Name);
           
            writer.Write("Delimiters");
            writer.Write(theme.Delimiters.Length);
            for (var i = 0; i < theme.Delimiters.Length; i++)
            {
                writer.Write(theme.Delimiters[i].Name);
                writer.Write(theme.Delimiters[i].Keychars.Length);
                foreach (var t in theme.Delimiters[i].Keychars)
                {
                    writer.Write(Convert.ToInt32(t));
                }
                WriteStyle(theme.Delimiters[i].Style, writer);
            }

            writer.Write("Words");
            writer.Write(theme.Words.Length);
            for (var i = 0; i < theme.Words.Length; i++)
            {
                writer.Write(theme.Words[i].Name);
                writer.Write(theme.Words[i].Keywords.Length);
                foreach (var t in theme.Words[i].Keywords)
                {
                    writer.Write(t);
                }
                WriteStyle(theme.Words[i].Style, writer);
            }

            writer.Write("Spans");
            writer.Write(theme.Spans.Length);
            for (var i = 0; i < theme.Spans.Length; i++)
            {
                writer.Write(theme.Spans[i].Name);
                writer.Write(theme.Spans[i].StartKeyword);
                writer.Write(theme.Spans[i].StopKeyword);
                writer.Write(Convert.ToInt32(theme.Spans[i].EscapeChar));
                WriteStyle(theme.Spans[i].Style, writer);
            }

            writer.Write("TextStyle");
            WriteStyle(theme.TextStyle, writer);
            writer.Write("ValueStyle");
            WriteStyle(theme.ValueStyle, writer);
            writer.Write("BackgroundColor");
            WriteColor(theme.BackgroundColor, writer);
            writer.Write("CurrentLineColor");
            WriteColor(theme.CurrentLineColor, writer);

            writer.Write("CustomIDEEndOfTheme");
            fileStream.Flush();
            fileStream.Dispose();
        }

        public static ThemeStruct LoadTheme(ThemeStruct originalTheme, string filename)
        {
            var theme = new ThemeStruct();
            var follower = "Init";
            try
            {
                var fileStream = File.OpenRead(filename);
                var reader = new BinaryReader(fileStream);
                if (reader.ReadString() == "CustomIDETheme") //File header
                {
                    follower = "Name";
                    theme.Name = reader.ReadString();

                    follower = "Delimiters";
                    if (reader.ReadString() == "Delimiters")
                    {
                        theme.Delimiters = new DelimiterStruct[reader.ReadInt32()];
                        for (var i = 0; i < theme.Delimiters.Length; i++)
                        {
                            theme.Delimiters[i].Name = reader.ReadString();
                            theme.Delimiters[i].Keychars = new char[reader.ReadInt32()];
                            for (var j = 0; j < theme.Delimiters[i].Keychars.Length; j++)
                            {
                                theme.Delimiters[i].Keychars[j] = Convert.ToChar(reader.ReadInt32());
                            }
                            theme.Delimiters[i].Style = ReadStyle(reader);
                        }
                    } 
                    else throw new Exception("File Structure Error.");

                    follower = "Words";
                    if (reader.ReadString() == "Words")
                    {
                        theme.Words = new WordStruct[reader.ReadInt32()];
                        for (var i = 0; i < theme.Words.Length; i++)
                        {
                            theme.Words[i].Name = reader.ReadString();
                            theme.Words[i].Keywords = new string[reader.ReadInt32()];
                            for (var j = 0; j < theme.Words[i].Keywords.Length; j++)
                            {
                                theme.Words[i].Keywords[j] = reader.ReadString();
                            }
                            theme.Words[i].Style = ReadStyle(reader);
                        }
                    }
                    else throw new Exception("File Structure Error.");

                    follower = "Spans";
                    if (reader.ReadString() == "Spans")
                    {
                        theme.Spans = new SpanStruct[reader.ReadInt32()];
                        for (var i = 0; i < theme.Spans.Length; i++)
                        {
                            theme.Spans[i].Name = reader.ReadString();
                            theme.Spans[i].StartKeyword = reader.ReadString();
                            theme.Spans[i].StopKeyword = reader.ReadString();
                            theme.Spans[i].EscapeChar = Convert.ToChar(reader.ReadInt32());
                            theme.Spans[i].Style = ReadStyle(reader);
                        }
                    }
                    else throw new Exception("File Structure Error.");

                    follower = "TextStyle";
                    if (reader.ReadString() == "TextStyle") theme.TextStyle = ReadStyle(reader); else throw new Exception("File Structure Error.");
                    follower = "ValueStyle";
                    if (reader.ReadString() == "ValueStyle") theme.ValueStyle = ReadStyle(reader); else throw new Exception("File Structure Error.");
                    follower = "BackgroundColor";
                    if (reader.ReadString() == "BackgroundColor") theme.BackgroundColor = ReadColor(reader); else throw new Exception("File Structure Error.");
                    follower = "CurrentLineColor";
                    if (reader.ReadString() == "CurrentLineColor") theme.CurrentLineColor = ReadColor(reader); else throw new Exception("File Structure Error.");
                } else throw new Exception("Bad file header.");

                follower = "CustomIDEEndOfTheme";
                if (reader.ReadString() != "CustomIDEEndOfTheme") throw new Exception("Bad file footer.");
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error Loading Theme at " + follower, MessageBoxButtons.OK);
                return originalTheme;
            }
            return theme;
        }

        public static ThemeStruct DefaultGLSLDarkTheme(Font prototypeFont)
        {
            return new ThemeStruct
            {
                Name = "Default GLSL Dark",
                BackgroundColor = Color.FromArgb(255, 64, 64, 64),
                CurrentLineColor = Color.FromArgb(255, 56, 56, 56),
                TextStyle = new FontAndColorStruct(prototypeFont, Color.White),
                ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.RoyalBlue),


                Delimiters = new[]{
                new DelimiterStruct { //whitespaces and breaks
                    Name = "Breaks",
                    Keychars = new[] {' ', ',', ';', '(', ')', '{', '}', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Gray)
                },               
                new DelimiterStruct { //operators
                    Name = "Operators",
                    Keychars = new[] { '/', '+', '-', '*', '=', '<', '>', '!' },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.DarkTurquoise)
                }},

                Words = new[]{
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[] {"#version", "uniform", "layout", "in", "out", "location", "void", "for", "else", "if", "main",
                    "smooth", "varying", "const", "flat​", "noperspective​"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Orange)
                },
                new WordStruct
                {
                    Name = "Types",
                    Keywords = new[] { "bool", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4" },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Yellow)
                },
                new WordStruct
                {
                    Name = "Functions",
                    Keywords = new[] { "gl_Position", "min", "max", "dot", "normalize", "clamp", "mix" },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Lime)
                }},
                Spans = new[] {   
                    new SpanStruct
                    {
                        Name = "TODO Comment", 
                        StartKeyword = "//TODO", 
                        StopKeyword = "\n", 
                        EscapeChar = '\n', 
                        Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Underline), Color.DeepSkyBlue)
                    },
                new SpanStruct
                {
                    Name = "Comment", 
                    StartKeyword = "//", 
                    StopKeyword = "\n", 
                    EscapeChar = '\n', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Italic), Color.Green)
                },
                new SpanStruct
                {
                    Name = "Inline String", 
                    StartKeyword = "\"", 
                    StopKeyword = "\"", 
                    EscapeChar = '\\', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Violet)
                }
            }


            };
        }
        public static ThemeStruct DefaultGLSLLightTheme(Font prototypeFont)
        {
            return new ThemeStruct
            {
                Name = "Default GLSL Light",
                BackgroundColor = Color.FromArgb(255, 255, 255, 255),
                CurrentLineColor = Color.FromArgb(255, 192, 216, 255),
                TextStyle = new FontAndColorStruct(prototypeFont, Color.Black),
                ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.RoyalBlue),


                Delimiters = new[]{
                new DelimiterStruct { //whitespaces and breaks
                    Name = "Breaks",
                    Keychars = new[] {' ', ',', ';', '(', ')', '{', '}', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.LightSlateGray)
                },               
                new DelimiterStruct { //operators
                    Name = "Operators",
                    Keychars = new[] { '/', '+', '-', '*', '=', '<', '>', '!' },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.DarkTurquoise)
                }},

                Words = new[]{
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[] {"#version", "uniform", "layout", "in", "out", "location", "void", "for", "else", "if", "main",
                    "smooth", "varying", "const", "flat​", "noperspective​"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Orange)
                },
                new WordStruct
                {
                    Name = "Types",
                    Keywords = new[] { "bool", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4" },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Red)
                },
                new WordStruct
                {
                    Name = "Functions",
                    Keywords = new[] { "gl_Position", "min", "max", "dot", "normalize", "clamp", "mix" },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Lime)
                }},
                Spans = new[] {   
                    new SpanStruct
                    {
                        Name = "TODO Comment", 
                        StartKeyword = "//TODO", 
                        StopKeyword = "\n", 
                        EscapeChar = '\n', 
                        Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Underline), Color.DeepSkyBlue)
                    },
                new SpanStruct
                {
                    Name = "Comment", 
                    StartKeyword = "//", 
                    StopKeyword = "\n", 
                    EscapeChar = '\n', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Italic), Color.Green)
                },
                new SpanStruct
                {
                    Name = "Inline String", 
                    StartKeyword = "\"", 
                    StopKeyword = "\"", 
                    EscapeChar = '\\', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Violet)
                }
            }


            };
        }
        public static ThemeStruct DefaultCSDarkTheme(Font prototypeFont)
        {
            return new ThemeStruct
            {
                Name = "Default C# Dark",
                BackgroundColor = Color.FromArgb(255, 30, 30, 30),
                CurrentLineColor = Color.FromArgb(255, 15, 15, 15),
                TextStyle = new FontAndColorStruct(prototypeFont, Color.White),
                ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.FromArgb(255, 156, 206, 168)),


                Delimiters = new[]{
                new DelimiterStruct { //whitespaces, breaks and operators
                    Name = "Breaks",
                    Keychars = new[] {' ', ',', ';', '(', ')', '{', '}', '/', '+', '-', '*', '=', '<', '>', '!', '[',']', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.White)
                }},

                Words = new[]{
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[] {"int", "new", "public", "private", "static", "class", "bool", "float", "single", "double", "try",
                    "catch", "throw", "const", "return​", "if", "else", "for", "foreach", "var", "while", "char", "string", "void"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.FromArgb(255, 67, 156, 214 ))
                },
                new WordStruct
                {
                    Name = "Classes",
                    Keywords = new[] { "Font", "Form", "Exception", "MessageBox", "Convert", "File", "BinaryReader", "BinaryWriter", "EventArgs" },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.FromArgb(255, 78, 201, 176))
                }},
                Spans = new[] {   
                    new SpanStruct
                    {
                        Name = "TODO Comment", 
                        StartKeyword = "//TODO",
                        StopKeyword = "\n", 
                        EscapeChar = '\n', 
                        Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.FromArgb(255,135,206,250) )
                    },
                new SpanStruct
                {
                    Name = "Comment", 
                    StartKeyword = "//", 
                    StopKeyword = "\n", 
                    EscapeChar = '\n', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Italic), Color.FromArgb(255, 96, 139, 78))
                },
                new SpanStruct
                {
                    Name = "Inline String", 
                    StartKeyword = "\"", 
                    StopKeyword = "\"", 
                    EscapeChar = '\\', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),  Color.FromArgb(255, 214, 157, 133))
                },
                new SpanStruct
                {
                    Name = "Region", 
                    StartKeyword = "#", 
                    StopKeyword = "\n", 
                    EscapeChar = '\n', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),  Color.FromArgb(255, 155, 155, 155))
                }
            }


            };
        }
        public static ThemeStruct DefaultCSBlueTheme(Font prototypeFont)
        {
            return new ThemeStruct
            {
                Name = "Default C# Blue",
                BackgroundColor = Color.FromArgb(255, 255, 255, 255),
                CurrentLineColor = Color.FromArgb(255, 255, 255, 255),
                TextStyle = new FontAndColorStruct(prototypeFont, Color.Black),
                ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Black),


                Delimiters = new[]{
                new DelimiterStruct { //whitespaces, breaks and operators
                    Name = "Breaks",
                    Keychars = new[] {' ', ',', ';', '(', ')', '{', '}', '/', '+', '-', '*', '=', '<', '>', '!', '[',']', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Black)
                }},

                Words = new[]{
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[] {"int", "new", "public", "private", "static", "class", "bool", "float", "single", "double", "try",
                    "catch", "throw", "const", "return​", "if", "else", "for", "foreach", "var", "while", "char", "string", "void"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.FromArgb(255, 0, 0, 255 ))
                },
                new WordStruct
                {
                    Name = "Classes",
                    Keywords = new[] { "Font", "Form", "Exception", "MessageBox", "Convert", "File", "BinaryReader", "BinaryWriter", "EventArgs" },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.FromArgb(255, 43, 145, 175))
                }},
                Spans = new[] {   
                    new SpanStruct
                    {
                        Name = "TODO Comment", 
                        StartKeyword = "//TODO",
                        StopKeyword = "\n",
                        EscapeChar = '\n', 
                        Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.FromArgb(255,0,0,139) )
                    },
                new SpanStruct
                {
                    Name = "Comment", 
                    StartKeyword = "//", 
                    StopKeyword = "\n", 
                    EscapeChar = '\n', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.FromArgb(255, 0, 128, 0))
                },
                new SpanStruct
                {
                    Name = "Inline String", 
                    StartKeyword = "\"", 
                    StopKeyword = "\"", 
                    EscapeChar = '\\', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),  Color.FromArgb(255, 163, 21, 21))
                },
                new SpanStruct
                {
                    Name = "Region", 
                    StartKeyword = "#", 
                    StopKeyword = "\n", 
                    EscapeChar = '\n', 
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),  Color.FromArgb(255, 0, 0, 255))
                }
            }


            };
        }
    }
}
