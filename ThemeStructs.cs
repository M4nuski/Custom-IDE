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
    public struct SFontColor
    {
        public Font StyleFont;
        public Color StyleColor;

        public SFontColor(Font styleFont, Color styleColor)
        {
            StyleFont = styleFont;
            StyleColor = styleColor;
        }
        public static bool operator ==(SFontColor a, SFontColor b)
        {
            return (a.StyleColor == b.StyleColor) & (a.StyleFont.Equals(b.StyleFont));
        }
        public static bool operator !=(SFontColor a, SFontColor b)
        {
            return (a.StyleColor != b.StyleColor) | !(a.StyleFont.Equals(b.StyleFont));
        }
    }

    public struct SDelimiter
    {
        public string Name;
        public char[] Tokens;
        public SFontColor Style;
    }

    public struct SWord
    {
        public string Name;
        public string[] Words;
        public SFontColor Style;
    }

    public struct SSpan
    {
        public string Name;
        public string StartDelimiter;
        public string StopDelimiter; //or EOL
        public char EscapeChar;
        public SFontColor Style;
    }

    public struct SToken
    {
        public int Offset;
        public string Text;
        public SFontColor Style;

        public static bool operator ==(SToken a, SToken b)
        {
            return (a.Text == b.Text) & (a.Style == b.Style);
        }
        public static bool operator !=(SToken a, SToken b)
        {
            return (a.Text != b.Text) | (a.Style != b.Style);
        }

    }

    public struct STheme // Main struct
    {
        public string Name;
        public SDelimiter[] Delimiters;
        public SWord[] Words;
        public SSpan[] Spans;
    }
    #endregion





    static class ThemeStructs
    {
        #region Helpers Methods
        static private void WriteStyle(SFontColor style, BinaryWriter writer)
        {
            writer.Write(style.StyleColor.A.ToString(CultureInfo.InvariantCulture));
            writer.Write(style.StyleColor.R.ToString(CultureInfo.InvariantCulture));
            writer.Write(style.StyleColor.G.ToString(CultureInfo.InvariantCulture));
            writer.Write(style.StyleColor.B.ToString(CultureInfo.InvariantCulture));
            writer.Write(style.StyleFont.FontFamily.Name);
            writer.Write(style.StyleFont.Size.ToString(CultureInfo.InvariantCulture));
            writer.Write(style.StyleFont.Bold.ToString());
            writer.Write(style.StyleFont.Italic.ToString());
            writer.Write(style.StyleFont.Strikeout.ToString());
            writer.Write(style.StyleFont.Underline.ToString());
        }

        static private SFontColor ReadStyle(BinaryReader reader)
        {
            var style = new SFontColor
            {
                StyleColor = Color.FromArgb(Convert.ToInt32(reader.ReadString()),
                    Convert.ToInt32(reader.ReadString()),
                    Convert.ToInt32(reader.ReadString()),
                    Convert.ToInt32(reader.ReadString()))
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

        static public void SaveTheme(STheme theme, string filename)
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
                writer.Write(theme.Delimiters[i].Tokens.Length.ToString(CultureInfo.InvariantCulture));
                for (var j = 0; j < theme.Delimiters[i].Tokens.Length; j++)
                {
                    writer.Write(Convert.ToInt32(theme.Delimiters[i].Tokens[j]).ToString(CultureInfo.InvariantCulture));
                }
                WriteStyle(theme.Delimiters[i].Style, writer);
            }

            writer.Write("Words");
            writer.Write(theme.Words.Length.ToString(CultureInfo.InvariantCulture));
            for (var i = 0; i < theme.Words.Length; i++)
            {
                writer.Write(theme.Words[i].Name);
                writer.Write(theme.Words[i].Words.Length.ToString(CultureInfo.InvariantCulture));
                for (var j = 0; j < theme.Words[i].Words.Length; j++)
                {
                    writer.Write(theme.Words[i].Words[j]);
                }
                WriteStyle(theme.Words[i].Style, writer);
            }

            writer.Write("Spans");
            writer.Write(theme.Spans.Length.ToString(CultureInfo.InvariantCulture));
            for (var i = 0; i < theme.Spans.Length; i++)
            {
                writer.Write(theme.Spans[i].Name);
                writer.Write(theme.Spans[i].StartDelimiter);
                writer.Write(theme.Spans[i].StopDelimiter);
                writer.Write(Convert.ToByte(theme.Spans[i].EscapeChar).ToString(CultureInfo.InvariantCulture));
                WriteStyle(theme.Spans[i].Style, writer);
            }

            fileStream.Dispose();
        }

        public static STheme LoadTheme(string filename)
        {
            var theme = new STheme();
            var fileStream = File.OpenRead(filename);
            var reader = new BinaryReader(fileStream);
            if (reader.ReadString() == "CustomIDETheme") //File header
            {
                theme.Name = reader.ReadString();

                if (reader.ReadString() == "Delimiters")
                {
                    theme.Delimiters = new SDelimiter[Convert.ToInt32(reader.ReadString())];
                    for (var i = 0; i < theme.Delimiters.Length; i++)
                    {
                        theme.Delimiters[i].Name = reader.ReadString();
                        theme.Delimiters[i].Tokens = new char[Convert.ToInt32(reader.ReadString())];
                        for (var j = 0; j < theme.Delimiters[i].Tokens.Length; j++)
                        {
                            theme.Delimiters[i].Tokens[j] = Convert.ToChar(Convert.ToInt32(reader.ReadString()));
                        }
                        theme.Delimiters[i].Style = ReadStyle(reader);
                    }
                }
                if (reader.ReadString() == "Words")
                {
                    theme.Words = new SWord[Convert.ToInt32(reader.ReadString())];
                    for (var i = 0; i < theme.Words.Length; i++)
                    {
                        theme.Words[i].Name = reader.ReadString();
                        theme.Words[i].Words = new string[Convert.ToInt32(reader.ReadString())];
                        for (var j = 0; j < theme.Words[i].Words.Length; j++)
                        {
                            theme.Words[i].Words[j] = reader.ReadString();
                        }
                        theme.Words[i].Style = ReadStyle(reader);
                    }
                }
                if (reader.ReadString() == "Spans")
                {
                    theme.Spans = new SSpan[Convert.ToInt32(reader.ReadString())];
                    for (var i = 0; i < theme.Spans.Length; i++)
                    {
                        theme.Spans[i].Name = reader.ReadString();
                        theme.Spans[i].StartDelimiter = reader.ReadString();
                        theme.Spans[i].StopDelimiter = reader.ReadString();
                        theme.Spans[i].EscapeChar = Convert.ToChar(Convert.ToInt32(reader.ReadString()));
                        theme.Spans[i].Style = ReadStyle(reader);
                    }
                }

                
            }
            
            fileStream.Dispose();
            return theme;
        }
    }
}
