using System;
using System.Drawing;
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

    public struct DelimiterStruct
    {
        public string Name;
        public char[] Keychars;
        public FontAndColorStruct Style;

        public DelimiterStruct(Font prototypeFont)
        {
            Name = "<new>";
            Keychars = new char[] { };
            Style = new FontAndColorStruct { StyleFont = prototypeFont };
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
            Style = new FontAndColorStruct { StyleFont = prototypeFont };
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
            Style = new FontAndColorStruct { StyleFont = prototypeFont };
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

    public class EditorBoxTheme
    {
        public string Name;
        public DelimiterStruct[] Delimiters;
        public WordStruct[] Words;
        public SpanStruct[] Spans;
        public FontAndColorStruct TextStyle, ValueStyle;
        public Color BackgroundColor, CurrentLineColor;

        public EditorBoxTheme()
        {
            Name = "<Empty>";
            //BackgroundColor = backColor;
            //CurrentLineColor = backColor;
            //TextStyle = new FontAndColorStruct(prototypeFont, textColor);
            //ValueStyle = new FontAndColorStruct(prototypeFont, textColor);
            Delimiters = new DelimiterStruct[0];
            Words = new WordStruct[0];
            Spans = new SpanStruct[0];
        }

        public EditorBoxTheme(Font prototypeFont, Color textColor, Color backColor)
        {
            Name = "<Default>";
            BackgroundColor = backColor;
            CurrentLineColor = backColor;
            TextStyle = new FontAndColorStruct(prototypeFont, textColor);
            ValueStyle = new FontAndColorStruct(prototypeFont, textColor);
            Delimiters = new DelimiterStruct[0];
            Words = new WordStruct[0];
            Spans = new SpanStruct[0];
        }

        private void clone(EditorBoxTheme theme)
        {
            Name = theme.Name;
            BackgroundColor = theme.BackgroundColor;
            CurrentLineColor = theme.CurrentLineColor;
            TextStyle = theme.TextStyle;
            ValueStyle = theme.ValueStyle;
            Delimiters = theme.Delimiters;
            Words = theme.Words;
            Spans = theme.Spans;
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
            return Color.FromArgb(reader.ReadByte(),
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
            if (reader.ReadBoolean()) styleBuffer |= FontStyle.Bold;
            if (reader.ReadBoolean()) styleBuffer |= FontStyle.Italic;
            if (reader.ReadBoolean()) styleBuffer |= FontStyle.Strikeout;
            if (reader.ReadBoolean()) styleBuffer |= FontStyle.Underline;

            style.StyleFont = new Font(fontName, fontEmSize, styleBuffer);
            return style;
        }

        public void ChangeFont(Font newfont)
        {
            TextStyle.StyleFont = newfont;
            ValueStyle.StyleFont = new Font(newfont, ValueStyle.StyleFont.Style);

            //Update all tokens to use this new font.
            for (var i = 0; i < Delimiters.Length; i++)
            {
                Delimiters[i].Style.StyleFont = new Font(newfont, Delimiters[i].Style.StyleFont.Style);
            }

            for (var i = 0; i < Words.Length; i++)
            {
                Words[i].Style.StyleFont = new Font(newfont, Words[i].Style.StyleFont.Style);
            }

            for (var i = 0; i < Spans.Length; i++)
            {
                Spans[i].Style.StyleFont = new Font(newfont, Spans[i].Style.StyleFont.Style);
            }
        }

    

         public void SaveToFile(string filename)
        {
            var fileStream = File.Create(filename);
            var writer = new BinaryWriter(fileStream);

            writer.Write("CustomIDETheme"); //File header
            writer.Write(Name);

            writer.Write("Delimiters");
            writer.Write(Delimiters.Length);
            for (var i = 0; i < Delimiters.Length; i++)
            {
                writer.Write(Delimiters[i].Name);
                writer.Write(Delimiters[i].Keychars.Length);
                foreach (var t in Delimiters[i].Keychars)
                {
                    writer.Write(Convert.ToInt32(t));
                }
                WriteStyle(Delimiters[i].Style, writer);
            }

            writer.Write("Words");
            writer.Write(Words.Length);
            for (var i = 0; i < Words.Length; i++)
            {
                writer.Write(Words[i].Name);
                writer.Write(Words[i].Keywords.Length);
                foreach (var t in Words[i].Keywords)
                {
                    writer.Write(t);
                }
                WriteStyle(Words[i].Style, writer);
            }

            writer.Write("Spans");
            writer.Write(Spans.Length);
            for (var i = 0; i < Spans.Length; i++)
            {
                writer.Write(Spans[i].Name);
                writer.Write(Spans[i].StartKeyword);
                writer.Write(Spans[i].StopKeyword);
                writer.Write(Convert.ToInt32(Spans[i].EscapeChar));
                WriteStyle(Spans[i].Style, writer);
            }

            writer.Write("TextStyle");
            WriteStyle(TextStyle, writer);
            writer.Write("ValueStyle");
            WriteStyle(ValueStyle, writer);
            writer.Write("BackgroundColor");
            WriteColor(BackgroundColor, writer);
            writer.Write("CurrentLineColor");
            WriteColor(CurrentLineColor, writer);

            writer.Write("CustomIDEEndOfTheme");
            fileStream.Flush();
            fileStream.Dispose();
        }

        public void LoadFromFile( string filename)
        {
            var backup = this;
            var follower = "Init";
            try
            {
                var fileStream = File.OpenRead(filename);
                var reader = new BinaryReader(fileStream);
                if (reader.ReadString() == "CustomIDETheme") //File header
                {
                    follower = "Name";
                    Name = reader.ReadString();

                    follower = "Delimiters";
                    if (reader.ReadString() == "Delimiters")
                    {
                        Delimiters = new DelimiterStruct[reader.ReadInt32()];
                        for (var i = 0; i < Delimiters.Length; i++)
                        {
                            Delimiters[i].Name = reader.ReadString();
                            Delimiters[i].Keychars = new char[reader.ReadInt32()];
                            for (var j = 0; j < Delimiters[i].Keychars.Length; j++)
                            {
                                Delimiters[i].Keychars[j] = Convert.ToChar(reader.ReadInt32());
                            }
                            Delimiters[i].Style = ReadStyle(reader);
                        }
                    }
                    else throw new Exception("File Structure Error.");

                    follower = "Words";
                    if (reader.ReadString() == "Words")
                    {
                        Words = new WordStruct[reader.ReadInt32()];
                        for (var i = 0; i < Words.Length; i++)
                        {
                            Words[i].Name = reader.ReadString();
                            Words[i].Keywords = new string[reader.ReadInt32()];
                            for (var j = 0; j < Words[i].Keywords.Length; j++)
                            {
                                Words[i].Keywords[j] = reader.ReadString();
                            }
                            Words[i].Style = ReadStyle(reader);
                        }
                    }
                    else throw new Exception("File Structure Error.");

                    follower = "Spans";
                    if (reader.ReadString() == "Spans")
                    {
                        Spans = new SpanStruct[reader.ReadInt32()];
                        for (var i = 0; i < Spans.Length; i++)
                        {
                            Spans[i].Name = reader.ReadString();
                            Spans[i].StartKeyword = reader.ReadString();
                            Spans[i].StopKeyword = reader.ReadString();
                            Spans[i].EscapeChar = Convert.ToChar(reader.ReadInt32());
                            Spans[i].Style = ReadStyle(reader);
                        }
                    }
                    else throw new Exception("File Structure Error.");

                    follower = "TextStyle";
                    if (reader.ReadString() == "TextStyle") TextStyle = ReadStyle(reader); else throw new Exception("File Structure Error.");
                    follower = "ValueStyle";
                    if (reader.ReadString() == "ValueStyle") ValueStyle = ReadStyle(reader); else throw new Exception("File Structure Error.");
                    follower = "BackgroundColor";
                    if (reader.ReadString() == "BackgroundColor") BackgroundColor = ReadColor(reader); else throw new Exception("File Structure Error.");
                    follower = "CurrentLineColor";
                    if (reader.ReadString() == "CurrentLineColor") CurrentLineColor = ReadColor(reader); else throw new Exception("File Structure Error.");
                }
                else throw new Exception("Bad file header.");

                follower = "CustomIDEEndOfTheme";
                if (reader.ReadString() != "CustomIDEEndOfTheme") throw new Exception("Bad file footer.");
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error Loading Theme at " + follower, MessageBoxButtons.OK);
                clone(backup);
            }
        }

        public void LoadDefaultGLSLDarkTheme(Font prototypeFont)
        {
            Name = "Default GLSL Dark";
            BackgroundColor = Color.FromArgb(255, 32, 32, 32);
            CurrentLineColor = Color.FromArgb(255, 24, 24, 24);
            TextStyle = new FontAndColorStruct(prototypeFont, Color.White);
            ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.RoyalBlue);
            
            Delimiters = new[]
            {
                new DelimiterStruct
                {
                    //whitespaces and breaks
                    Name = "Breaks",
                    Keychars = new[] {' ', ',', ';', '(', ')', '{', '}', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Gray)
                },
                new DelimiterStruct
                {
                    //operators
                    Name = "Operators",
                    Keychars = new[] {'/', '+', '-', '*', '=', '<', '>', '!'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.DarkTurquoise)
                }
            };

            Words = new[]
            {
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[]
                    {
                        "#version", "uniform", "layout", "in", "out", "location", "void", "for", "else", "if", "main",
                        "smooth", "varying", "const", "flat​", "noperspective​"
                    },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Orange)
                },
                new WordStruct
                {
                    Name = "Types",
                    Keywords = new[] {"bool", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Yellow)
                },
                new WordStruct
                {
                    Name = "Functions",
                    Keywords = new[] {"gl_Position", "min", "max", "dot", "normalize", "clamp", "mix"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Lime)
                }
            };

            Spans = new[]
            {
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
            };
        }



        public  void LoadDefaultGLSLLightTheme(Font prototypeFont)
        {

            Name = "Default GLSL Light";
            BackgroundColor = Color.FromArgb(255, 255, 255, 255);
            CurrentLineColor = Color.FromArgb(255, 192, 216, 255);
            TextStyle = new FontAndColorStruct(prototypeFont, Color.Black);
            ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.RoyalBlue);

            Delimiters = new[]
            {
                new DelimiterStruct
                {
                    //whitespaces and breaks
                    Name = "Breaks",
                    Keychars = new[] {' ', ',', ';', '(', ')', '{', '}', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.LightSlateGray)
                },
                new DelimiterStruct
                {
                    //operators
                    Name = "Operators",
                    Keychars = new[] {'/', '+', '-', '*', '=', '<', '>', '!'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.DarkTurquoise)
                }
            };

            Words = new[]
            {
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[]
                    {
                        "#version", "uniform", "layout", "in", "out", "location", "void", "for", "else", "if", "main",
                        "smooth", "varying", "const", "flat​", "noperspective​"
                    },
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Orange)
                },
                new WordStruct
                {
                    Name = "Types",
                    Keywords = new[] {"bool", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Red)
                },
                new WordStruct
                {
                    Name = "Functions",
                    Keywords = new[] {"gl_Position", "min", "max", "dot", "normalize", "clamp", "mix"},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Lime)
                }
            };

            Spans = new[]
            {
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
            };
        }

        public void LoadDefaultCSDarkTheme(Font prototypeFont)
        {
            Name = "Default C# Dark";
            BackgroundColor = Color.FromArgb(255, 30, 30, 30);
            CurrentLineColor = Color.FromArgb(255, 15, 15, 15);
            TextStyle = new FontAndColorStruct(prototypeFont, Color.White);
            ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular),Color.FromArgb(255, 156, 206, 168));

            Delimiters = new[]
            {
                new DelimiterStruct
                {
                    //whitespaces, breaks and operators
                    Name = "Breaks",
                    Keychars =
                        new[]
                        {' ', ',', ';', '(', ')', '{', '}', '/', '+', '-', '*', '=', '<', '>', '!', '[', ']', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.White)
                }
            };

            Words = new[]
            {
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[]
                    {
                        "int", "new", "public", "private", "static", "class", "bool", "float", "single", "double", "try",
                        "catch", "throw", "const", "return​", "if", "else", "for", "foreach", "var", "while", "char",
                        "string", "void"
                    },
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular),
                            Color.FromArgb(255, 67, 156, 214))
                },
                new WordStruct
                {
                    Name = "Classes",
                    Keywords =
                        new[]
                        {
                            "Font", "Form", "Exception", "MessageBox", "Convert", "File", "BinaryReader", "BinaryWriter",
                            "EventArgs"
                        },
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 78, 201, 176))
                }
            };

            Spans = new[]
            {
                new SpanStruct
                {
                    Name = "TODO Comment",
                    StartKeyword = "//TODO",
                    StopKeyword = "\n",
                    EscapeChar = '\n',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 135, 206, 250))
                },
                new SpanStruct
                {
                    Name = "Comment",
                    StartKeyword = "//",
                    StopKeyword = "\n",
                    EscapeChar = '\n',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Italic),
                            Color.FromArgb(255, 96, 139, 78))
                },
                new SpanStruct
                {
                    Name = "Inline String",
                    StartKeyword = "\"",
                    StopKeyword = "\"",
                    EscapeChar = '\\',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 214, 157, 133))
                },
                new SpanStruct
                {
                    Name = "Region",
                    StartKeyword = "#",
                    StopKeyword = "\n",
                    EscapeChar = '\n',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 155, 155, 155))
                }
            };
        }

        public void LoadDefaultCSBlueTheme(Font prototypeFont)
        {
            Name = "Default C# Blue";
            BackgroundColor = Color.FromArgb(255, 255, 255, 255);
            CurrentLineColor = Color.FromArgb(255, 255, 255, 255);
            TextStyle = new FontAndColorStruct(prototypeFont, Color.Black);
            ValueStyle = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular), Color.Black);

            Delimiters = new[]
            {
                new DelimiterStruct
                {
                    //whitespaces, breaks and operators
                    Name = "Breaks",
                    Keychars =
                        new[]
                        {' ', ',', ';', '(', ')', '{', '}', '/', '+', '-', '*', '=', '<', '>', '!', '[', ']', '\t'},
                    Style = new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold), Color.Black)
                }
            };

            Words = new[]
            {
                new WordStruct
                {
                    Name = "Reserved",
                    Keywords = new[]
                    {
                        "int", "new", "public", "private", "static", "class", "bool", "float", "single", "double",
                        "try",
                        "catch", "throw", "const", "return​", "if", "else", "for", "foreach", "var", "while", "char",
                        "string", "void"
                    },
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular),
                            Color.FromArgb(255, 0, 0, 255))
                },
                new WordStruct
                {
                    Name = "Classes",
                    Keywords =
                        new[]
                        {
                            "Font", "Form", "Exception", "MessageBox", "Convert", "File", "BinaryReader",
                            "BinaryWriter", "EventArgs"
                        },
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 43, 145, 175))
                }
            };
            Spans = new[]
            {
                new SpanStruct
                {
                    Name = "TODO Comment",
                    StartKeyword = "//TODO",
                    StopKeyword = "\n",
                    EscapeChar = '\n',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 0, 0, 139))
                },
                new SpanStruct
                {
                    Name = "Comment",
                    StartKeyword = "//",
                    StopKeyword = "\n",
                    EscapeChar = '\n',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Regular),
                            Color.FromArgb(255, 0, 128, 0))
                },
                new SpanStruct
                {
                    Name = "Inline String",
                    StartKeyword = "\"",
                    StopKeyword = "\"",
                    EscapeChar = '\\',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 163, 21, 21))
                },
                new SpanStruct
                {
                    Name = "Region",
                    StartKeyword = "#",
                    StopKeyword = "\n",
                    EscapeChar = '\n',
                    Style =
                        new FontAndColorStruct(new Font(prototypeFont, FontStyle.Bold),
                            Color.FromArgb(255, 0, 0, 255))
                }
            };
        }




    }





}
