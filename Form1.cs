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

namespace ShaderIDE
{

   public partial class Form1 : Form
    {
        #region Debug Fields
        private readonly Stopwatch _debugStopwatch = new Stopwatch();
        private Int64 _totalTokenize, _totalCheckChanges, _totalApplyColors;
        #endregion

        #region Lists
        public List<TokenStruct> TokenList;
        private List<TokenStruct> _lastTokenList;
        private List<HighlightStruct> _highlights = new List<HighlightStruct>();
        #endregion

        #region Properties

        public ThemeStruct Theme;

        private Point _boxOriginPoint; // RichEditBox client area point

       private Point _lastMousePos, _currentMousePos;
       private int _hoverCount;
      

        public int[] ErrorList;
        private bool _inParser;
        private int _lineSelectionStart, _lineSelectionLength;

        private readonly WordStyleDialog _styleDialogWords = new WordStyleDialog();
        private readonly DelimiterStyleDialog _styleDialogDelimiters = new DelimiterStyleDialog();
        private readonly SpanStyleDialog _styleDialogSpans = new SpanStyleDialog();
        #endregion

        #region Parsing / Tokens
        private static bool IsValue(string s)
        {
            double doubleBuffer;
            var result = double.TryParse(s.Trim().Replace('.', ','), out doubleBuffer);
            return (result | (s.ToUpper() == "TRUE") | (s.ToUpper() == "FALSE"));
        }

        private FontAndColorStruct GetDelimiterFont(char c)
        {
            foreach (var delimiterStruct in Theme.Delimiters)
            {
                if (delimiterStruct.Keychars.Contains(c)) return delimiterStruct.Style;
            }
            return Theme.TextStyle;
        }

        private FontAndColorStruct GetWordFont(string s)
        {
            foreach (var wordStruct in Theme.Words)
            {
                if (wordStruct.Keywords.Contains(s)) return wordStruct.Style;
            }
            if (IsValue(s)) return Theme.ValueStyle;
            return Theme.TextStyle;
        }

        private List<TokenStruct> TokenizeLines(string[] lines)
        {
            var textOffset = 0;
            var result = new List<TokenStruct>();

            foreach (var line in lines)
            {
                result.AddRange(Theme.Spans.Any(currentSpan => line.Contains(currentSpan.StartKeyword))
                    ? TokenizeLinePerSpan(line, textOffset)
                    : TokenizeLinePerDelimiter(line, textOffset));

                textOffset += line.Length + 1;// implicit "\n",  +1 for "\r"
            }
            return result;
        }

