using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        public static bool operator ==(FontAndColorStruct a, FontAndColorStruct b)
        {
            return (a.StyleColor == b.StyleColor) & (a.StyleFont.Equals(b.StyleFont));
        }
        public static bool operator !=(FontAndColorStruct a, FontAndColorStruct b)
        {
            return (a.StyleColor != b.StyleColor) | !(a.StyleFont.Equals(b.StyleFont));
        }
    }

    public struct ThemeStruct // Main struct
    {
        public string Name;
        public DelimiterStruct[] Delimiters;
        public WordStruct[] Words;
        public SpanStruct[] Spans;
        public FontAndColorStruct TextStyle, ValueStyle;
        public Color BackgroundColor, CurrentLineColor, ErrorLineColor;
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

    public struct TokenStruct
    {
        public int Offset;
        public string Text;
        public FontAndColorStruct Style;

        public static bool operator ==(TokenStruct a, TokenStruct b)
        {
            return (a.Text == b.Text) & (a.Style == b.Style);
        }
        public static bool operator !=(TokenStruct a, TokenStruct b)
        {
            return (a.Text != b.Text) | (a.Style != b.Style);
        }
    }
    #endregion

    static class ThemeStructs
    {
        #region Helpers Methods

        static private void WriteColor(Color color, BinaryWriter writer)
        {
            writer.Write(color.A.ToString(CultureInfo.InvariantCulture));
            writer.Write(color.R.ToString(CultureInfo.InvariantCulture));
            writer.Write(color.G.ToString(CultureInfo.InvariantCulture));
            writer.Write(color.B.ToString(CultureInfo.InvariantCulture));
        }

        static private Color ReadColor(BinaryReader reader)
        {
            return Color.FromArgb(  Convert.ToInt32(reader.ReadString()),
                                    Convert.ToInt32(reader.ReadString()),
                                    Convert.ToInt32(reader.ReadString()),
                                    Convert.ToInt32(reader.ReadString())    );
        }

        static private void WriteStyle(FontAndColorStruct style, BinaryWriter writer)
        {
            WriteColor(style.StyleColor, writer);
            writer.Write(style.StyleFont.FontFamily.Name);
            writer.Write(style.StyleFont.Size.ToString(CultureInfo.InvariantCulture));
            writer.Write(style.StyleFont.Bold.ToString());
            writer.Write(style.StyleFont.Italic.ToString());
            writer.Write(style.StyleFont.Strikeout.ToString());
            writer.Write(style.StyleFont.Underline.ToString());
        }

        static private FontAndColorStruct ReadStyle(BinaryReader reader)
        {
            var style = new FontAndColorStruct
            {
                StyleColor = ReadColor(reader)
            };

            var fontName = reader.ReadString();
            var fontEmSize = Convert.ToSingle(reader.ReadString());

            var styleBuffer = FontStyle.Regular;
            if (Convert.ToBoolean(reader.ReadString())) styleBuffer = styleBuffer | FontStyle.Bold;
            if (Convert.ToBoolean(reader.ReadString())) styleBuffer = styleBuffer | FontStyle.Italic;
            if (Convert.ToBoolean(reader.ReadString())) styleBuffer = styleBuffer | FontStyle.Strikeout;
            if (Convert.ToBoolean(reader.ReadString())) styleBuffer = styleBuffer | FontStyle.Underline;

            style.StyleFont = new Font(fontName, fontEmSize, styleBuffer);
            return style;
        }
        #endregion

        static public void SaveTheme(ThemeStruct theme, string filename)
        {
            var fileStream = File.Create(filename);
            var writer = new BinaryWriter(fileStream);
            
            writer.Write("CustomIDETheme"); //File header

            writer.Write(theme.Name);
           
            writer.Write("Delimiters");
            writer.Write(theme.Delimiters.Length.ToString(CultureInfo.InvariantCulture));
            for (var i = 0; i < theme.Delimiters.Length; i++)
            {
                writer.Write(theme.Delimiters[i].Name);
                writer.Write(theme.Delimiters[i].Keychars.Length.ToString(CultureInfo.InvariantCulture));
                for (var j = 0; j < theme.Delimiters[i].Keychars.Length; j++)
                {
                    writer.Write(Convert.ToInt32(theme.Delimiters[i].Keychars[j]).ToString(CultureInfo.InvariantCulture));
                }
                WriteStyle(theme.Delimiters[i].Style, writer);
            }

            writer.Write("Words");
            writer.Write(theme.Words.Length.ToString(CultureInfo.InvariantCulture));
            for (var i = 0; i < theme.Words.Length; i++)
            {
                writer.Write(theme.Words[i].Name);
                writer.Write(theme.Words[i].Keywords.Length.ToString(CultureInfo.InvariantCulture));
                for (var j = 0; j < theme.Words[i].Keywords.Length; j++)
                {
                    writer.Write(theme.Words[i].Keywords[j]);
                }
                WriteStyle(theme.Words[i].Style, writer);
            }

            writer.Write("Spans");
            writer.Write(theme.Spans.Length.ToString(CultureInfo.InvariantCulture));
            for (var i = 0; i < theme.Spans.Length; i++)
            {
                writer.Write(theme.Spans[i].Name);
                writer.Write(theme.Spans[i].StartKeyword);
                writer.Write(theme.Spans[i].StopKeyword);
                writer.Write(Convert.ToByte(theme.Spans[i].EscapeChar).ToString(CultureInfo.InvariantCulture));
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
            writer.Write("ErrorLineColor");
            WriteColor(theme.ErrorLineColor, writer);
            writer.Write("CustomIDEEndOfTheme");
            fileStream.Flush();
            fileStream.Dispose();
        }

        public static ThemeStruct LoadTheme(string filename)
        {
            var theme = new ThemeStruct();
            var fileStream = File.OpenRead(filename);
            var reader = new BinaryReader(fileStream);
            if (reader.ReadString() == "CustomIDETheme") //File header
            {
                theme.Name = reader.ReadString();

                if (reader.ReadString() == "Delimiters")
                {
                    theme.Delimiters = new DelimiterStruct[Convert.ToInt32(reader.ReadString())];
                    for (var i = 0; i < theme.Delimiters.Length; i++)
                    {
                        theme.Delimiters[i].Name = reader.ReadString();
                        theme.Delimiters[i].Keychars = new char[Convert.ToInt32(reader.ReadString())];
                        for (var j = 0; j < theme.Delimiters[i].Keychars.Length; j++)
                        {
                            theme.Delimiters[i].Keychars[j] = Convert.ToChar(Convert.ToInt32(reader.ReadString()));
                        }
                        theme.Delimiters[i].Style = ReadStyle(reader);
                    }
                }
                if (reader.ReadString() == "Words")
                {
                    theme.Words = new WordStruct[Convert.ToInt32(reader.ReadString())];
                    for (var i = 0; i < theme.Words.Length; i++)
                    {
                        theme.Words[i].Name = reader.ReadString();
                        theme.Words[i].Keywords = new string[Convert.ToInt32(reader.ReadString())];
                        for (var j = 0; j < theme.Words[i].Keywords.Length; j++)
                        {
                            theme.Words[i].Keywords[j] = reader.ReadString();
                        }
                        theme.Words[i].Style = ReadStyle(reader);
                    }
                }
                if (reader.ReadString() == "Spans")
                {
                    theme.Spans = new SpanStruct[Convert.ToInt32(reader.ReadString())];
                    for (var i = 0; i < theme.Spans.Length; i++)
                    {
                        theme.Spans[i].Name = reader.ReadString();
                        theme.Spans[i].StartKeyword = reader.ReadString();
                        theme.Spans[i].StopKeyword = reader.ReadString();
                        theme.Spans[i].EscapeChar = Convert.ToChar(Convert.ToInt32(reader.ReadString()));
                        theme.Spans[i].Style = ReadStyle(reader);
                    }
                }

                
            }

            if (reader.ReadString() == "TextStyle") theme.TextStyle = ReadStyle(reader);
            if (reader.ReadString() == "ValueStyle") theme.ValueStyle = ReadStyle(reader);
            if (reader.ReadString() == "BackgroundColor") theme.BackgroundColor = ReadColor(reader);
            if (reader.ReadString() == "CurrentLineColor") theme.CurrentLineColor = ReadColor(reader);
            if (reader.ReadString() == "ErrorLineColor") theme.ErrorLineColor = ReadColor(reader);
            //if (reader.ReadString() == "CustomIDEEndOfTheme") 
            fileStream.Dispose();
            return theme;
        }
    }
}
