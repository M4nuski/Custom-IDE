using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.SqlServer.Server;

namespace ShaderIDE
{
    public partial class Form1 : Form
    {
        #region Vestigial Fields
        /* public struct SToken
        {
            public string Text;
            public FontStyle Style;
            public Color FontColor;
            public int OriginalTextOffset;
        }*/
        /*
        public Color DelColor = Color.Gray;
        public Color ResColor = Color.Orange;
        public Color TypColor = Color.Yellow;
        public Color FunColor = Color.Lime;
        public Color TxtColor = Color.White;
        public Color NumColor = Color.RoyalBlue;
        public Color ComColor = Color.DrarkGreen;
        */
        /*
        private readonly char[] _delimiterTokens =
        {
            '\t', '\r', '\n', ' ', ',', ';', '/', '+', '-', '*', '(', ')', '{', '}', '=', '<', '>'
        };*/
        /*
        private readonly string[] _reservedTokens =
        {
            "#version", "uniform", "layout", "in", "out", "location", "void", "for", "else", "if", "main",
            "smooth", "varying", "const", "flat​", "noperspective​"
        };*/
        /*
        private readonly string[] _typesTokens =
        {
            "bool", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4"
        };*/
        /*
        private readonly string[] _functionTokens =
        {
            "gl_Position", "min", "max", "dot", "normalize", "clamp", "mix"
        };*/
        //    public int LastSelection;

        #endregion

        #region Vestigial Methods
        /*
        public int FindFirst(char[] list, char target)
        {
            for (var i = 0; i < list.Length; i++)
            {
                if (list[i] == target)
                {
                    return i;
                }
            }
            return -1;
        }*/
        /*
        public Color GetTokenColor(string s)
        {
            if (functionTokens.Contains(s)) return FunColor;
            if (reservedTokens.Contains(s)) return ResColor;
            if (IsNum(s)) return NumColor;
            if (typesTokens.Contains(s)) return TypColor;
            return TxtColor;
        }*/
        /*
        public FontStyle GetTokenStyle(string s)
        {
            if (_functionTokens.Contains(s)) return FontStyle.Italic;
            if (_reservedTokens.Contains(s)) return FontStyle.Bold;
            return FontStyle.Regular;
        }*/
        /*
        public SFontColor GetTokenFont(string s)
        {
            foreach (var d in s)
            {
                
            }
            if (_reservedTokens.Contains(s)) return ResFont;
            if (_typesTokens.Contains(s)) return TypFont;
            if (_functionTokens.Contains(s)) return FunFont;

            if (IsNum(s)) return NumbersFont;
            return DefaultTextFont;
        }*/

        /*
         public List<SToken> Tokenize(string line)
        {
            var result = new List<SToken>();
            var linePointer = 0;
            for (var i = 0; i < line.Length; i++)
            {
                // 0123456789012345678901234567890
                // vec4 buf_specular, buf_normal;
                if (delimiterTokens.Contains(line[i]))
                {
                    if (linePointer != i)
                    {
                        var currentText = line.Substring(linePointer, i - linePointer);
                        result.Add(new SToken
                        {
                            Text = currentText,
                            FontColor = GetTokenColor(currentText),
                            Style = GetTokenStyle(currentText),
                            OriginalTextOffset = linePointer
                        });
                }

                result.Add(new SToken
                    {
                        Text = line[i].ToString(),
                        FontColor = DelColor,
                        Style = FontStyle.Bold,
                        OriginalTextOffset = i
                    });
                    linePointer = i+1;
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
         * */
        //private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        //{
        //    label1.Refresh();
        // }
        /*
       private Size Measure(IDeviceContext g, string s)
       {
           return TextRenderer.MeasureText(g, s, label1.Font, label1.ClientSize, TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);   
       }*/