        private List<TokenStruct> TokenizeLinePerSpan(string line, int offset)
        {
            var result = new List<TokenStruct>();
            var spanStartIndices = new int[Theme.Spans.Length];
            var lineOffset = 0;

            while (line.Length > 0)
            {
                // Get list of indexOf for each span StartDelimiters
                for (var j = 0; j < Theme.Spans.Length; j++)
                {
                    spanStartIndices[j] = line.IndexOf(Theme.Spans[j].StartKeyword, StringComparison.Ordinal);
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
                    var spanEndIndex = line.IndexOf(Theme.Spans[firstSpanType].StopKeyword, Theme.Spans[firstSpanType].StartKeyword.Length, StringComparison.Ordinal);
                    if (spanEndIndex == -1) spanEndIndex = line.Length;

                    // Skip Escape Char
                    while ((spanEndIndex < line.Length) & (line[spanEndIndex - 1] == Theme.Spans[firstSpanType].EscapeChar))
                    {
                        spanEndIndex = line.IndexOf(Theme.Spans[firstSpanType].StopKeyword, spanEndIndex + 1, StringComparison.Ordinal);
                        if (spanEndIndex == -1) spanEndIndex = line.Length;
                    }

                    // Include Stop Delimiter
                    if (spanEndIndex < line.Length)
                    {
                        spanEndIndex += Theme.Spans[firstSpanType].StopKeyword.Length;
                        if (spanEndIndex > line.Length) spanEndIndex = line.Length;
                    }

                    // Create Token for current Span
                    result.Add(new TokenStruct
                    {
                        Offset = offset + lineOffset,
                        Text = line.SafeRemove(spanEndIndex),
                        Style = Theme.Spans[firstSpanType].Style
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

        private List<TokenStruct> TokenizeLinePerDelimiter(string line, int offset)
        {
            var result = new List<TokenStruct>();
            var linePointer = 0;
            for (var i = 0; i < line.Length; i++)
            {
                if (Theme.Delimiters.Any(delimiterStuct => delimiterStuct.Keychars.Contains(line[i])))
                {
                    if (linePointer != i)
                    {
                        var currentText = line.Substring(linePointer, i - linePointer);
                        result.Add(new TokenStruct
                        {
                            Offset = linePointer + offset,
                            Text = currentText,
                            Style = GetWordFont(currentText)
                        });
                    }

                    result.Add(new TokenStruct
                    {
                        Offset = i + offset,
                        Text = line[i].ToString(CultureInfo.InvariantCulture),
                        Style = GetDelimiterFont(line[i])
                    });
                    linePointer = i + 1;
                }

            }
            if (linePointer < line.Length) result.Add(new TokenStruct
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

            _lastTokenList = new List<TokenStruct>();
            _inParser = false;

            Theme = ThemeHelper.DefaultGLSLDarkTheme(richTextBox1.Font);
            //Theme = ThemeHelper.DefaultGLSLLightTheme(richTextBox1.Font);
            //Theme = ThemeHelper.DefaultCSDarkTheme(richTextBox1.Font);
            //Theme = ThemeHelper.DefaultCSBlueTheme(richTextBox1.Font);
            _highlights.Add(new HighlightStruct(1, "Hint: don't talk too much in comments", Color.MidnightBlue)); //debug
            _highlights.Add(new HighlightStruct(8, "Warning: blah blah blah2", Color.Goldenrod)); //debug
            _highlights.Add(new HighlightStruct(38, "Warning: Color.GoldenRod is kinda weird", Color.Goldenrod)); //debug
            _highlights.Add(new HighlightStruct(8, "ERROR: blah blah blah", Color.DarkRed)); //debug

            PopulateMenu();
            force_Redraw(this, new EventArgs());
        }

        private void richTextBox1_Resize(object sender, EventArgs e)
        {
            _boxOriginPoint = new Point(richTextBox1.ClientSize) { Y = 1 };
        }
        #endregion
        
        #region Editor Update
        private void force_Redraw(object sender, EventArgs e) // Force Parsing
        {
            _lastTokenList = new List<TokenStruct>(); //Clear old list
            _inParser = false;
            richEditBox_ReDraw(sender, e);
        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            if (!_inParser & (TokenList != null))
            {
                _inParser = true;
                var dummyArray = new bool[TokenList.Count];
                ReDraw(dummyArray);
                _inParser = false;
            }
        }

        private bool[] CheckForChanges()
        {
            var result = new bool[TokenList.Count];

            if ((TokenList.Count > 0) & (_lastTokenList.Count > 0))
            {

                if (TokenList.Count >= _lastTokenList.Count)
                {
                    var firstMismatch = 0;
                    var lastMismatch = TokenList.Count - 1;
                    var tokenCountsDelta = TokenList.Count - _lastTokenList.Count;

                    while (ThemeHelper.TokenEqual(TokenList[firstMismatch], _lastTokenList[firstMismatch]) & (firstMismatch < _lastTokenList.Count - 1))
                    {
                        firstMismatch++;
                    }
                    while (ThemeHelper.TokenEqual(TokenList[lastMismatch], _lastTokenList[lastMismatch - tokenCountsDelta]) & (lastMismatch > (firstMismatch + tokenCountsDelta)))
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
                    var lastMismatch = _lastTokenList.Count - 1;
                    var tokenCountsDelta = _lastTokenList.Count - TokenList.Count;

                    while (ThemeHelper.TokenEqual(_lastTokenList[firstMismatch], TokenList[firstMismatch]) & (firstMismatch < TokenList.Count - 1))
                    {
                        firstMismatch++;
                    }
                    while (ThemeHelper.TokenEqual(_lastTokenList[lastMismatch], TokenList[lastMismatch - tokenCountsDelta]) & (lastMismatch > (firstMismatch + tokenCountsDelta)))
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

            _lastTokenList = new List<TokenStruct>(TokenList);
            return result;
        }

        private void GetSelectionFromLine(int lineNumber)
        {
            var maxLine = richTextBox1.Lines.Length - 1;
            if (lineNumber < maxLine)
            {
                _lineSelectionStart = richTextBox1.GetFirstCharIndexFromLine(lineNumber);
                _lineSelectionLength = richTextBox1.GetFirstCharIndexFromLine(lineNumber + 1) - _lineSelectionStart;
            }
            else 
            {
                _lineSelectionStart = richTextBox1.GetFirstCharIndexFromLine(maxLine);
                _lineSelectionLength = richTextBox1.TextLength-1 - _lineSelectionStart;
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
            richTextBox1.BackColor = Theme.BackgroundColor;
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Theme.BackgroundColor;
            
            // Update Colors
            for (var i = 0; i < TokenList.Count; i++)
                if (changedList[i])
                {
                    totalReDrawn++;//Debug
                    richTextBox1.Select(TokenList[i].Offset, TokenList[i].Text.Length);
                    richTextBox1.SelectionColor = TokenList[i].Style.StyleColor;
                    richTextBox1.SelectionFont = TokenList[i].Style.StyleFont;
                }

            // Line highlights Color
            foreach (var currentHighlight in _highlights)
            {
                GetSelectionFromLine(currentHighlight.LineNumber);
                richTextBox1.Select(_lineSelectionStart, _lineSelectionLength);
                richTextBox1.SelectionBackColor = currentHighlight.LineColor;
            }

            // Current line Color
            richTextBox1.Select(currentLineIndex, currentLineLength);
            richTextBox1.SelectionBackColor = Theme.CurrentLineColor;

            // Reset View
            richTextBox1.Select(boxOrigin, 0);
            richTextBox1.ScrollToCaret();

            // Reset Selections
            richTextBox1.Select(carretPositionBuffer, carretLengthBuffer);
            richTextBox1.SelectionColor = Theme.TextStyle.StyleColor;
            richTextBox1.SelectionFont = Theme.TextStyle.StyleFont;

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

                TokenList = TokenizeLines(richTextBox1.Lines);

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
                Debug.WriteLine("TokenUpdated: " + totalUpdated + " / " + changedTokens.Length);//debug

            }
        }
        #endregion

        #region Menu Strip Color and Style Settings
        private void MenuItem_FontClick(object sender, EventArgs e)
        {
            fontDialog1.Font = Theme.TextStyle.StyleFont;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Theme = ThemeHelper.ChangeFont(Theme, fontDialog1.Font);
                richTextBox1.Font = fontDialog1.Font;
                force_Redraw(sender, e);
            }
        }


        private void MenuItem_SaveClick(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ThemeHelper.SaveTheme(Theme, saveFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Error Saving Theme", MessageBoxButtons.OK);
                }   
            }
        }

        private void MenuItem_LoadClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Theme = ThemeHelper.LoadTheme(Theme, openFileDialog1.FileName);
                PopulateMenu();
                force_Redraw(sender, e);                
            }

        }

       private void SetToLargest(ToolStripItemCollection stripCollection)
       {
           var largestWidth = 0;
           for (var i = 0; i < stripCollection.Count; i++)
           {
               var currentWidth =
                   TextRenderer.MeasureText(stripCollection[i].Text, MenuStrip_TopTheme.Font).Width;
               largestWidth = (currentWidth > largestWidth) ? currentWidth : largestWidth;
           }
           for (var i = 0; i < stripCollection.Count; i++)
           {
               stripCollection[i].Width = largestWidth;
           }
       }

       private void PopulateMenu()
       {
           while (MenuStrip_TokensDelimiter.DropDownItems.Count > 2) MenuStrip_TokensDelimiter.DropDownItems.RemoveAt(2);
           for (var i = 0; i < Theme.Delimiters.Length; i++)
           {
               var currentItem = MenuStrip_TokensDelimiter.DropDownItems.Add(new ToolStripButton(
                   Theme.Delimiters[i].Name));
               MenuStrip_TokensDelimiter.DropDownItems[currentItem].Name = "D" + i.ToString(CultureInfo.InvariantCulture);
               MenuStrip_TokensDelimiter.DropDownItems[currentItem].Click += MenuItem_TokensClick;
           }
           SetToLargest(MenuStrip_TokensDelimiter.DropDownItems);

           while (MenuStrip_TokensWords.DropDownItems.Count > 2) MenuStrip_TokensWords.DropDownItems.RemoveAt(2);
           for (var i = 0; i < Theme.Words.Length; i++)
           {
               var currentItem = MenuStrip_TokensWords.DropDownItems.Add(new ToolStripButton(
                   Theme.Words[i].Name));
               MenuStrip_TokensWords.DropDownItems[currentItem].Name = "W" + i.ToString(CultureInfo.InvariantCulture);
               MenuStrip_TokensWords.DropDownItems[currentItem].Click += MenuItem_TokensClick;
           }
           SetToLargest(MenuStrip_TokensWords.DropDownItems);

           while (MenuStrip_TokensSpans.DropDownItems.Count > 2) MenuStrip_TokensSpans.DropDownItems.RemoveAt(2);
           for (var i = 0; i < Theme.Spans.Length; i++)
           {
               var currentItem = MenuStrip_TokensSpans.DropDownItems.Add(new ToolStripButton(
                   Theme.Spans[i].Name));
               MenuStrip_TokensSpans.DropDownItems[currentItem].Name = "S" + i.ToString(CultureInfo.InvariantCulture);
               MenuStrip_TokensSpans.DropDownItems[currentItem].Click += MenuItem_TokensClick;
           }
           SetToLargest(MenuStrip_TokensSpans.DropDownItems);
       }

       private void MenuItem_TokensClick(object sender, EventArgs e)
       {
           var sendeMenu = sender as ToolStripItem;
           if (sendeMenu != null)
           {
               //Check which on is called, spawn the dialog, if OK add to theme
               int currentIndex;
               if (int.TryParse(sendeMenu.Name.Substring(1), out currentIndex))
               {
                   if (sendeMenu.Name[0] == 'V')
                       if (_styleDialogWords.ShowDialog(Theme.ValueStyle, Theme.BackgroundColor) == DialogResult.OK)
                       {
                           var microsoftsWeirdMarshalingWariningEliminator =_styleDialogWords.DialogOutput;
                           Theme.ValueStyle = microsoftsWeirdMarshalingWariningEliminator.Style;
                       }
                   if (sendeMenu.Name[0] == 'D')
                   {
                    if (_styleDialogDelimiters.ShowDialog(Theme.Delimiters[currentIndex], Theme.BackgroundColor) == DialogResult.OK)
                       {
                           Theme.Delimiters[currentIndex] = _styleDialogDelimiters.DialogOutput;
                           MenuStrip_TokensDelimiter.DropDownItems[currentIndex + 2].Text = Theme.Delimiters[currentIndex].Name;
                           SetToLargest(MenuStrip_TokensDelimiter.DropDownItems);
                       }
                   }
                   if (sendeMenu.Name[0] == 'W')
                       if (_styleDialogWords.ShowDialog(Theme.Words[currentIndex], Theme.BackgroundColor) == DialogResult.OK)
                       {
                           Theme.Words[currentIndex] = _styleDialogWords.DialogOutput;
                           MenuStrip_TokensWords.DropDownItems[currentIndex + 2].Text = Theme.Words[currentIndex].Name;
                           SetToLargest(MenuStrip_TokensWords.DropDownItems);
                       }

                   if (sendeMenu.Name[0] == 'S')
                   {
                       if (_styleDialogSpans.ShowDialog(Theme.Spans[currentIndex], Theme.BackgroundColor) == DialogResult.OK)
                       {
                           Theme.Spans[currentIndex] = _styleDialogSpans.DialogOutput;
                           MenuStrip_TokensSpans.DropDownItems[currentIndex + 2].Text = Theme.Spans[currentIndex].Name;
                           SetToLargest(MenuStrip_TokensSpans.DropDownItems);
                       }
                   }

               } //tryparse
               force_Redraw(sender, e);
           }//not null
           else Text = @"Null Reference in TokensClick";
       }

       #endregion

       private Color GetColorDialogResult(Color initialColor)
       {
           colorDialog1.Color = initialColor;
           return colorDialog1.ShowDialog() == DialogResult.OK ? colorDialog1.Color : initialColor;
       }

       private static T[] AppendArray<T>(T[] inputArray, T newElement)
       {
           var newArray = new T[inputArray.Length + 1];
           inputArray.CopyTo(newArray, 0);
           newArray[newArray.Length - 1] = newElement;
           return newArray;
       }

       private void MenuItem_ColorsClick(object sender, EventArgs e)
       {
           var senderObject = sender as ToolStripItem;
           if (senderObject != null)
           {
               if (senderObject.Tag.ToString() == "TXT_COLOR") Theme.TextStyle.StyleColor = GetColorDialogResult(Theme.TextStyle.StyleColor);
               if (senderObject.Tag.ToString() == "BG_COLOR") Theme.BackgroundColor = GetColorDialogResult(Theme.BackgroundColor);
               if (senderObject.Tag.ToString() == "LINE_COLOR") Theme.CurrentLineColor = GetColorDialogResult(Theme.CurrentLineColor);
               force_Redraw(sender, e);
           }
           else Text = @"Null Reference in ColorClick";
       }

       private void MenuItem_Tokens_NewClick(object sender, EventArgs e)
       {
           var senderObject = sender as ToolStripItem;
           if (senderObject != null)
           {
               if (senderObject.Tag.ToString() == "DEL_NEW") 
                   if (_styleDialogDelimiters.ShowDialog(
                        new DelimiterStruct(Theme.TextStyle.StyleFont), 
                        Theme.BackgroundColor) == DialogResult.OK)
                   Theme.Delimiters = AppendArray(Theme.Delimiters, _styleDialogDelimiters.DialogOutput);

               if (senderObject.Tag.ToString() == "WORD_NEW")
                   if (_styleDialogWords.ShowDialog(
                        new WordStruct(Theme.TextStyle.StyleFont),
                        Theme.BackgroundColor) == DialogResult.OK)
                   Theme.Words = AppendArray(Theme.Words, _styleDialogWords.DialogOutput);

               if (senderObject.Tag.ToString() == "SPAN_NEW") 
                   if (_styleDialogSpans.ShowDialog(
                        new SpanStruct(Theme.TextStyle.StyleFont),
                        Theme.BackgroundColor) == DialogResult.OK)
                   Theme.Spans = AppendArray(Theme.Spans, _styleDialogSpans.DialogOutput);

               PopulateMenu();
               force_Redraw(sender, e);
           }
           else Text = @"Null Reference in NewTokensClick";
       }

       private void hoverTimer_Tick(object sender, EventArgs e)
       {
           if (_currentMousePos == _lastMousePos)
           {
               _hoverCount++;
               if ((_hoverCount > 5) & (toolTip1.Active))
               {
                   var hoverLine = richTextBox1.GetLineFromCharIndex(richTextBox1.GetCharIndexFromPosition(_currentMousePos));
                   var hoverHint = _highlights.Where(hlLine => hlLine.LineNumber == hoverLine).Aggregate("", (current, hlLine) => current + (hlLine.LineHint + "\n"));
                   if (hoverHint != "")
                   {
                       var hintLocationWithOffset = _currentMousePos;
                       hintLocationWithOffset.Offset(new Point(0, 20));
                       toolTip1.Show(hoverHint, richTextBox1, hintLocationWithOffset);
                   }
                   _hoverCount = 0;
               }
           }
           else
           {
               _lastMousePos = _currentMousePos;
           }

       }

       private void richTextBox1_MouseMove(object sender, MouseEventArgs e)
       {
           _currentMousePos = e.Location;
           if (_currentMousePos != _lastMousePos)
           {
               toolTip1.Hide(richTextBox1);
               _hoverCount = 0;
           }
       }


    }//class
}//namespace
