using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Serialization;

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
    #endregion

    public partial class Form1 : Form
    {
        #region Debug Fields
        private readonly Stopwatch _debugStopwatch = new Stopwatch();
        private Int64 _totalTokenize, _totalCheckChanges, _totalApplyColors;
        #endregion

        private struct SToken
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

        #region Lists
        private List<SToken> _tokenList, _lastTokenList;
        public SDelimiter[] Delimiters;
        public SWord[] Words;
        public SSpan[] Spans;
        #endregion

        #region Properties
        private Point _boxOriginPoint; // RichEditBox client area point
        public Color BackgroundColor, CurrentLineColor, ErrorLineColor;
        public int[] ErrorList;
        private bool _inParser;
        private int _lineSelectionStart, _lineSelectionLength;

        public SFontColor
            DefaultTextFont,    //default fallback font style / richeditbox font.
            ValuesFont,         //font values (int bool float etc)
            DelFont, OpeFont,   //delimiters fonts
            ResFont, TypFont, FunFont,  //words fonts
            ComFont, StrFont, TdoFont;   // spans fonts
        #endregion

        #region Parsing / Tokens
        private static bool IsValue(string s)
        {
            double doubleBuffer;
            var result = double.TryParse(s.Trim().Replace('.', ','), out doubleBuffer);
            return (result | (s.ToUpper() == "TRUE") | (s.ToUpper() == "FALSE"));
        }

        private SFontColor GetDelimiterFont(char c)
        {
            foreach (var delimiterStruct in Delimiters)
            {
                if (delimiterStruct.Tokens.Contains(c)) return delimiterStruct.Style;
            }
            return DefaultTextFont;
        }

        private SFontColor GetWordFont(string s)
        {
            foreach (var wordStruct in Words)
            {
                if (wordStruct.Words.Contains(s)) return wordStruct.Style;
            }
            if (IsValue(s)) return ValuesFont;
            return DefaultTextFont;
        }

        private List<SToken> TokenizeLines(string[] lines)
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

        private List<SToken> TokenizeLinePerSpan(string line, int offset)
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

        private List<SToken> TokenizeLinePerDelimiter(string line, int offset)
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

            _lastTokenList = new List<SToken>();
            _inParser = false;

            DefaultTextFont = new SFontColor(richTextBox1.Font, richTextBox1.ForeColor);
            ValuesFont = new SFontColor(new Font(richTextBox1.Font, FontStyle.Regular), Color.RoyalBlue);

            Delimiters = new[]
            {
                new SDelimiter { //whitespaces and breaks
                    Name = "Breaks",
                    Tokens = new[] { '\t', '\r', '\n', ' ', ',', ';', '(', ')', '{', '}'},
                    Style = new SFontColor
                    {
                        StyleColor = Color.Gray,
                        StyleFont = new Font(richTextBox1.Font, FontStyle.Bold)
                    }},               
                new SDelimiter { //operators
                    Name = "Operators",
                    Tokens = new[] { '/', '+', '-', '*', '=', '<', '>', '!' },
                    Style = new SFontColor
                    {
                        StyleColor = Color.DarkTurquoise,
                        StyleFont = new Font(richTextBox1.Font, FontStyle.Bold)
                    }}
            };


            ResFont = new SFontColor(new Font(richTextBox1.Font, FontStyle.Regular), Color.Orange);
            TypFont = new SFontColor(new Font(richTextBox1.Font, FontStyle.Regular), Color.Yellow);
            FunFont = new SFontColor(new Font(richTextBox1.Font, FontStyle.Bold), Color.Lime);

            Words = new[]
            {
                new SWord
                {   Name = "Reserved",
                    Words = new [] {"#version", "uniform", "layout", "in", "out", "location", "void", "for", "else", "if", "main",
                    "smooth", "varying", "const", "flat​", "noperspective​"},
                    Style = ResFont},
                new SWord
                {   Name = "Types",
                    Words = new [] {"bool", "int", "float", "vec2", "vec3", "vec4", "mat3", "mat4"},
                    Style = TypFont},
                new SWord
                {   Name = "Functions",
                    Words = new []{"gl_Position", "min", "max", "dot", "normalize", "clamp", "mix"},
                    Style = FunFont}
            };

            ComFont = new SFontColor(new Font(richTextBox1.Font, FontStyle.Italic), Color.Green);
            StrFont = new SFontColor(new Font(richTextBox1.Font, FontStyle.Bold), Color.Violet);
            TdoFont = new SFontColor(new Font(richTextBox1.Font, FontStyle.Underline), Color.DeepSkyBlue);

            Spans = new[]
            {   new SSpan
                {Name = "TODO Comment", StartDelimiter = "//TODO", StopDelimiter = "\n", EscapeChar = '\n', Style = TdoFont},
                new SSpan
                {Name = "Comment", StartDelimiter = "//", StopDelimiter = "\n", EscapeChar = '\n', Style = ComFont},
                new SSpan
                {Name = "Inline String", StartDelimiter = "\"", StopDelimiter = "\"", EscapeChar = '\\', Style = StrFont}
            };

            CurrentLineColor = Color.FromArgb(255, 56, 56, 56);
            BackgroundColor = richTextBox1.BackColor;
            ErrorLineColor = Color.DarkRed;
        }

        private void richTextBox1_Resize(object sender, EventArgs e)
        {
            _boxOriginPoint = new Point(richTextBox1.ClientSize) { Y = 1 };
        }
        #endregion
        
        #region Font and Styles Selections
        //TODO remake with dialogs
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
            try
            {
                var writer = new XmlSerializer(_tokenList.GetType());
                var file = new StreamWriter("Tokens.xml");
                writer.Serialize(file, Words);
                file.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
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
            _lastTokenList = new List<SToken>(); //Clear old list
            _inParser = false;
            richEditBox_ReDraw(sender, e);
        }

        private bool[] CheckForChanges()
        {
            var result = new bool[_tokenList.Count];

            if ((_tokenList.Count > 0) & (_lastTokenList.Count > 0))
            {

                if (_tokenList.Count >= _lastTokenList.Count)
                {
                    var firstMismatch = 0;
                    var lastMismatch = _tokenList.Count - 1;
                    var tokenCountsDelta = _tokenList.Count - _lastTokenList.Count;

                    while ((_tokenList[firstMismatch] == _lastTokenList[firstMismatch]) & (firstMismatch < _lastTokenList.Count - 1))
                    {
                        firstMismatch++;
                    }
                    while ((_tokenList[lastMismatch] == _lastTokenList[lastMismatch - tokenCountsDelta]) & (lastMismatch > firstMismatch))
                    {
                        lastMismatch--;
                    }

                    for (var j = 0; j < _tokenList.Count; j++)
                    {
                        result[j] = (j >= firstMismatch) & (j <= lastMismatch);
                    }

                    Debug.WriteLine("New > Old - First: " + firstMismatch + " - Last: " + lastMismatch + " - Total: " + _tokenList.Count);
                }
                else
                {
                    var firstMismatch = 0;
                    var lastMismatch = _lastTokenList.Count - 1;
                    var tokenCountsDelta = _lastTokenList.Count - _tokenList.Count;

                    while ((_lastTokenList[firstMismatch] == _tokenList[firstMismatch]) & (firstMismatch < _tokenList.Count - 1))
                    {
                        firstMismatch++;
                    }
                    while ((_lastTokenList[lastMismatch] == _tokenList[lastMismatch - tokenCountsDelta]) & (lastMismatch > firstMismatch))
                    {
                        lastMismatch--;
                    }

                    for (var j = 0; j < _tokenList.Count; j++)
                    {
                        result[j] = (j >= firstMismatch) & (j <= lastMismatch);
                    }

                    Debug.WriteLine("New < Old - First: " + firstMismatch + " - Last: " + lastMismatch + " - Total: " + _tokenList.Count);
                }
            }
            else
            {
                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = true;
                }
            }

            _lastTokenList = new List<SToken>(_tokenList);
            return result;
        }

        private void GetSelectionFromLine(int lineNumber)
        {
            var maxLines = richTextBox1.Lines.Count();
            if (lineNumber < maxLines)
            {
                _lineSelectionStart = richTextBox1.GetFirstCharIndexFromLine(lineNumber);
                _lineSelectionLength = richTextBox1.GetFirstCharIndexFromLine(lineNumber + 1) - _lineSelectionStart;
            }
            else if (lineNumber == maxLines)
            {
                _lineSelectionStart = richTextBox1.GetFirstCharIndexFromLine(lineNumber);
                _lineSelectionLength = richTextBox1.TextLength - _lineSelectionStart;
            }
            else
            {
                _lineSelectionStart = 0;
                _lineSelectionLength = 0;
            }
        }

        private int ReDraw(bool[] changedList)
        {
            var totalReDrawn = 0; //debug

            //richTextBox1.SuspendLayout();
            SuspendUpdate.Suspend(richTextBox1);

            // Save current state
            var carretPositionBuffer = richTextBox1.SelectionStart;
            var carretLengthBuffer = richTextBox1.SelectionLength;
            var boxOrigin = richTextBox1.GetCharIndexFromPosition(_boxOriginPoint);

            // Get Current Line informations
            GetSelectionFromLine(richTextBox1.GetLineFromCharIndex(richTextBox1.GetFirstCharIndexOfCurrentLine()));
            var currentLineIndex = _lineSelectionStart;
            var currentLineLength = _lineSelectionLength;
          
            // Reset Background
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = BackgroundColor;
            
            // Update Colors
            for (var i = 0; i < _tokenList.Count; i++)
                if (changedList[i] | checkBox1.Checked)
                {
                    totalReDrawn++;//Debug
                    richTextBox1.Select(_tokenList[i].Offset, _tokenList[i].Text.Length);
                    richTextBox1.SelectionColor = _tokenList[i].Style.StyleColor;
                    richTextBox1.SelectionFont = _tokenList[i].Style.StyleFont;
                }

            // Current line Color
            richTextBox1.Select(currentLineIndex, currentLineLength);
            richTextBox1.SelectionBackColor = CurrentLineColor;

            // Errors line Color
            foreach (var eInt in ErrorList)
            {
                GetSelectionFromLine(eInt);
                richTextBox1.Select(_lineSelectionStart, _lineSelectionLength);
                richTextBox1.SelectionBackColor = ErrorLineColor;
            }

            // Reset View
            richTextBox1.Select(boxOrigin, 0);
            richTextBox1.ScrollToCaret();

            // Reset Selections
            richTextBox1.Select(carretPositionBuffer, carretLengthBuffer);
            richTextBox1.SelectionColor = DefaultTextFont.StyleColor;
            richTextBox1.SelectionFont = DefaultTextFont.StyleFont;

            //richTextBox1.ResumeLayout();
            SuspendUpdate.Resume(richTextBox1);
            return totalReDrawn;
        }

        private void richEditBox_ReDraw(object sender, EventArgs e)
        {
            if (!_inParser)
            {
                _inParser = true;
                
                _totalApplyColors = 0;//debug
                _totalCheckChanges = 0;//debug
                _totalTokenize = 0;//debug
                _debugStopwatch.Restart();//debug

                _tokenList = TokenizeLines(richTextBox1.Lines);


                var rdn = new Random();//debug
                ErrorList = new int[3];//debug
                ErrorList[0] = rdn.Next(richTextBox1.Lines.Count());//debug
                ErrorList[1] = rdn.Next(richTextBox1.Lines.Count());//debug
                ErrorList[2] = rdn.Next(richTextBox1.Lines.Count());//debug
                
                _totalTokenize += _debugStopwatch.Elapsed.Ticks;//Debug
                _debugStopwatch.Restart();//Debug

                var changedTokens = CheckForChanges();
                
                _totalCheckChanges += _debugStopwatch.Elapsed.Ticks;//Debug
                _debugStopwatch.Restart();//Debug

                var totalUpdated = ReDraw(changedTokens);

                _inParser = false;

                _totalApplyColors += _debugStopwatch.Elapsed.Ticks;//Debug
                _debugStopwatch.Restart();//Debug

                _debugStopwatch.Stop();//debug
                Debug.WriteLine("Tokenize: " + (1000 * _totalTokenize / (float)Stopwatch.Frequency).ToString("F6") + "ms");//debug
                Debug.WriteLine("CheckChanges: " + (1000 * _totalCheckChanges / (float)Stopwatch.Frequency).ToString("F6") + "ms");//debug
                Debug.WriteLine("ApplyColors: " + (1000 * _totalApplyColors / (float)Stopwatch.Frequency).ToString("F6") + "ms");//debug
                Debug.WriteLine("TokenUpdated: " + totalUpdated + " / " + changedTokens.Count());//debug

            }
        }
        #endregion
    }

}
