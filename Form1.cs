using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;

namespace ShaderIDE
{
    public partial class Form1 : Form
    {
        #region Properties

        private ThemeStruct Editors_Theme;

        #region Dialogs
        private readonly WordStyleDialog _styleDialogWords = new WordStyleDialog();
        private readonly DelimiterStyleDialog _styleDialogDelimiters = new DelimiterStyleDialog();
        private readonly SpanStyleDialog _styleDialogSpans = new SpanStyleDialog();
        #endregion

        #region Context
        private IWindowInfo _windowInfo;
        private IGraphicsContext _context;
        private readonly ContextSetup ContextSetupData = new ContextSetup();
        #endregion

        #region Build Results
        private int _lastProgram = -1;
        private int _lastVextexShader = -1;
        private int _lastFragmentShader = -1;
        private string _lastError = "";
        private Color _ErrorColor = Color.Red;
        private Color _HintColor = Color.DarkOliveGreen;
        #endregion

        #region Uniform and Data             
        private List<IUniformProperty> UniformData;
        #endregion

        #endregion

        #region Initialization and basic Form Events
        private void Msg(string msg)
        {
            //TODO use a freaking string builder / the Logger component
            textBox1.AppendText(msg + "\r\n");
            if (msg.SafeRemove(6) == "ERROR:")
            {
                _lastError = msg.Substring(6);
            }
        }

        public Form1()
        {
            InitializeComponent();
            Console.OnConsoleMessage += Msg;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Editors_Theme = ThemeHelper.DefaultGLSLDarkTheme(editorBox1.Font);
            editorBox1.Theme = Editors_Theme;
            editorBox2.Theme = Editors_Theme;
            PopulateMenu();
            editorBox1.Size = tabPage1.Size;
            editorBox2.Size = tabPage2.Size;
            editorBox1.ForceRedraw(sender, e);
            editorBox2.ForceRedraw(sender, e);
            ContextPropertyGrid.SelectedObject = ContextSetupData;
            
            button1_Click(this, new EventArgs());
            UniformData = UniformPropertyTester.BuildDefaultList(UniformPropertyGrid, UniformMatrixControl);
            PopulateUniformList();
           
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Menu Strip Color and Style Settings
        private void MenuItem_FontClick(object sender, EventArgs e)
        {
            fontDialog1.Font = Editors_Theme.TextStyle.StyleFont;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Editors_Theme = ThemeHelper.ChangeFont(Editors_Theme, fontDialog1.Font);
                editorBox1.Font = fontDialog1.Font;
                editorBox1.ForceRedraw(sender, e);
                editorBox2.Font = fontDialog1.Font;
                editorBox2.ForceRedraw(sender, e);
            }
        }

