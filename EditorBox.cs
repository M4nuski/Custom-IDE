﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ShaderIDE
{
    sealed class EditorBox : RichTextBox
    {
        #region Lists
        public List<TokenStruct> TokenList;
        private List<TokenStruct> _lastTokenList;
        public List<HighlightStruct> Highlights = new List<HighlightStruct>();
        #endregion

        #region Properties

        #region Tooptip
        private Point _lastMousePos, _currentMousePos;
        private int _hoverCount;
        private readonly ToolTip _hoverToolTip = new ToolTip();
        private readonly Timer _hoverTimer = new Timer();
        #endregion

        #region Parser and Editor controls
        public ThemeStruct Theme { get; set; }
        private bool _inParser;
        private Point _textboxBottomLeftPoint;
        private int _lineSelectionStart, _lineSelectionLength;
        private int _lastSelectionStart, _lastSelectionLength;
        private int _lastNumLines;
        #endregion

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
            if (Theme.Delimiters != null)
            {
                foreach (
                    var delimiterStruct in
                        Theme.Delimiters.Where(delimiterStruct => delimiterStruct.Keychars.Contains(c)))
                {
                    return delimiterStruct.Style;
                }
            }
            return Theme.TextStyle;
        }

        private FontAndColorStruct GetWordFont(string s)
        {
            if (Theme.Words != null)
            {
                foreach (var wordStruct in Theme.Words.Where(wordStruct => wordStruct.Keywords.Contains(s)))
                {
                    return wordStruct.Style;
                }
            }
            return IsValue(s) ? Theme.ValueStyle : Theme.TextStyle;
        }

        private List<TokenStruct> TokenizeLines(string[] lines)
        {
            var textOffset = 0;
            var result = new List<TokenStruct>();

            foreach (var line in lines)
            {
                if (Theme.Spans != null)
                {
                    result.AddRange(Theme.Spans.Any(currentSpan => line.Contains(currentSpan.StartKeyword))
                        ? TokenizeLinePerSpan(line, textOffset)
                        : TokenizeLinePerDelimiter(line, textOffset));
                }
                else
                {
                    result.AddRange(TokenizeLinePerDelimiter(line, textOffset));
                }
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
            if (Theme.Delimiters != null)
            {
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
                if (linePointer < line.Length)
                    result.Add(new TokenStruct
                    {
                        Offset = linePointer + offset,
                        Text = line.Substring(linePointer, line.Length - linePointer),
                        Style = GetWordFont(line.Substring(linePointer, line.Length - linePointer))
                    });
            }
            else
            {
                result.Add(new TokenStruct
                {
                    Offset = offset,
                    Text = line,
                    Style = Theme.TextStyle
                });
            }
            return result;
        }
        #endregion

        #region Tooltip Handling
        private void hoverTimer_Tick(object sender, EventArgs e)
        {
            if (_currentMousePos == _lastMousePos)
            {
                _hoverCount++;
                if ((_hoverCount > 3) & (_hoverToolTip.Active))
                {
                    var hoverLine = GetLineFromCharIndex(GetCharIndexFromPosition(_currentMousePos));
                    var hoverHint = Highlights.Where(hlLine => hlLine.LineNumber == hoverLine)
                        .Aggregate("", (current, hlLine) => current + (hlLine.LineHint + "\n"));
                    if (hoverHint != "")
                    {
                        var hintLocationWithOffset = _currentMousePos;
                        hintLocationWithOffset.Offset(new Point(0, 20));
                        _hoverToolTip.Show(hoverHint, this, hintLocationWithOffset);
                    }
                    _hoverCount = 0;
                }
            }
            else
            {
                _lastMousePos = _currentMousePos;
            }
        }

        private void EditorBox_MouseMove(object sender, MouseEventArgs e)
        {
            _currentMousePos = e.Location;
            if ((_currentMousePos != _lastMousePos) & (_hoverToolTip != null))
            {
                _hoverToolTip.Hide(this);
                _hoverCount = 0;
            }
        }
        #endregion

        #region Form Events
        public EditorBox()
        {
            _lastTokenList = new List<TokenStruct>();
            _inParser = false;

            TextChanged += EditorBox_TextChanged;
            Resize += EditorBox_Resize;
            Click += EditorBox_Click;
            MouseMove += EditorBox_MouseMove;
            ForeColorChanged += OnForeColorChanged;
            Disposed += OnDisposed;

            ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            WordWrap = false;

            Theme = new ThemeStruct(Font, ForeColor, BackColor);

            _hoverToolTip.Active = true;
            _hoverTimer.Tick += hoverTimer_Tick;
            _hoverTimer.Interval = 100;
            _hoverTimer.Enabled = true;

        }

        private void OnDisposed(object sender, EventArgs eventArgs)
        {
            if (_hoverTimer != null)
            {
                _hoverTimer.Enabled = false;
                _hoverTimer.Tick -= hoverTimer_Tick;
            }
        }

        private void OnForeColorChanged(object sender, EventArgs eventArgs)
        {
            if (Theme.Name == "<Default>")
            {
                Theme = new ThemeStruct(Font, ForeColor, BackColor);
            }
        }

        private void EditorBox_Resize(object sender, EventArgs e)
        {
            _textboxBottomLeftPoint = new Point(ClientSize) { Y = 1 };
        }

        private void EditorBox_Click(object sender, EventArgs e) //Redraw selections and background
        {

            if (!_inParser & (TokenList != null))
            {
                if (_lastNumLines != Lines.Length)
                {
                    Highlights.Clear();
                    _lastNumLines = Lines.Length;
                }
                _inParser = true;
                var dummyArray = new bool[TokenList.Count];
                if ((_lastSelectionStart != SelectionStart) |
                    (_lastSelectionLength != SelectionLength))
                {
                    var startOffset = GetFirstCharIndexFromLine(GetLineFromCharIndex(_lastSelectionStart));

                    for (var i = 0; i < dummyArray.Length; i++)
                    {
                        dummyArray[i] = (TokenList[i].Offset + 1 >= startOffset) &
                             (TokenList[i].Offset - 1 <= _lastSelectionStart + _lastSelectionLength);
                    }
                    _lastSelectionStart = SelectionStart;
                    _lastSelectionLength = SelectionLength;
                }
                ReDraw(dummyArray);
                _inParser = false;
            }
        }
        #endregion

        #region Editor Update
        public void ForceRedraw(object sender, EventArgs e)
        {
            _lastTokenList = new List<TokenStruct>(); //Clear old list
            _inParser = false;
            EditorBox_TextChanged(sender, e);
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
            if ((Lines.Length > 0) & (lineNumber > -1))
            {
                var maxLine = Lines.Length - 1;
                if (lineNumber < maxLine)
                {
                    _lineSelectionStart = GetFirstCharIndexFromLine(lineNumber);
                    _lineSelectionLength = GetFirstCharIndexFromLine(lineNumber + 1) - _lineSelectionStart;
                }
                else
                {
                    _lineSelectionStart = GetFirstCharIndexFromLine(maxLine);
                    _lineSelectionLength = TextLength - 1 - _lineSelectionStart;
                }
            }
            else
            {
                _lineSelectionStart = 0;
                _lineSelectionLength = 0;
            }
        }

        private Color MixColors(Color A, Color B, float factor)
        {
            factor = (factor > 1.0f) ? 1.0f : (factor < 0) ? 0 : factor;

            var invFactor = 1.0f - factor;
            var aa = (A.A * invFactor) + (B.A * factor);
            var rr = (A.R * invFactor) + (B.R * factor);
            var gg = (A.G * invFactor) + (B.G * factor);
            var bb = (A.B * invFactor) + (B.B * factor);
            return Color.FromArgb((int)(aa), (int)(rr), (int)(gg), (int)(bb));
        }

        private void ReDraw(bool[] changedList)
        {
            SuspendUpdate.Suspend(this);

            // Save current state
            var carretPositionBuffer = SelectionStart;
            var carretLengthBuffer = SelectionLength;
            var boxOrigin = GetCharIndexFromPosition(_textboxBottomLeftPoint);

            // Get Current Line informations
            GetSelectionFromLine(GetLineFromCharIndex(GetFirstCharIndexOfCurrentLine()));
            var currentLineIndex = _lineSelectionStart;
            var currentLineLength = _lineSelectionLength;

            // Reset Background
            BackColor = Theme.BackgroundColor;
            SelectAll();
            SelectionBackColor = Theme.BackgroundColor;

            // Update Colors
            for (var i = 0; i < TokenList.Count; i++)
                if (changedList[i])
                {
                    Select(TokenList[i].Offset, TokenList[i].Text.Length);
                    SelectionColor = TokenList[i].Style.StyleColor;
                    SelectionFont = TokenList[i].Style.StyleFont;
                }

            // Current line Color
            Select(currentLineIndex, currentLineLength);
            SelectionBackColor = Theme.CurrentLineColor;

            // Line highlights Color
            foreach (var currentHighlight in Highlights)
            {
                GetSelectionFromLine(currentHighlight.LineNumber);
                Select(_lineSelectionStart, _lineSelectionLength);
                SelectionBackColor = currentLineIndex != _lineSelectionStart ? currentHighlight.LineColor : MixColors(Theme.CurrentLineColor, currentHighlight.LineColor, 0.3f);
            }

            // Reset View
            Select(boxOrigin, 0);
            ScrollToCaret();

            // Reset Selections
            Select(carretPositionBuffer, carretLengthBuffer);
            SelectionColor = Theme.TextStyle.StyleColor;
            SelectionFont = Theme.TextStyle.StyleFont;

            SuspendUpdate.Resume(this);
        }

        private void EditorBox_TextChanged(object sender, EventArgs e)
        {
            if (!_inParser)
            {
                if (_lastNumLines != Lines.Length)
                {
                    Highlights.Clear();
                    _lastNumLines = Lines.Length;
                }
                _inParser = true;
                TokenList = TokenizeLines(Lines);
                ReDraw(CheckForChanges());
                _inParser = false;
            }
        }
        #endregion
    }
}
