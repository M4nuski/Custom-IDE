using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ShaderIDE
{
    public partial class Form1 : Form
    {
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

        #region Properties
        public Point BoxOriginPoint; // RichEditBox client area point
        public bool InParser;

        public SFontColor
            DefaultTextFont,    //default fallback font style / richeditbox font.
            ValuesFont,         //font values (int bool float etc)
            DelFont, OpeFont,   //delimiters fonts
            ResFont, TypFont, FunFont,  //words fonts
            ComFont, StrFont, TdoFont;   // spans fonts
        #endregion

        #region Parsing / Tokens
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
            }
            if (IsValue(s)) return ValuesFont;
            return DefaultTextFont;
        }

        public List<SToken> TokenizeLines(string[] lines)
        {
            var textOffset = 0;
            var result = new List<SToken>();

            foreach (var line in lines)
            {
                result.AddRange(Spans.Any(currentSpan => line.Contains(currentSpan.StartDelimiter))
                    ? TokenizeLinePerSpan(line, textOffset)
                    : TokenizeLinePerDelimiter(line, textOffset));

                textOffset += line.Length + 1;// implicit "\n",  +1 for "\r"
            }
            return result;
        }

        public List<SToken> TokenizeLinePerSpan(string line, int offset)
        {
            var result = new List<SToken>();
            var spanStartIndices = new int[Spans.Count()];
            var lineOffset = 0;

            while (line.Length > 0)
            {
                // Get list of indexOf for each span StartDelimiters
                for (var j = 0; j < Spans.Count(); j++)
                {
                    spanStartIndices[j] = line.IndexOf(Spans[j].StartDelimiter, StringComparison.Ordinal);
                } 
                // Find the first one on the line
                var firstSpanStartIndex = line.Length;
                var firstSpanType = -1;
                for (var j = 0; j < spanStartIndices.Length; j++)
                {
                    if ((spanStartIndices[j] >= 0) & (spanStartIndices[j] < firstSpanStartIndex))
                    {
                        firstSpanStartIndex = spanStartIndices[j];
                        firstSpanType = j;
                    }
                }
                if (firstSpanType > -1) //Check if there is still a span to tokenize in the line
                { 
                    if (spanStartIndices[firstSpanType] > 0)
                    {
                        // Tokenize line up to first occurence
                        result.AddRange(TokenizeLinePerDelimiter(line.Substring(0, spanStartIndices[firstSpanType]),
                            offset + lineOffset));
                        // Increase Line Pointer up to current position
                        lineOffset += spanStartIndices[firstSpanType];
                        // Reduce line to current start of span
                        line = line.Substring(spanStartIndices[firstSpanType]);
                    }

                    // Find end of current Span
                    var spanEndIndex = line.IndexOf(Spans[firstSpanType].StopDelimiter, Spans[firstSpanType].StartDelimiter.Length, StringComparison.Ordinal);
                    if (spanEndIndex == -1) spanEndIndex = line.Length;

                    // Skip Escape Char
                    while ((spanEndIndex < line.Length) & (line[spanEndIndex - 1] == Spans[firstSpanType].EscapeChar))
                    {
                        spanEndIndex = line.IndexOf(Spans[firstSpanType].StopDelimiter, spanEndIndex + 1, StringComparison.Ordinal);
                        if (spanEndIndex == -1) spanEndIndex = line.Length;
                    }

                    // Include Stop Delimiter
                    if (spanEndIndex < line.Length)
                    {
                        spanEndIndex += Spans[firstSpanType].StopDelimiter.Length;
                        if (spanEndIndex > line.Length) spanEndIndex = line.Length;
                    }

                    // Create Token for current Span
                    result.Add(new SToken
                    {
                        Offset = offset + lineOffset,
                        Text = line.SafeRemove(spanEndIndex),
                        Style = Spans[firstSpanType].Style
                    });

                    // Increase Line Pointer to next char
                    line = line.Substring(spanEndIndex);
                    lineOffset += spanEndIndex;
                }
                else 
                {   // No more span in this line
                    result.AddRange(TokenizeLinePerDelimiter(line, offset + lineOffset));
                    line = "";
                }
            } // while (line.Length > 0)
            return result;
        } 

        public List<SToken> TokenizeLinePerDelimiter(string line, int offset)
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
        #endregion

        #region Initialization and basic Form Events
        public Form1()
        {
            InitializeComponent();

            LastTokenList = new List<SToken>();
            InParser = false;

            DefaultTextFont.StyleColor = richTextBox1.ForeColor;
            DefaultTextFont.StyleFont = richTextBox1.Font;

            ValuesFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Regular);
            ValuesFont.StyleColor = Color.RoyalBlue;

            Delimiters = new[]
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
                    StyleColor = Color.DarkTurquoise,
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
            TdoFont.StyleFont = new Font(richTextBox1.Font, FontStyle.Underline);
            TdoFont.StyleColor = Color.DeepSkyBlue;

            Spans = new[]
            {   new SSpan
                {StartDelimiter = "//TODO", StopDelimiter = "\n", EscapeChar = '\n', Style = TdoFont},
                new SSpan
                {StartDelimiter = "//", StopDelimiter = "\n", EscapeChar = '\n', Style = ComFont},
                new SSpan
                {StartDelimiter = "\"", StopDelimiter = "\"", EscapeChar = '\\', Style = StrFont}
            };
        }

        private void richTextBox1_Resize(object sender, EventArgs e)
        {
            BoxOriginPoint = new Point(richTextBox1.ClientSize) { Y = 1 };
        }
        #endregion

        #region Font and Styles Selections
        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = DefaultTextFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.ForeColor = colorDialog1.Color;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = DelFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                DelFont.StyleColor = colorDialog1.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = FunFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                FunFont.StyleColor = colorDialog1.Color;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = ResFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ResFont.StyleColor = colorDialog1.Color;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = ValuesFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ValuesFont.StyleColor = colorDialog1.Color;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = TypFont.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                TypFont.StyleColor = colorDialog1.Color;
            }
        }

        private void richTextBox1_FontChanged(object sender, EventArgs e)
        {
            DefaultTextFont.StyleColor = richTextBox1.ForeColor;
            DefaultTextFont.StyleFont = richTextBox1.Font;
            richEditBox_ReDraw(sender, e);
        }
        #endregion

        #region Editor Update
        private void button1_Click(object sender, EventArgs e) // Force Parsing
        {
            LastTokenList = new List<SToken>(); //Clear old list
            InParser = false;
            richEditBox_ReDraw(sender, e);
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

                    Debug.WriteLine("New > Old - First: " + firstMismatch + " - Last: " + lastMismatch + " - Total: " + TokenList.Count);
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

        private int ReDraw(bool[] changedList)
        {
            var totalReDrawn = 0;

                SuspendUpdate.Suspend(richTextBox1);
                richTextBox1.SuspendLayout();

                var carretPositionBuffer = richTextBox1.SelectionStart;
                var carretLengthBuffer = richTextBox1.SelectionLength;

                var boxOrigin = richTextBox1.GetCharIndexFromPosition(BoxOriginPoint);

                for (var i = 0; i < TokenList.Count; i++)
                    if (changedList[i] | checkBox1.Checked)
                    {
                        totalReDrawn++;//Debug
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

                richTextBox1.ResumeLayout();
                SuspendUpdate.Resume(richTextBox1);
            return totalReDrawn;
        }

        private void richEditBox_ReDraw(object sender, EventArgs e)
        {
            if (!InParser)
            {
                InParser = true;
                
                _totalApplyColors = 0;//debug
                _totalCheckChanges = 0;//debug
                _totalTokenize = 0;//debug
                DebugStopwatch.Restart();//debug

                TokenList = TokenizeLines(richTextBox1.Lines);
                
                _totalTokenize += DebugStopwatch.Elapsed.Ticks;//Debug
                DebugStopwatch.Restart();//Debug

                var changedTokens = CheckForChanges();
                
                _totalCheckChanges += DebugStopwatch.Elapsed.Ticks;//Debug
                DebugStopwatch.Restart();//Debug

                var totalUpdated = ReDraw(changedTokens);

                InParser = false;

                _totalApplyColors += DebugStopwatch.Elapsed.Ticks;//Debug
                DebugStopwatch.Restart();//Debug

                DebugStopwatch.Stop();//debug
                Debug.WriteLine("Tokenize: " + (1000 * _totalTokenize / (float)Stopwatch.Frequency).ToString("F6") + "ms");//debug
                Debug.WriteLine("CheckChanges: " + (1000 * _totalCheckChanges / (float)Stopwatch.Frequency).ToString("F6") + "ms");//debug
                Debug.WriteLine("ApplyColors: " + (1000 * _totalApplyColors / (float)Stopwatch.Frequency).ToString("F6") + "ms");//debug
                Debug.WriteLine("TokenUpdated: " + totalUpdated + " / " + changedTokens.Count());//debug

            }
        }
        #endregion
    }

}
