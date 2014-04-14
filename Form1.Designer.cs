namespace ShaderIDE
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.styleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.colorsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tokensToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.delimitersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wordsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.spansToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.newToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.newToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.forceParseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.HideSelection = false;
            this.richTextBox1.Location = new System.Drawing.Point(0, 84);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBox1.Size = new System.Drawing.Size(1196, 759);
            this.richTextBox1.TabIndex = 12;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            this.richTextBox1.WordWrap = false;
            this.richTextBox1.Click += new System.EventHandler(this.richTextBox1_Click);
            this.richTextBox1.FontChanged += new System.EventHandler(this.richTextBox1_FontChanged);
            this.richTextBox1.ForeColorChanged += new System.EventHandler(this.richTextBox1_FontChanged);
            this.richTextBox1.TextChanged += new System.EventHandler(this.richEditBox_ReDraw);
            this.richTextBox1.Resize += new System.EventHandler(this.richTextBox1_Resize);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.forceParseToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1197, 28);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.styleToolStripMenuItem,
            this.tokensToolStripMenuItem1});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(66, 24);
            this.toolStripMenuItem1.Text = "Theme";
            // 
            // styleToolStripMenuItem
            // 
            this.styleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.styleToolStripMenuItem1,
            this.colorsToolStripMenuItem1});
            this.styleToolStripMenuItem.Name = "styleToolStripMenuItem";
            this.styleToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.styleToolStripMenuItem.Text = "Font";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.loadToolStripMenuItem.Text = "Load...";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.saveToolStripMenuItem.Text = "Save...";
            // 
            // styleToolStripMenuItem1
            // 
            this.styleToolStripMenuItem1.Name = "styleToolStripMenuItem1";
            this.styleToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.styleToolStripMenuItem1.Text = "Style";
            this.styleToolStripMenuItem1.Click += new System.EventHandler(this.styleToolStripMenuItem1_Click);
            // 
            // colorsToolStripMenuItem1
            // 
            this.colorsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textToolStripMenuItem,
            this.backgroundToolStripMenuItem,
            this.currentBackgroundToolStripMenuItem,
            this.errorBackgroundToolStripMenuItem});
            this.colorsToolStripMenuItem1.Name = "colorsToolStripMenuItem1";
            this.colorsToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.colorsToolStripMenuItem1.Text = "Colors";
            // 
            // tokensToolStripMenuItem1
            // 
            this.tokensToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.delimitersToolStripMenuItem1,
            this.wordsToolStripMenuItem1,
            this.spansToolStripMenuItem1,
            this.numbersToolStripMenuItem});
            this.tokensToolStripMenuItem1.Name = "tokensToolStripMenuItem1";
            this.tokensToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.tokensToolStripMenuItem1.Text = "Tokens";
            // 
            // delimitersToolStripMenuItem1
            // 
            this.delimitersToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem3,
            this.toolStripMenuItem4});
            this.delimitersToolStripMenuItem1.Name = "delimitersToolStripMenuItem1";
            this.delimitersToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.delimitersToolStripMenuItem1.Text = "Delimiters";
            // 
            // wordsToolStripMenuItem1
            // 
            this.wordsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem4,
            this.toolStripMenuItem5});
            this.wordsToolStripMenuItem1.Name = "wordsToolStripMenuItem1";
            this.wordsToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.wordsToolStripMenuItem1.Text = "Words";
            // 
            // spansToolStripMenuItem1
            // 
            this.spansToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem5,
            this.toolStripMenuItem6});
            this.spansToolStripMenuItem1.Name = "spansToolStripMenuItem1";
            this.spansToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.spansToolStripMenuItem1.Text = "Spans";
            // 
            // newToolStripMenuItem3
            // 
            this.newToolStripMenuItem3.Name = "newToolStripMenuItem3";
            this.newToolStripMenuItem3.Size = new System.Drawing.Size(152, 24);
            this.newToolStripMenuItem3.Text = "New";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(149, 6);
            // 
            // newToolStripMenuItem4
            // 
            this.newToolStripMenuItem4.Name = "newToolStripMenuItem4";
            this.newToolStripMenuItem4.Size = new System.Drawing.Size(152, 24);
            this.newToolStripMenuItem4.Text = "New";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(149, 6);
            // 
            // newToolStripMenuItem5
            // 
            this.newToolStripMenuItem5.Name = "newToolStripMenuItem5";
            this.newToolStripMenuItem5.Size = new System.Drawing.Size(152, 24);
            this.newToolStripMenuItem5.Text = "New";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(149, 6);
            // 
            // forceParseToolStripMenuItem
            // 
            this.forceParseToolStripMenuItem.Name = "forceParseToolStripMenuItem";
            this.forceParseToolStripMenuItem.Size = new System.Drawing.Size(96, 24);
            this.forceParseToolStripMenuItem.Text = "Force Parse";
            this.forceParseToolStripMenuItem.Click += new System.EventHandler(this.force_Redraw);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(209, 24);
            this.textToolStripMenuItem.Text = "Text";
            this.textToolStripMenuItem.Click += new System.EventHandler(this.textToolStripMenuItem_Click);
            // 
            // backgroundToolStripMenuItem
            // 
            this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
            this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(209, 24);
            this.backgroundToolStripMenuItem.Text = "Background";
            this.backgroundToolStripMenuItem.Click += new System.EventHandler(this.backgroundToolStripMenuItem_Click);
            // 
            // currentBackgroundToolStripMenuItem
            // 
            this.currentBackgroundToolStripMenuItem.Name = "currentBackgroundToolStripMenuItem";
            this.currentBackgroundToolStripMenuItem.Size = new System.Drawing.Size(240, 24);
            this.currentBackgroundToolStripMenuItem.Text = "Current Line Background";
            this.currentBackgroundToolStripMenuItem.Click += new System.EventHandler(this.currentBackgroundToolStripMenuItem_Click);
            // 
            // errorBackgroundToolStripMenuItem
            // 
            this.errorBackgroundToolStripMenuItem.Name = "errorBackgroundToolStripMenuItem";
            this.errorBackgroundToolStripMenuItem.Size = new System.Drawing.Size(240, 24);
            this.errorBackgroundToolStripMenuItem.Text = "Error Background";
            this.errorBackgroundToolStripMenuItem.Click += new System.EventHandler(this.errorBackgroundToolStripMenuItem_Click);
            // 
            // numbersToolStripMenuItem
            // 
            this.numbersToolStripMenuItem.Name = "numbersToolStripMenuItem";
            this.numbersToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.numbersToolStripMenuItem.Text = "Values";
            this.numbersToolStripMenuItem.Click += new System.EventHandler(this.numbersToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1197, 840);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem colorsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tokensToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem delimitersToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem wordsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem spansToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem forceParseToolStripMenuItem;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numbersToolStripMenuItem;
    }
}