        private void MenuItem_SaveClick(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ThemeHelper.SaveTheme(Editors_Theme, saveFileDialog1.FileName);
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
                Editors_Theme = ThemeHelper.LoadTheme(Editors_Theme, openFileDialog1.FileName);
                PopulateMenu();
                editorBox1.ForceRedraw(sender, e);
                editorBox2.ForceRedraw(sender, e);
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
            for (var i = 0; i < Editors_Theme.Delimiters.Length; i++)
            {
                var currentItem = MenuStrip_TokensDelimiter.DropDownItems.Add(new ToolStripButton(
                    Editors_Theme.Delimiters[i].Name));
                MenuStrip_TokensDelimiter.DropDownItems[currentItem].Name = "D" + i.ToString(CultureInfo.InvariantCulture);
                MenuStrip_TokensDelimiter.DropDownItems[currentItem].Click += MenuItem_TokensClick;
            }
            SetToLargest(MenuStrip_TokensDelimiter.DropDownItems);

            while (MenuStrip_TokensWords.DropDownItems.Count > 2) MenuStrip_TokensWords.DropDownItems.RemoveAt(2);
            for (var i = 0; i < Editors_Theme.Words.Length; i++)
            {
                var currentItem = MenuStrip_TokensWords.DropDownItems.Add(new ToolStripButton(
                    Editors_Theme.Words[i].Name));
                MenuStrip_TokensWords.DropDownItems[currentItem].Name = "W" + i.ToString(CultureInfo.InvariantCulture);
                MenuStrip_TokensWords.DropDownItems[currentItem].Click += MenuItem_TokensClick;
            }
            SetToLargest(MenuStrip_TokensWords.DropDownItems);

            while (MenuStrip_TokensSpans.DropDownItems.Count > 2) MenuStrip_TokensSpans.DropDownItems.RemoveAt(2);
            for (var i = 0; i < Editors_Theme.Spans.Length; i++)
            {
                var currentItem = MenuStrip_TokensSpans.DropDownItems.Add(new ToolStripButton(
                    Editors_Theme.Spans[i].Name));
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
                        if (_styleDialogWords.ShowDialog(Editors_Theme.ValueStyle, Editors_Theme.BackgroundColor) == DialogResult.OK)
                        {
                            var bufferTheme = Editors_Theme;
                            var microsoftsWeirdMarshalingWariningEliminator = _styleDialogWords.DialogOutput;
                            bufferTheme.ValueStyle = microsoftsWeirdMarshalingWariningEliminator.Style;
                            Editors_Theme = bufferTheme;
                        }
                    if (sendeMenu.Name[0] == 'D')
                    {
                        if (_styleDialogDelimiters.ShowDialog(Editors_Theme.Delimiters[currentIndex], Editors_Theme.BackgroundColor) == DialogResult.OK)
                        {
                            Editors_Theme.Delimiters[currentIndex] = _styleDialogDelimiters.DialogOutput;
                            MenuStrip_TokensDelimiter.DropDownItems[currentIndex + 2].Text = Editors_Theme.Delimiters[currentIndex].Name;
                            SetToLargest(MenuStrip_TokensDelimiter.DropDownItems);
                        }
                    }
                    if (sendeMenu.Name[0] == 'W')
                        if (_styleDialogWords.ShowDialog(Editors_Theme.Words[currentIndex], Editors_Theme.BackgroundColor) == DialogResult.OK)
                        {
                            Editors_Theme.Words[currentIndex] = _styleDialogWords.DialogOutput;
                            MenuStrip_TokensWords.DropDownItems[currentIndex + 2].Text = Editors_Theme.Words[currentIndex].Name;
                            SetToLargest(MenuStrip_TokensWords.DropDownItems);
                        }

                    if (sendeMenu.Name[0] == 'S')
                    {
                        if (_styleDialogSpans.ShowDialog(Editors_Theme.Spans[currentIndex], Editors_Theme.BackgroundColor) == DialogResult.OK)
                        {
                            Editors_Theme.Spans[currentIndex] = _styleDialogSpans.DialogOutput;
                            MenuStrip_TokensSpans.DropDownItems[currentIndex + 2].Text = Editors_Theme.Spans[currentIndex].Name;
                            SetToLargest(MenuStrip_TokensSpans.DropDownItems);
                        }
                    }

                } //tryparse
                editorBox1.ForceRedraw(sender, e);
                editorBox2.ForceRedraw(sender, e);
            }//not null
            else Text = @"Null Reference in TokensClick";
        }

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
                var bufferTheme = Editors_Theme;
                if (senderObject.Tag.ToString() == "TXT_COLOR") bufferTheme.TextStyle.StyleColor = GetColorDialogResult(Editors_Theme.TextStyle.StyleColor);
                if (senderObject.Tag.ToString() == "BG_COLOR") bufferTheme.BackgroundColor = GetColorDialogResult(Editors_Theme.BackgroundColor);
                if (senderObject.Tag.ToString() == "LINE_COLOR") bufferTheme.CurrentLineColor = GetColorDialogResult(Editors_Theme.CurrentLineColor);
                Editors_Theme = bufferTheme;
                editorBox1.ForceRedraw(sender, e);
                editorBox2.ForceRedraw(sender, e);
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
                        new DelimiterStruct(Editors_Theme.TextStyle.StyleFont),
                        Editors_Theme.BackgroundColor) == DialogResult.OK)
                    {
                        var bufferTheme = Editors_Theme;
                        bufferTheme.Delimiters = AppendArray(Editors_Theme.Delimiters, _styleDialogDelimiters.DialogOutput);
                        Editors_Theme = bufferTheme;
                    }

                if (senderObject.Tag.ToString() == "WORD_NEW")
                    if (_styleDialogWords.ShowDialog(
                        new WordStruct(Editors_Theme.TextStyle.StyleFont),
                        Editors_Theme.BackgroundColor) == DialogResult.OK)
                    {
                        var bufferTheme = Editors_Theme;
                        bufferTheme.Words = AppendArray(Editors_Theme.Words, _styleDialogWords.DialogOutput);
                        Editors_Theme = bufferTheme;
                    }

                if (senderObject.Tag.ToString() == "SPAN_NEW")
                    if (_styleDialogSpans.ShowDialog(
                        new SpanStruct(Editors_Theme.TextStyle.StyleFont),
                        Editors_Theme.BackgroundColor) == DialogResult.OK)
                    {
                        var bufferTheme = Editors_Theme;
                        bufferTheme.Spans = AppendArray(Editors_Theme.Spans, _styleDialogSpans.DialogOutput);
                        Editors_Theme = bufferTheme;
                    }

                PopulateMenu();
                editorBox1.ForceRedraw(sender, e);
                editorBox2.ForceRedraw(sender, e);
            }
            else Text = @"Null Reference in NewTokensClick";
        }
        #endregion

        #region Uniforms
        private int BindAndMsg(string uniformName, int _program)
        {
            return GL.GetUniformLocation(_program, uniformName);
        }

        private void FindAndBind(EditorBox editor, int _program)
        {
            for (var i = 0; i < editor.Lines.Length; i++)
            {
                var currentUniform = editor.Lines[i];
                currentUniform = currentUniform.Trim(new[] { ' ', '\t', '\n', '\r', ';' });
                if (currentUniform.StartsWith("uniform"))
                {
                    var uniformStart = currentUniform.LastIndexOf(' ') + 1;

                    currentUniform = currentUniform.Substring(uniformStart, currentUniform.Length - uniformStart);
                    var uniInt = BindAndMsg(currentUniform, _program);
                    if (uniInt == -1)
                    {
                        editor.Highlights.Add(new HighlightStruct
                        {
                            LineColor = _HintColor,
                            LineHint = "Hint: Uniform not used in program.",
                            LineNumber = i
                        });
                    }
                    else
                    {
                        UniformListBox.Items.Add(currentUniform + ":" + uniInt.ToString("D"));
                    }
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((_lastProgram > -1) & (_lastVextexShader > -1) & (_lastFragmentShader > -1))
            {
                for (var i = editorBox1.Highlights.Count -1; i >=0 ; i--)
                {
                    if (editorBox1.Highlights[i].LineColor == _HintColor) editorBox1.Highlights.RemoveAt(i);
                }
                for (var i = editorBox2.Highlights.Count - 1; i >= 0; i--)
                {
                    if (editorBox2.Highlights[i].LineColor == _HintColor) editorBox2.Highlights.RemoveAt(i);
                }
                GL.UseProgram(_lastProgram);
                //TODO remove that shit and stop rebuilding the list every 2 seconds.
                UniformListBox.Items.Clear();
                FindAndBind(editorBox1, _lastProgram);
                FindAndBind(editorBox2, _lastProgram);
                //keep all last properties
                //find new set
                //match selected properties if still exists
                //if not deselect matrixcontrol and grid
            }
        }
        #endregion

        #region Build and Link
        private int compileShader(EditorBox editor, ShaderType type)
        {
            var currentShader = ShaderLoader.Load_Shader(editor.Text, type);
            editor.Highlights.Clear();
            if (currentShader == -1)
            {
                var errorList = _lastError.Split('\n');
                foreach (var currentError in errorList)
                {
                    var startIndex = currentError.IndexOf("(", StringComparison.Ordinal) + 1;
                    var lineStart = currentError.Substring(startIndex);
                    var stopIndex = lineStart.IndexOf(")", StringComparison.Ordinal);

                    int errorLine;
                    var errorPos = lineStart.SafeRemove(stopIndex);
                    if (errorPos != "")
                    {
                        int.TryParse(errorPos, out errorLine);
                        editor.Highlights.Add(new HighlightStruct
                        {
                            LineColor = _ErrorColor,
                            LineHint = currentError.Substring(currentError.IndexOf(":", StringComparison.Ordinal) + 1),
                            LineNumber = errorLine - 1
                        });
                    }
                }
            }
            return currentShader;
        }

        private void compileShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_context != null)
            {
                timer1.Stop();
                textBox1.Clear();
                _context.MakeCurrent(_windowInfo);

                var shader = tabControl1.SelectedIndex;

                if (shader == 0)
                {
                    //compile vertex shader
                    Console.Message(@"Compiling Vertex Shader:");
                    var _currentShaderResult = compileShader(editorBox1, ShaderType.VertexShader);
                    if (_currentShaderResult > -1)
                    {
                        Console.Message(@"Done.");
                        _lastVextexShader = _currentShaderResult;
                    }
                    else
                    {
                        editorBox1.ForceRedraw(this, new EventArgs());
                    }
                }

                else if (shader == 1)
                {
                    //compile fragment shader
                    Console.Message(@"Compiling Fragment Shader:");
                    var _currentShaderResult = compileShader(editorBox2, ShaderType.FragmentShader);
                    if (_currentShaderResult > -1)
                    {
                        Console.Message(@"Done.");
                        _lastFragmentShader = _currentShaderResult;
                    }
                    else
                    {
                        editorBox2.ForceRedraw(this, new EventArgs());
                    }
                }
                timer1.Start();
            }
            else
            {
                Console.Message(@"Context not set.");
            }
        }

        private void buildProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //compile all shader and try linking into program
            if (_context != null)
            {
                textBox1.Clear();
                _context.MakeCurrent(_windowInfo);
                Console.Message(@"Building Program:");
                var shaderList = new List<int>();

                //compile vertex shader
                Console.Message(@"Compiling Vertex Shader:");
                _lastVextexShader = compileShader(editorBox1, ShaderType.VertexShader);
                if (_lastVextexShader > -1)
                {
                    shaderList.Add(_lastVextexShader);
                }

                //compile fragment shader
                Console.Message(@"Compiling Fragment Shader:");
                _lastFragmentShader = compileShader(editorBox2, ShaderType.FragmentShader);
                if (_lastFragmentShader > -1)
                {
                    shaderList.Add(_lastFragmentShader);
                }

                //linking program
                _lastProgram = ShaderLoader.Link_Program(shaderList);
                if ((_lastProgram > -1) & (shaderList.Count == 2))
                {
                    GL.UseProgram(_lastProgram);
                    Console.Message(@"Done with " + shaderList.Count + " Shaders.");
                    UniformListBox.Items.Clear();
                    FindAndBind(editorBox1, _lastProgram);
                    FindAndBind(editorBox2, _lastProgram);

                }
                else
                {
                    Console.Message(@"Program Link Failed.");
                }
                editorBox1.ForceRedraw(this, new EventArgs());
                editorBox2.ForceRedraw(this, new EventArgs());
            }
        }
        #endregion

        #region Helpers
        private GraphicsContextFlags BoolToGraphContextFlags()
        {
            var result = GraphicsContextFlags.Default;
            if (ContextSetupData.DebugFlag) result |= GraphicsContextFlags.Debug;
            if (ContextSetupData.EmbeddedFlag) result |= GraphicsContextFlags.Embedded;
            if (ContextSetupData.ForwardCompatibleFlag) result |= GraphicsContextFlags.ForwardCompatible;
            return result;
        }

        private ClearBufferMask BoolToClearMask()
        {
            var result = ClearBufferMask.None;
            if (ContextSetupData.ClearColorBuffer) result |= ClearBufferMask.ColorBufferBit;
            if (ContextSetupData.ClearDepthBuffer) result |= ClearBufferMask.DepthBufferBit;
            return result;
        }
        #endregion

        #region Context
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            Console.Message(@"Reseting Context");
            _windowInfo = Utilities.CreateWindowsWindowInfo(panel1.Handle);
            var colorFormat = new ColorFormat(ContextSetupData.Red, ContextSetupData.Green, ContextSetupData.Blue, ContextSetupData.Alpha);
            var accumFormat = new ColorFormat(ContextSetupData.AccumRed, ContextSetupData.AccumGreen, ContextSetupData.AccumBlue, ContextSetupData.AccumAlpha);

            var mode = new GraphicsMode(colorFormat, ContextSetupData.Depth, ContextSetupData.Stencil, ContextSetupData.Samples, accumFormat, ContextSetupData.Buffers, ContextSetupData.Stereo);
            _context?.Dispose();
            _context = new GraphicsContext(mode, _windowInfo, ContextSetupData.MajorVersion, ContextSetupData.MinorVersion, BoolToGraphContextFlags());
            _context.MakeCurrent(_windowInfo);
            (_context as IGraphicsContextInternal).LoadAll();

            Console.Message(@"Setting OpenGL State");
            GL.PolygonMode(ContextSetupData.MaterialFace, ContextSetupData.PolygonMode);
            if (ContextSetupData.CullFace) { GL.Enable(EnableCap.CullFace); } else { GL.Disable(EnableCap.CullFace); }
            if (ContextSetupData.DepthTest) { GL.Enable(EnableCap.DepthTest); } else { GL.Disable(EnableCap.DepthTest); }

            GL.ClearColor(ContextSetupData.ClearColor);
            GL.Clear(ClearBufferMask.AccumBufferBit);
            GL.Clear(BoolToClearMask());
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
            if (_context != null)
            {
                _context.MakeCurrent(_windowInfo);
                GL.ClearColor(ContextSetupData.ClearColor);
                GL.Clear(BoolToClearMask());
                _context.SwapBuffers();
            }
        }
        #endregion

        #region Uniforms And Data

        private void PopulateUniformList()
        {
            PropertiesListBox.Items.Clear();
            foreach (var uniformProperty in UniformData)
            {
                PropertiesListBox.Items.Add(uniformProperty.Name);
            }
        }

        private void PropertiesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            UniformMatrixControl.Enabled = false;
            UniformPropertyGrid.Enabled = false;
            UniformData[PropertiesListBox.SelectedIndex].EditProperty();
        }
        #endregion
        private void UniformListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO Select PropertiesListBox index to match assigned value
        }

        private void UniformPropertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            UniformMatrixControl.Enabled = false;
        }

        private void UniformMatrixControl_EnabledChanged(object sender, EventArgs e)
        {
            if (UniformMatrixControl.Enabled) UniformPropertyGrid.Enabled = false;
        }
    }//class
}//namespace