        /*    private Point GetOffsetFromCharIndex(IDeviceContext g, int index)
            {
                var startLine = textBox1.GetLineFromCharIndex(index);

                var startLinePosition = textBox1.GetFirstCharIndexFromLine(startLine);

                var result = new Point(0, Measure(g, textBox1.Text.Substring(0, (startLinePosition ==0) ? 1 : startLinePosition)).Height-23);

                if (startLinePosition != index)
                {
                    result.X = Measure(g, textBox1.Lines[startLine].Substring(0, index - startLinePosition)).Width;
                }
                return result;
            }*/
        /* private void label1_Paint(object sender, PaintEventArgs e)
 {
    if (TokenList.Count > 0)
     {
         //get top-left char offset
         var topLeftPosition = GetOffsetFromCharIndex(e.Graphics, textBox1.GetCharIndexFromPosition(new Point(1, 1)));
         Text = topLeftPosition.ToString() +" "+ textBox1.GetCharIndexFromPosition(new Point(1, 1));
         var textPosition = new Point(-topLeftPosition.X, -topLeftPosition.Y);

         //left margin
         textPosition.X += 5;

         //save for subsequent lines
         var startOffset = textPosition.X;

         for (var i = 0; i < TokenList.Count; i++)
         {
             var curFont = new Font(label1.Font, TokenList[i].Style);
             var tokenSize = Measure(e.Graphics, TokenList[i].Text);
             if (TokenList[i].Text == "\n")
             {
                 textPosition.X = startOffset;
                 textPosition.Y += tokenSize.Height / 2; // 1 height for text + 1 height for line break
             }
             else if ((TokenList[i].Text != "\r") & (TokenList[i].Text != ""))
             {
                 TextRenderer.DrawText(e.Graphics, TokenList[i].Text, curFont, new Rectangle(textPosition, tokenSize), TokenList[i].FontColor, TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
                 textPosition.X += tokenSize.Width;
             }
         }

         if (textBox1.SelectionLength > 0)
         {
             var brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
             var SelStartLine = textBox1.GetLineFromCharIndex(textBox1.SelectionStart);
             var SelStopLine = textBox1.GetLineFromCharIndex(textBox1.SelectionStart + textBox1.SelectionLength);

             if (SelStartLine == SelStopLine)
             {   //single line selection
                 textPosition = GetOffsetFromCharIndex(e.Graphics, textBox1.SelectionStart);
                 textPosition.X -= topLeftPosition.X - 5;
                 textPosition.Y -= topLeftPosition.Y;
                 e.Graphics.FillRectangle(brush, new Rectangle(textPosition, Measure(e.Graphics, textBox1.SelectedText)));
             }
             else
             {
                 //start of multiline selection
                        
                 textPosition = GetOffsetFromCharIndex(e.Graphics, textBox1.SelectionStart);
                 textPosition.X -= topLeftPosition.X - 5;
                 textPosition.Y -= topLeftPosition.Y;
                 var SelStartLineLength =  textBox1.SelectionStart - textBox1.GetFirstCharIndexFromLine(SelStartLine);
                 e.Graphics.FillRectangle(brush, new Rectangle(textPosition, Measure(e.Graphics, textBox1.Lines[SelStartLine].Substring(SelStartLineLength))));
                        
                 //full lines between selections
                 for (var i = SelStartLine+1; i < SelStopLine; i++)
                 {
                     textPosition = GetOffsetFromCharIndex(e.Graphics, textBox1.GetFirstCharIndexFromLine(i));
                     textPosition.X -= topLeftPosition.X - 5;
                     textPosition.Y -= topLeftPosition.Y;
                     e.Graphics.FillRectangle(brush, new Rectangle(textPosition, Measure(e.Graphics, textBox1.Lines[i])));
                 }
               
                 //end of multiline selection
                 textPosition = GetOffsetFromCharIndex(e.Graphics, textBox1.GetFirstCharIndexFromLine(SelStopLine));
                 textPosition.X -= topLeftPosition.X - 5;
                 textPosition.Y -= topLeftPosition.Y;
                 var SelStopLineLength = textBox1.SelectionStart + textBox1.SelectionLength - textBox1.GetFirstCharIndexFromLine(SelStopLine);
                 var LastLine = (SelStopLineLength != textBox1.Lines[SelStopLine].Length)
                     ? textBox1.Lines[SelStopLine].Remove(SelStopLineLength)
                     : textBox1.Lines[SelStopLine];
                     e.Graphics.FillRectangle(brush, new Rectangle(textPosition, Measure(e.Graphics, LastLine)));
             }
         }

         textPosition = GetOffsetFromCharIndex(e.Graphics, textBox1.SelectionStart + textBox1.SelectionLength);
         textPosition.X -= topLeftPosition.X - 5;
         textPosition.Y -= topLeftPosition.Y;
         e.Graphics.DrawLine(new Pen(Color.White), textPosition.X, textPosition.Y, textPosition.X, textPosition.Y + 23);
     }
 }*/
        #endregion
        
