using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShaderIDE
{
    public partial class UserControl1 : TextBox
    {
        public struct SToken
        {
            public string Text;
            public FontStyle Style;
            public Color FontColor;
        }

        public Color DelColor = Color.Black;
        public Color ResColor = Color.Orange;
        public Color FunColor = Color.Lime;
        public Color TxtColor = Color.White;
        public Color NumColor = Color.DarkBlue;

        public List<SToken> res = new List<SToken>();

        char[] delimiterChars = { '\t', '\r', '\n', ' ', ',', ';', '/', '+', '-', '*', '(', ')', '{', '}', '=', '<', '>' };
        private char[] maskedDelimiters = { '\r', '\n' };

        private string[] reservedTokens =
        {
            "#version", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4", "uniform",
            "layout", "in", "out", "location", "void",  "void", "for", "else", "if"
        };

        private string[] functionTokens = { "gl_Position", "min", "max", "dot", "normalize", "main", "clamp" };
        public bool IsNum(string s)
        {
            double doubleBuffer;
            var result = double.TryParse(s.Trim().Replace('.', ','), out doubleBuffer);
            return result;
        }

        public Color GetTokenColor(string s)
        {
            if (functionTokens.Contains(s)) return FunColor;
            if (reservedTokens.Contains(s)) return ResColor;
            if (IsNum(s)) return NumColor;
            return TxtColor;
        }

        public FontStyle GetTokenStyle(string s)
        {
            //  if (functionTokens.Contains(s)) return FontStyle.Regular;
            if (reservedTokens.Contains(s)) return FontStyle.Bold;
            return FontStyle.Regular;
        }

        public List<SToken> Tokenize(string line)
        {
            var result = new List<SToken>();
            var linePointer = 0;
            for (var i = 0; i < line.Length; i++)
            {
                // 0123456789012345678901234567890
                // vec4 buf_specular, buf_normal;
                if (delimiterChars.Contains(line[i]))
                {
                    if (linePointer != i)
                    {
                        var currentText = line.Substring(linePointer, i - linePointer);
                        result.Add(new SToken
                        {
                            Text = currentText,
                            FontColor = GetTokenColor(currentText),
                            Style = GetTokenStyle(currentText)
                        });
                    }

                    result.Add(new SToken
                    {
                        Text = line[i].ToString(),
                        FontColor = DelColor,
                        Style = FontStyle.Bold
                    });
                    linePointer = i + 1;
                }

            }
            if (linePointer < line.Length) result.Add(new SToken
            {
                Text = line.Substring(linePointer, line.Length - linePointer),
                FontColor = GetTokenColor(line.Substring(linePointer, line.Length - linePointer)),
                Style = GetTokenStyle(line.Substring(linePointer, line.Length - linePointer))
            });
            return result;
        }
        public UserControl1()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
          //  base.OnPaint(e);
            res = Tokenize(textBox1.Text);
            if (res.Count > 0)
            {
            //    var dist = TextRenderer.MeasureText(e.Graphics, textBox1.Text, label1.Font, label1.ClientSize, TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
            //    vScrollBar1.Maximum = 32 + ((dist.Height > label1.ClientSize.Height)
            //        ? dist.Height - label1.ClientSize.Height: 0);
            //    hScrollBar1.Maximum = 32 + ((dist.Width > label1.ClientSize.Width)
            //        ? dist.Width - label1.ClientSize.Width: 0);
            //    var textPosition = new Point(-hScrollBar1.Value+8, -vScrollBar1.Value);
                var textPosition = new Point(0, 0);
                for (int i = 0; i < res.Count; i++)
                {
                    var curFont = new Font(Font, res[i].Style);
                   var dist = TextRenderer.MeasureText(e.Graphics, res[i].Text, curFont, ClientSize, TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
                    if (res[i].Text == "\n")
                    {
                        textPosition.X = 0;//-hScrollBar1.Value+8;
                        textPosition.Y += dist.Height / 2;
                    }
                    else if ((res[i].Text != "\r") & (res[i].Text != ""))
                    {
                        TextRenderer.DrawText(e.Graphics, res[i].Text, curFont, new Rectangle(textPosition, dist), res[i].FontColor, TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
                        textPosition.X += dist.Width;
                    }
                }
            }

            //
        }
    }
}
