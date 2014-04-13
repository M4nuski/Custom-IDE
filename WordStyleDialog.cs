using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShaderIDE
{
    public partial class WordStyleDialog : Form
    {
        public SWord DialogResultWord;

        public WordStyleDialog()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(SWord wordsToUpdate, Color backGroundColor)
        {
            //fill form with current data
            textBox2.Text = wordsToUpdate.Name;
            textBox1.Lines = wordsToUpdate.Words;

            colorDialog1.Color = wordsToUpdate.Style.StyleColor;

            label2.Font = wordsToUpdate.Style.StyleFont;
            label2.ForeColor = wordsToUpdate.Style.StyleColor;
            label2.BackColor = backGroundColor;

            checkBox1.Checked = wordsToUpdate.Style.StyleFont.Bold;
            checkBox2.Checked = wordsToUpdate.Style.StyleFont.Italic;
            checkBox3.Checked = wordsToUpdate.Style.StyleFont.Strikeout;
            checkBox4.Checked = wordsToUpdate.Style.StyleFont.Underline;

            return ShowDialog();
        }

        private void close_Dialog(object sender, EventArgs e) 
        {
            var button = sender as Button;
            if (button != null)
            {
                //fill properties with current form data
                if (button.DialogResult == DialogResult.OK)
                {
                    DialogResultWord.Name = textBox2.Text;
                    DialogResultWord.Words = textBox1.Lines;

                    DialogResultWord.Style.StyleColor = colorDialog1.Color;

                    DialogResultWord.Style.StyleFont = new Font(DialogResultWord.Style.StyleFont, BoxesToStyle());
                }

                DialogResult = button.DialogResult;
            }
        }

        private FontStyle BoxesToStyle()
        {
            var result = FontStyle.Regular;
            if (checkBox1.Checked) result = result | FontStyle.Bold;
            if (checkBox2.Checked) result = result | FontStyle.Italic;
            if (checkBox3.Checked) result = result | FontStyle.Strikeout;
            if (checkBox4.Checked) result = result | FontStyle.Underline;
            return result;
        }
    }
}