        #region Debug Fields
        public Stopwatch DebugStopwatch = new Stopwatch();
        private Int64 _totalTokenize, _totalCheckChanges, _totalApplyColors;
        #endregion
        
        #region Structs
        public struct SFontColor
        {
            public Font StyleFont;
            public Color StyleColor;

            public static bool operator ==(SFontColor a, SFontColor b)
            {
                return (a.StyleColor == b.StyleColor) & (a.StyleFont.Equals(b.StyleFont));
            }
            public static bool operator !=(SFontColor a, SFontColor b)
            {
                return (a.StyleColor != b.StyleColor) | !(a.StyleFont.Equals(b.StyleFont));
            }
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

       public struct SDelimiter
        {
            public char[] Tokens;
            public SFontColor Style;
        }

        public struct SWord
        {
            public string[] Words;
            public SFontColor Style;
        }

        public struct SSpan
        {
            public string StartDelimiter;
            public string StopDelimiter; //or EOL
            public char EscapeChar;
            public SFontColor Style;
        }
        #endregion 

        #region Lists
        public List<SToken> TokenList, LastTokenList;
        public SDelimiter[] Delimiters;
        public SWord[] Words;
        public SSpan[] Spans;
        #endregion
        
        public Point BoxOriginPoint; // RichEditBox client area point
        public bool InParser;

        public SFontColor 
            DefaultTextFont,    //default fallback font style / richeditbox font.
            ValuesFont,         //font values (int bool float etc)
            DelFont, OpeFont,   //delimiters fonts
            ResFont, TypFont, FunFont,  //words fonts
            ComFont, StrFont;   // spans fonts


        public bool IsValue(string s)
        {
            double doubleBuffer;
            var result = double.TryParse(s.Trim().Replace('.', ','), out doubleBuffer);
            return (result | (s.ToUpper() == "TRUE") | (s.ToUpper() == "FALSE"));
        }

        public SFontColor GetDelimiterFont(char c)
        {
            foreach (var delimiterStruct in Delimiters)
            {
                if (delimiterStruct.Tokens.Contains(c)) return delimiterStruct.Style;
            }
            return DefaultTextFont;
        }

        public SFontColor GetWordFont(string s)
        {
            foreach (var wordStruct in Words)
            {
                if (wordStruct.Words.Contains(s)) return wordStruct.Style;
                if (IsValue(s)) return ValuesFont;
            }
            return DefaultTextFont;
        }

        public List<SToken> Pre_Tokenize(string[] lines)
        {
            var textOffset = 0;
            var result = new List<SToken>();
            foreach (var line in lines)
            {


                //TODO per SSpans with escape chars
                if (line.Contains("//"))
                {
                    var commentStart = line.IndexOf("//",StringComparison.InvariantCulture);
                    result.Add(new SToken
                    {
                        Offset = textOffset + commentStart,
                        Style = ComFont,
                        Text = line.Substring(commentStart, line.Length - commentStart)
                    });
                    result.AddRange(Tokenize(line.Substring(0, commentStart), textOffset));

                } 
                else
                {
                    result.AddRange(Tokenize(line, textOffset));
                }



                textOffset = textOffset + line.Length + 1; // implicit "\n",  +1 for "\r"
            }
            return result;
        } 

        public List<SToken> Tokenize(string line, int offset)
        {
            var result = new List<SToken>();
            var linePointer = 0;
            for (var i = 0; i < line.Length; i++)
            {
                if (Delimiters.Any(delimiterStuct => delimiterStuct.Tokens.Contains(line[i])))
                {
                    if (linePointer != i)
                    {
                        var currentText = line.Substring(linePointer, i - linePointer);
                        result.Add(new SToken
                        {
                            Offset = linePointer + offset,
                            Text = currentText,
                            Style = GetWordFont(currentText)
                        });
                    }

                    result.Add(new SToken
                    {
                        Offset = i + offset,
                        Text = line[i].ToString(CultureInfo.InvariantCulture),
                        Style = GetDelimiterFont(line[i])
                    });
                    linePointer = i + 1;
                }

            }
            if (linePointer < line.Length) result.Add(new SToken
            {
                Offset = linePointer + offset,
                Text = line.Substring(linePointer, line.Length - linePointer),
                Style = GetWordFont(line.Substring(linePointer, line.Length - linePointer))
            });
            return result;
        }

