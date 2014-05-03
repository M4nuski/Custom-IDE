using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace ShaderIDE
{
    [System.ComponentModel.DesignerCategory(@"Code")]
    public partial class Form1 : Form
    {
        //Dialogs
        private readonly WordStyleDialog _styleDialogWords = new WordStyleDialog();
        private readonly DelimiterStyleDialog _styleDialogDelimiters = new DelimiterStyleDialog();
        private readonly SpanStyleDialog _styleDialogSpans = new SpanStyleDialog();

        private IWindowInfo _windowInfo;
        private IGraphicsContext _context;
        //public ColorFormat CF1 = new ColorFormat(8, 8, 8, 8);


        private ContextSetup ContextSetupData = new ContextSetup();


        #region Initialization and basic Form Events
        private void Msg(string msg)
        {
            textBox1.AppendText(msg + "\r\n");
        }

        public Form1()
        {
            InitializeComponent();
            Console.OnConsoleMessage += Msg;

            //editorBox1.Highlights.Add(new HighlightStruct(1, "Hint: don't talk too much in comments", Color.MidnightBlue)); //debug
            //editorBox1.Highlights.Add(new HighlightStruct(8, "Warning: blah blah blah2", Color.Goldenrod)); //debug
            //editorBox1.Highlights.Add(new HighlightStruct(38, "Warning: Color.GoldenRod is kinda weird", Color.Goldenrod)); //debug
            //editorBox1.Highlights.Add(new HighlightStruct(8, "ERROR: blah blah blah", Color.DarkRed)); //debug
            //editorBox1.Theme = ThemeHelper.DefaultGLSLDarkTheme(editorBox1.Font);
            //editorBox1.Theme = ThemeHelper.DefaultGLSLLightTheme(richTextBox1.Font);
            //editorBox1.Theme = ThemeHelper.DefaultCSDarkTheme(richTextBox1.Font);
            //editorBox1.Theme = ThemeHelper.DefaultCSBlueTheme(richTextBox1.Font);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            editorBox1.Theme = ThemeHelper.DefaultGLSLDarkTheme(editorBox1.Font);
            editorBox2.Theme = ThemeHelper.DefaultGLSLDarkTheme(editorBox1.Font);
            PopulateMenu();
            editorBox1.Size = tabPage1.Size;
            editorBox2.Size = tabPage2.Size;
            editorBox1.ForceRedraw(sender, e);
            editorBox2.ForceRedraw(sender, e);
            propertyGrid1.SelectedObject = ContextSetupData;
            button1_Click(this, new EventArgs());

        }
        #endregion

        #region Menu Strip Color and Style Settings
        private void MenuItem_FontClick(object sender, EventArgs e)
        {
            fontDialog1.Font = editorBox1.Theme.TextStyle.StyleFont;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                editorBox1.Theme = ThemeHelper.ChangeFont(editorBox1.Theme, fontDialog1.Font);
                editorBox1.Font = fontDialog1.Font;
                editorBox1.ForceRedraw(sender, e);
            }
        }

        private void MenuItem_SaveClick(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ThemeHelper.SaveTheme(editorBox1.Theme, saveFileDialog1.FileName);
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
                editorBox1.Theme = ThemeHelper.LoadTheme(editorBox1.Theme, openFileDialog1.FileName);
                PopulateMenu();
                editorBox1.ForceRedraw(sender, e);
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
            for (var i = 0; i < editorBox1.Theme.Delimiters.Length; i++)
            {
                var currentItem = MenuStrip_TokensDelimiter.DropDownItems.Add(new ToolStripButton(
                    editorBox1.Theme.Delimiters[i].Name));
                MenuStrip_TokensDelimiter.DropDownItems[currentItem].Name = "D" + i.ToString(CultureInfo.InvariantCulture);
                MenuStrip_TokensDelimiter.DropDownItems[currentItem].Click += MenuItem_TokensClick;
            }
            SetToLargest(MenuStrip_TokensDelimiter.DropDownItems);

            while (MenuStrip_TokensWords.DropDownItems.Count > 2) MenuStrip_TokensWords.DropDownItems.RemoveAt(2);
            for (var i = 0; i < editorBox1.Theme.Words.Length; i++)
            {
                var currentItem = MenuStrip_TokensWords.DropDownItems.Add(new ToolStripButton(
                    editorBox1.Theme.Words[i].Name));
                MenuStrip_TokensWords.DropDownItems[currentItem].Name = "W" + i.ToString(CultureInfo.InvariantCulture);
                MenuStrip_TokensWords.DropDownItems[currentItem].Click += MenuItem_TokensClick;
            }
            SetToLargest(MenuStrip_TokensWords.DropDownItems);

            while (MenuStrip_TokensSpans.DropDownItems.Count > 2) MenuStrip_TokensSpans.DropDownItems.RemoveAt(2);
            for (var i = 0; i < editorBox1.Theme.Spans.Length; i++)
            {
                var currentItem = MenuStrip_TokensSpans.DropDownItems.Add(new ToolStripButton(
                    editorBox1.Theme.Spans[i].Name));
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
                        if (_styleDialogWords.ShowDialog(editorBox1.Theme.ValueStyle, editorBox1.Theme.BackgroundColor) == DialogResult.OK)
                        {
                            var bufferTheme = editorBox1.Theme;
                            var microsoftsWeirdMarshalingWariningEliminator = _styleDialogWords.DialogOutput;
                            bufferTheme.ValueStyle = microsoftsWeirdMarshalingWariningEliminator.Style;
                            editorBox1.Theme = bufferTheme;
                        }
                    if (sendeMenu.Name[0] == 'D')
                    {
                        if (_styleDialogDelimiters.ShowDialog(editorBox1.Theme.Delimiters[currentIndex], editorBox1.Theme.BackgroundColor) == DialogResult.OK)
                        {
                            editorBox1.Theme.Delimiters[currentIndex] = _styleDialogDelimiters.DialogOutput;
                            MenuStrip_TokensDelimiter.DropDownItems[currentIndex + 2].Text = editorBox1.Theme.Delimiters[currentIndex].Name;
                            SetToLargest(MenuStrip_TokensDelimiter.DropDownItems);
                        }
                    }
                    if (sendeMenu.Name[0] == 'W')
                        if (_styleDialogWords.ShowDialog(editorBox1.Theme.Words[currentIndex], editorBox1.Theme.BackgroundColor) == DialogResult.OK)
                        {
                            editorBox1.Theme.Words[currentIndex] = _styleDialogWords.DialogOutput;
                            MenuStrip_TokensWords.DropDownItems[currentIndex + 2].Text = editorBox1.Theme.Words[currentIndex].Name;
                            SetToLargest(MenuStrip_TokensWords.DropDownItems);
                        }

                    if (sendeMenu.Name[0] == 'S')
                    {
                        if (_styleDialogSpans.ShowDialog(editorBox1.Theme.Spans[currentIndex], editorBox1.Theme.BackgroundColor) == DialogResult.OK)
                        {
                            editorBox1.Theme.Spans[currentIndex] = _styleDialogSpans.DialogOutput;
                            MenuStrip_TokensSpans.DropDownItems[currentIndex + 2].Text = editorBox1.Theme.Spans[currentIndex].Name;
                            SetToLargest(MenuStrip_TokensSpans.DropDownItems);
                        }
                    }

                } //tryparse
                editorBox1.ForceRedraw(sender, e);
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
                var bufferTheme = editorBox1.Theme;
                if (senderObject.Tag.ToString() == "TXT_COLOR") bufferTheme.TextStyle.StyleColor = GetColorDialogResult(editorBox1.Theme.TextStyle.StyleColor);
                if (senderObject.Tag.ToString() == "BG_COLOR") bufferTheme.BackgroundColor = GetColorDialogResult(editorBox1.Theme.BackgroundColor);
                if (senderObject.Tag.ToString() == "LINE_COLOR") bufferTheme.CurrentLineColor = GetColorDialogResult(editorBox1.Theme.CurrentLineColor);
                editorBox1.Theme = bufferTheme;
                editorBox1.ForceRedraw(sender, e);
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
                        new DelimiterStruct(editorBox1.Theme.TextStyle.StyleFont),
                        editorBox1.Theme.BackgroundColor) == DialogResult.OK)
                    {
                        var bufferTheme = editorBox1.Theme;
                        bufferTheme.Delimiters = AppendArray(editorBox1.Theme.Delimiters, _styleDialogDelimiters.DialogOutput);
                        editorBox1.Theme = bufferTheme;
                    }

                if (senderObject.Tag.ToString() == "WORD_NEW")
                    if (_styleDialogWords.ShowDialog(
                        new WordStruct(editorBox1.Theme.TextStyle.StyleFont),
                        editorBox1.Theme.BackgroundColor) == DialogResult.OK)
                    {
                        var bufferTheme = editorBox1.Theme;
                        bufferTheme.Words = AppendArray(editorBox1.Theme.Words, _styleDialogWords.DialogOutput);
                        editorBox1.Theme = bufferTheme;                       
                    }

                if (senderObject.Tag.ToString() == "SPAN_NEW")
                    if (_styleDialogSpans.ShowDialog(
                        new SpanStruct(editorBox1.Theme.TextStyle.StyleFont),
                        editorBox1.Theme.BackgroundColor) == DialogResult.OK)
                    {
                        var bufferTheme = editorBox1.Theme;
                        bufferTheme.Spans = AppendArray(editorBox1.Theme.Spans, _styleDialogSpans.DialogOutput);
                        editorBox1.Theme = bufferTheme;  
                    }

                PopulateMenu();
                editorBox1.ForceRedraw(sender, e);
            }
            else Text = @"Null Reference in NewTokensClick";
        }
        #endregion

        private void compileShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_context != null)
            {
                var shader = tabControl1.SelectedIndex;
                if (shader == 0)
                {
                    //compile vertex shader
                    Console.Message(@"Compiling Vertex Shader:");
                    _context.MakeCurrent(_windowInfo);
                    var currentShader = ShaderLoader.Load_Shader(editorBox1.Text, ShaderType.VertexShader);
                    if (currentShader == -1)
                    {
                        //find error line and highlight;
                    }
                    else
                    {
                        Console.Message(@"Done.");
                    }
                }
                else if (shader == 1)
                {
                    //compile fragment shader
                    Console.Message(@"Compiling Fragment Shader:");
                    _context.MakeCurrent(_windowInfo);
                    var currentShader = ShaderLoader.Load_Shader(editorBox2.Text, ShaderType.FragmentShader);
                    if (currentShader == -1)
                    {
                        //find error line and highlight;
                    }
                    else
                    {
                        Console.Message(@"Done.");
                    }
                }
            }
            else
            {
                Console.Message(@"Context not set.");
            }
        }

        private void buildProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //compile all shader and try linking into program
            Console.Message(@"Building Program:");
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

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

        private void button1_Click(object sender, EventArgs e)
        {

            Console.Message(@"Reseting Context");
            _windowInfo = Utilities.CreateWindowsWindowInfo(tabPage4.Handle);
            var colorFormat = new ColorFormat(ContextSetupData.Red, ContextSetupData.Green, ContextSetupData.Blue, ContextSetupData.Alpha);
            var accumFormat = new ColorFormat(ContextSetupData.AccumRed, ContextSetupData.AccumGreen, ContextSetupData.AccumBlue, ContextSetupData.AccumAlpha);

            var mode = new GraphicsMode(colorFormat, ContextSetupData.Depth, ContextSetupData.Stencil, ContextSetupData.Samples, accumFormat, ContextSetupData.Buffers, ContextSetupData.Stereo);
            if (_context != null) _context.Dispose();
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






    }//class
}//namespace
