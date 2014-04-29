﻿namespace ShaderIDE
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuStrip_TopTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ThemeLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ThemeSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ThemeFont = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ThemeColors = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ColorsText = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ColorsBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ColorsCurrentline = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_ThemeTokens = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_TokensDelimiter = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_NewDelimiter = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_SeparatorDelimiters = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_TokensWords = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_NewWord = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_SeparatorWords = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_TokensSpans = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_NewSpan = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_SeparatorSpans = new System.Windows.Forms.ToolStripSeparator();
            this.V0 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_TopForceParse = new System.Windows.Forms.ToolStripMenuItem();
            this.compileShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.editorBox1 = new ShaderIDE.EditorBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.editorBox2 = new ShaderIDE.EditorBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.MenuStrip_TopFile = new System.Windows.Forms.ToolStripMenuItem();
            this.loadShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveShaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.FullOpen = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_TopFile,
            this.MenuStrip_TopTheme,
            this.MenuStrip_TopForceParse,
            this.compileShaderToolStripMenuItem,
            this.buildProgramToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(883, 28);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuStrip_TopTheme
            // 
            this.MenuStrip_TopTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_TopTheme.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_ThemeLoad,
            this.MenuStrip_ThemeSave,
            this.MenuStrip_ThemeFont,
            this.MenuStrip_ThemeColors,
            this.MenuStrip_ThemeTokens});
            this.MenuStrip_TopTheme.Name = "MenuStrip_TopTheme";
            this.MenuStrip_TopTheme.Size = new System.Drawing.Size(66, 24);
            this.MenuStrip_TopTheme.Text = "Theme";
            // 
            // MenuStrip_ThemeLoad
            // 
            this.MenuStrip_ThemeLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_ThemeLoad.Name = "MenuStrip_ThemeLoad";
            this.MenuStrip_ThemeLoad.Size = new System.Drawing.Size(152, 24);
            this.MenuStrip_ThemeLoad.Text = "Load...";
            this.MenuStrip_ThemeLoad.Click += new System.EventHandler(this.MenuItem_LoadClick);
            // 
            // MenuStrip_ThemeSave
            // 
            this.MenuStrip_ThemeSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_ThemeSave.Name = "MenuStrip_ThemeSave";
            this.MenuStrip_ThemeSave.Size = new System.Drawing.Size(152, 24);
            this.MenuStrip_ThemeSave.Text = "Save...";
            this.MenuStrip_ThemeSave.Click += new System.EventHandler(this.MenuItem_SaveClick);
            // 
            // MenuStrip_ThemeFont
            // 
            this.MenuStrip_ThemeFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_ThemeFont.Name = "MenuStrip_ThemeFont";
            this.MenuStrip_ThemeFont.Size = new System.Drawing.Size(152, 24);
            this.MenuStrip_ThemeFont.Text = "Font";
            this.MenuStrip_ThemeFont.Click += new System.EventHandler(this.MenuItem_FontClick);
            // 
            // MenuStrip_ThemeColors
            // 
            this.MenuStrip_ThemeColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_ThemeColors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_ColorsText,
            this.MenuStrip_ColorsBackground,
            this.MenuStrip_ColorsCurrentline});
            this.MenuStrip_ThemeColors.Name = "MenuStrip_ThemeColors";
            this.MenuStrip_ThemeColors.Size = new System.Drawing.Size(152, 24);
            this.MenuStrip_ThemeColors.Text = "Colors";
            // 
            // MenuStrip_ColorsText
            // 
            this.MenuStrip_ColorsText.Name = "MenuStrip_ColorsText";
            this.MenuStrip_ColorsText.Size = new System.Drawing.Size(240, 24);
            this.MenuStrip_ColorsText.Tag = "TXT_COLOR";
            this.MenuStrip_ColorsText.Text = "Text";
            this.MenuStrip_ColorsText.Click += new System.EventHandler(this.MenuItem_ColorsClick);
            // 
            // MenuStrip_ColorsBackground
            // 
            this.MenuStrip_ColorsBackground.Name = "MenuStrip_ColorsBackground";
            this.MenuStrip_ColorsBackground.Size = new System.Drawing.Size(240, 24);
            this.MenuStrip_ColorsBackground.Tag = "BG_COLOR";
            this.MenuStrip_ColorsBackground.Text = "Background";
            this.MenuStrip_ColorsBackground.Click += new System.EventHandler(this.MenuItem_ColorsClick);
            // 
            // MenuStrip_ColorsCurrentline
            // 
            this.MenuStrip_ColorsCurrentline.Name = "MenuStrip_ColorsCurrentline";
            this.MenuStrip_ColorsCurrentline.Size = new System.Drawing.Size(240, 24);
            this.MenuStrip_ColorsCurrentline.Tag = "LINE_COLOR";
            this.MenuStrip_ColorsCurrentline.Text = "Current Line Background";
            this.MenuStrip_ColorsCurrentline.Click += new System.EventHandler(this.MenuItem_ColorsClick);
            // 
            // MenuStrip_ThemeTokens
            // 
            this.MenuStrip_ThemeTokens.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_ThemeTokens.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_TokensDelimiter,
            this.MenuStrip_TokensWords,
            this.MenuStrip_TokensSpans,
            this.V0});
            this.MenuStrip_ThemeTokens.Name = "MenuStrip_ThemeTokens";
            this.MenuStrip_ThemeTokens.Size = new System.Drawing.Size(152, 24);
            this.MenuStrip_ThemeTokens.Text = "Tokens";
            // 
            // MenuStrip_TokensDelimiter
            // 
            this.MenuStrip_TokensDelimiter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_TokensDelimiter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_NewDelimiter,
            this.MenuStrip_SeparatorDelimiters});
            this.MenuStrip_TokensDelimiter.Name = "MenuStrip_TokensDelimiter";
            this.MenuStrip_TokensDelimiter.Size = new System.Drawing.Size(146, 24);
            this.MenuStrip_TokensDelimiter.Text = "Delimiters";
            // 
            // MenuStrip_NewDelimiter
            // 
            this.MenuStrip_NewDelimiter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_NewDelimiter.Name = "MenuStrip_NewDelimiter";
            this.MenuStrip_NewDelimiter.Size = new System.Drawing.Size(108, 24);
            this.MenuStrip_NewDelimiter.Tag = "DEL_NEW";
            this.MenuStrip_NewDelimiter.Text = "New";
            this.MenuStrip_NewDelimiter.Click += new System.EventHandler(this.MenuItem_Tokens_NewClick);
            // 
            // MenuStrip_SeparatorDelimiters
            // 
            this.MenuStrip_SeparatorDelimiters.Name = "MenuStrip_SeparatorDelimiters";
            this.MenuStrip_SeparatorDelimiters.Size = new System.Drawing.Size(105, 6);
            // 
            // MenuStrip_TokensWords
            // 
            this.MenuStrip_TokensWords.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_TokensWords.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_NewWord,
            this.MenuStrip_SeparatorWords});
            this.MenuStrip_TokensWords.Name = "MenuStrip_TokensWords";
            this.MenuStrip_TokensWords.Size = new System.Drawing.Size(146, 24);
            this.MenuStrip_TokensWords.Text = "Words";
            // 
            // MenuStrip_NewWord
            // 
            this.MenuStrip_NewWord.Name = "MenuStrip_NewWord";
            this.MenuStrip_NewWord.Size = new System.Drawing.Size(108, 24);
            this.MenuStrip_NewWord.Tag = "WORD_NEW";
            this.MenuStrip_NewWord.Text = "New";
            this.MenuStrip_NewWord.Click += new System.EventHandler(this.MenuItem_Tokens_NewClick);
            // 
            // MenuStrip_SeparatorWords
            // 
            this.MenuStrip_SeparatorWords.Name = "MenuStrip_SeparatorWords";
            this.MenuStrip_SeparatorWords.Size = new System.Drawing.Size(105, 6);
            // 
            // MenuStrip_TokensSpans
            // 
            this.MenuStrip_TokensSpans.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuStrip_TokensSpans.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_NewSpan,
            this.MenuStrip_SeparatorSpans});
            this.MenuStrip_TokensSpans.Name = "MenuStrip_TokensSpans";
            this.MenuStrip_TokensSpans.Size = new System.Drawing.Size(146, 24);
            this.MenuStrip_TokensSpans.Text = "Spans";
            // 
            // MenuStrip_NewSpan
            // 
            this.MenuStrip_NewSpan.Name = "MenuStrip_NewSpan";
            this.MenuStrip_NewSpan.Size = new System.Drawing.Size(108, 24);
            this.MenuStrip_NewSpan.Tag = "SPAN_NEW";
            this.MenuStrip_NewSpan.Text = "New";
            this.MenuStrip_NewSpan.Click += new System.EventHandler(this.MenuItem_Tokens_NewClick);
            // 
            // MenuStrip_SeparatorSpans
            // 
            this.MenuStrip_SeparatorSpans.Name = "MenuStrip_SeparatorSpans";
            this.MenuStrip_SeparatorSpans.Size = new System.Drawing.Size(105, 6);
            // 
            // V0
            // 
            this.V0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.V0.Name = "V0";
            this.V0.Size = new System.Drawing.Size(146, 24);
            this.V0.Text = "Values";
            this.V0.Click += new System.EventHandler(this.MenuItem_TokensClick);
            // 
            // MenuStrip_TopForceParse
            // 
            this.MenuStrip_TopForceParse.Name = "MenuStrip_TopForceParse";
            this.MenuStrip_TopForceParse.Size = new System.Drawing.Size(96, 24);
            this.MenuStrip_TopForceParse.Text = "Force Parse";
            // 
            // compileShaderToolStripMenuItem
            // 
            this.compileShaderToolStripMenuItem.Name = "compileShaderToolStripMenuItem";
            this.compileShaderToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.compileShaderToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.compileShaderToolStripMenuItem.Size = new System.Drawing.Size(127, 24);
            this.compileShaderToolStripMenuItem.Text = "Compile Shader";
            this.compileShaderToolStripMenuItem.Click += new System.EventHandler(this.compileShaderToolStripMenuItem_Click);
            // 
            // buildProgramToolStripMenuItem
            // 
            this.buildProgramToolStripMenuItem.Name = "buildProgramToolStripMenuItem";
            this.buildProgramToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.buildProgramToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.buildProgramToolStripMenuItem.Size = new System.Drawing.Size(116, 24);
            this.buildProgramToolStripMenuItem.Text = "Build Program";
            this.buildProgramToolStripMenuItem.Click += new System.EventHandler(this.buildProgramToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "thm";
            this.openFileDialog1.FileName = "*.thm";
            this.openFileDialog1.Filter = "Themes files|*.thm|All files|*.*";
            this.openFileDialog1.Title = "Open Theme File";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "thm";
            this.saveFileDialog1.Filter = "Themes files|*.thm|All files|*.*";
            this.saveFileDialog1.Title = "Save Theme File";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(883, 461);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.editorBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(875, 432);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Vertex Shader";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // editorBox1
            // 
            this.editorBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editorBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editorBox1.ForeColor = System.Drawing.Color.White;
            this.editorBox1.Location = new System.Drawing.Point(0, 0);
            this.editorBox1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.editorBox1.Name = "editorBox1";
            this.editorBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.editorBox1.Size = new System.Drawing.Size(875, 432);
            this.editorBox1.TabIndex = 17;
            this.editorBox1.Text = resources.GetString("editorBox1.Text");
            this.editorBox1.WordWrap = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.editorBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(875, 432);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Fragment Shader";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // editorBox2
            // 
            this.editorBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorBox2.BackColor = System.Drawing.SystemColors.Window;
            this.editorBox2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editorBox2.Location = new System.Drawing.Point(0, 0);
            this.editorBox2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.editorBox2.Name = "editorBox2";
            this.editorBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.editorBox2.Size = new System.Drawing.Size(875, 432);
            this.editorBox2.TabIndex = 0;
            this.editorBox2.Text = resources.GetString("editorBox2.Text");
            this.editorBox2.WordWrap = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 488);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(883, 103);
            this.textBox1.TabIndex = 19;
            this.textBox1.Text = "Output";
            this.textBox1.WordWrap = false;
            // 
            // MenuStrip_TopFile
            // 
            this.MenuStrip_TopFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadShaderToolStripMenuItem,
            this.saveShaderToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.quitToolStripMenuItem,
            this.quitToolStripMenuItem1});
            this.MenuStrip_TopFile.Name = "MenuStrip_TopFile";
            this.MenuStrip_TopFile.Size = new System.Drawing.Size(44, 24);
            this.MenuStrip_TopFile.Text = "File";
            // 
            // loadShaderToolStripMenuItem
            // 
            this.loadShaderToolStripMenuItem.Name = "loadShaderToolStripMenuItem";
            this.loadShaderToolStripMenuItem.Size = new System.Drawing.Size(161, 24);
            this.loadShaderToolStripMenuItem.Text = "Load Shader";
            // 
            // saveShaderToolStripMenuItem
            // 
            this.saveShaderToolStripMenuItem.Name = "saveShaderToolStripMenuItem";
            this.saveShaderToolStripMenuItem.Size = new System.Drawing.Size(161, 24);
            this.saveShaderToolStripMenuItem.Text = "Save Shader";
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(161, 24);
            this.saveAllToolStripMenuItem.Text = "Save All";
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(158, 6);
            // 
            // quitToolStripMenuItem1
            // 
            this.quitToolStripMenuItem1.Name = "quitToolStripMenuItem1";
            this.quitToolStripMenuItem1.Size = new System.Drawing.Size(161, 24);
            this.quitToolStripMenuItem1.Text = "Exit";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(883, 589);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Custom IDE / GLSL Shader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_TopTheme;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ThemeFont;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ThemeLoad;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ThemeSave;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ThemeTokens;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_TokensDelimiter;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_NewDelimiter;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_SeparatorDelimiters;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_TokensWords;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_NewWord;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_SeparatorWords;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_TokensSpans;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_NewSpan;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_SeparatorSpans;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_TopForceParse;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ToolStripMenuItem V0;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ThemeColors;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ColorsText;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ColorsBackground;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_ColorsCurrentline;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private EditorBox editorBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripMenuItem compileShaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildProgramToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private EditorBox editorBox2;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_TopFile;
        private System.Windows.Forms.ToolStripMenuItem loadShaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveShaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem1;
    }
}