        public Form1()
        {
            InitializeComponent();

            LastTokenList = new List<SToken>();
            InParser = false;
           
            DefaultTextFont.StyleColor = richTextBox1.ForeColor;
            DefaultTextFont.StyleFont = richTextBox1.Font;

            ValuesFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Regular);
            ValuesFont.StyleColor = Color.RoyalBlue;

            Delimiters = new []
            {
                new SDelimiter { //whitespaces and breaks
                Tokens = new[] { '\t', '\r', '\n', ' ', ',', ';', '(', ')', '{', '}'},
                Style = new SFontColor
                {
                    StyleColor = Color.Gray,
                    StyleFont = new Font(richTextBox1.Font, FontStyle.Bold)
                }},               
                new SDelimiter { //operators
                Tokens = new[] { '/', '+', '-', '*', '=', '<', '>', '!' },
                Style = new SFontColor
                {
                    StyleColor = Color.Red,
                    StyleFont = new Font(richTextBox1.Font, FontStyle.Bold)
                }}
            };
            

            ResFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Regular);
            ResFont.StyleColor = Color.Orange; 
            TypFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Regular);
            TypFont.StyleColor = Color.Yellow;
            FunFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Bold);
            FunFont.StyleColor = Color.Lime;

            Words = new[]
            {
                new SWord
                {   Words = new [] {"#version", "uniform", "layout", "in", "out", "location", "void", "for", "else", "if", "main",
                    "smooth", "varying", "const", "flat​", "noperspective​"},
                    Style = ResFont},
                new SWord
                {   Words = new [] {"bool", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4"},
                    Style = TypFont},
                new SWord
                {   Words = new []{"gl_Position", "min", "max", "dot", "normalize", "clamp", "mix"},
                    Style = FunFont}
            };

            ComFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Italic);
            ComFont.StyleColor = Color.DarkGreen;
            StrFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Bold);
            StrFont.StyleColor = Color.Violet;

            Spans = new[]
            {
                new SSpan
                {   StartDelimiter = "//", StopDelimiter = "\n", EscapeChar = '\n', Style = ComFont},
                new SSpan
                {StartDelimiter = "\"", StopDelimiter = "\"", EscapeChar = '\\', Style = StrFont}
            };
        }

        private void button1_Click(object sender, EventArgs e) // Force Parsing
        {

         //   TokenList = Pre_Tokenize(textBox1.Lines);
          //  label1.Refresh();
            LastTokenList = new List<SToken>();
            InParser = false;
            timer1_Tick(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = DefaultTextFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.ForeColor = colorDialog1.Color;
                label1.Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = DelFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                DelFont.StyleColor = colorDialog1.Color;
                label1.Refresh();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = FunFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                FunFont.StyleColor = colorDialog1.Color;
                label1.Refresh();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = ResFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ResFont.StyleColor = colorDialog1.Color;
                label1.Refresh();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = ValuesFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ValuesFont.StyleColor = colorDialog1.Color;
                label1.Refresh();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TypFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                TypFont.StyleColor = colorDialog1.Color;
                label1.Refresh();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TokenList = Pre_Tokenize(textBox1.Lines);
            label1.Refresh();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            label1.Refresh();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            label1.Refresh();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            label1.Refresh();
        }

        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            label1.Refresh();
        }

        private bool[] CheckForChanges()
        {
            var result = new bool[TokenList.Count];

            if ((TokenList.Count > 0) & (LastTokenList.Count > 0))
            {

                if (TokenList.Count >= LastTokenList.Count)
                {
                    var firstMismatch = 0;
                    var lastMismatch = TokenList.Count - 1;
                    var tokenCountsDelta = TokenList.Count - LastTokenList.Count;

                    while ((TokenList[firstMismatch] == LastTokenList[firstMismatch]) & (firstMismatch < LastTokenList.Count - 1))
                    {
                        firstMismatch++;
                    }
                    while ((TokenList[lastMismatch] == LastTokenList[lastMismatch - tokenCountsDelta]) & (lastMismatch > firstMismatch))
                    {
                        lastMismatch--;
                    }

                        for (var j = 0; j < TokenList.Count; j++)
                        {
                            result[j] = (j >= firstMismatch) & (j <= lastMismatch);
                        }

                    Debug.WriteLine("New > Old - First: " + firstMismatch+ " - Last: "+ lastMismatch + " - Total: " + TokenList.Count);
                }
                else
                {
                    var firstMismatch = 0;
                    var lastMismatch = LastTokenList.Count - 1;
                    var tokenCountsDelta = LastTokenList.Count - TokenList.Count;

                    while ((LastTokenList[firstMismatch] == TokenList[firstMismatch]) & (firstMismatch < TokenList.Count - 1))
                    {
                        firstMismatch++;
                    }
                    while ((LastTokenList[lastMismatch] == TokenList[lastMismatch - tokenCountsDelta]) & (lastMismatch > firstMismatch))
                    {
                        lastMismatch--;
                    }

                        for (var j = 0; j < TokenList.Count; j++)
                        {
                            result[j] = (j >= firstMismatch) & (j <= lastMismatch);
                        }

                    Debug.WriteLine("New < Old - First: " + firstMismatch + " - Last: " + lastMismatch + " - Total: " + TokenList.Count);
                }
            } 
            else
            {
                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = true;
                }
            }

            LastTokenList = new List<SToken>(TokenList);
            return result;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!InParser)
            {
                InParser = true;

                //debug / perf test
                _totalApplyColors = 0;
                _totalCheckChanges = 0;
                _totalTokenize = 0;
                var totalUpdated = 0;
                DebugStopwatch.Start();

                    TokenList = Pre_Tokenize(richTextBox1.Lines);
                    _totalTokenize += DebugStopwatch.Elapsed.Ticks;
                    DebugStopwatch.Restart();

                   var changedTokens = CheckForChanges();
                    _totalCheckChanges += DebugStopwatch.Elapsed.Ticks;
                    DebugStopwatch.Restart();

                    SuspendUpdateClass.Suspend(richTextBox1);
                    //  richTextBox1.SuspendLayout();

                    var carretPositionBuffer = richTextBox1.SelectionStart;
                    var carretLengthBuffer = richTextBox1.SelectionLength;

                    var boxOrigin = richTextBox1.GetCharIndexFromPosition(BoxOriginPoint);
                    for (var i = 0; i < TokenList.Count; i++)
                       if (changedTokens[i])
                        {
                           totalUpdated++;
                            richTextBox1.SelectionStart = TokenList[i].Offset;
                            richTextBox1.SelectionLength = TokenList[i].Text.Length;
                            richTextBox1.SelectionColor = TokenList[i].Style.StyleColor;
                            richTextBox1.SelectionFont = TokenList[i].Style.StyleFont;
                        }

                    richTextBox1.SelectionStart = boxOrigin;
                    richTextBox1.ScrollToCaret();

                    richTextBox1.SelectionStart = carretPositionBuffer;
                    richTextBox1.SelectionLength = carretLengthBuffer;

                    richTextBox1.SelectionColor = DefaultTextFont.StyleColor;
                    richTextBox1.SelectionFont = DefaultTextFont.StyleFont;

                    //    richTextBox1.ResumeLayout();
                    SuspendUpdateClass.Resume(richTextBox1);
                    InParser = false;

                    _totalApplyColors += DebugStopwatch.Elapsed.Ticks;
                    DebugStopwatch.Restart();

            DebugStopwatch.Stop();
            Debug.WriteLine("Tokenize: " + (1000 * _totalTokenize / (float)Stopwatch.Frequency).ToString("F6") +"ms");
            Debug.WriteLine("CheckChanges: " + (1000 * _totalCheckChanges / (float)Stopwatch.Frequency).ToString("F6") + "ms");
            Debug.WriteLine("ApplyColors: " + (1000 * _totalApplyColors / (float)Stopwatch.Frequency).ToString("F6") + "ms");
            Debug.WriteLine("TokenUpdated: " + totalUpdated + " / " + changedTokens.Count());

                }
        }

        private void richTextBox1_FontChanged(object sender, EventArgs e)
        {
            DefaultTextFont.StyleColor = richTextBox1.ForeColor;
            DefaultTextFont.StyleFont = richTextBox1.Font;
        }

        private void richTextBox1_Resize(object sender, EventArgs e)
        {
            BoxOriginPoint = new Point(richTextBox1.ClientSize) { Y = 1 };
        }
    }
}
