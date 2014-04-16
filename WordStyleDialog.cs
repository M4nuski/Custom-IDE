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
        public WordStruct DialogResultWordStruct;
        public Color PrevireBackColor;

        public WordStyleDialog()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(WordStruct wordsStructToUpdate, Color backGroundColor)
        { // Polymorphed to include default display

          // Fill form with current data
            textBox2.Text = wordsStructToUpdate.Name;
            textBox1.Lines = wordsStructToUpdate.Keywords;

            colorDialog1.Color = wordsStructToUpdate.Style.StyleColor;

            PrevireBackColor = backGroundColor;
            Update_Preview();

            checkBox1.Checked = wordsStructToUpdate.Style.StyleFont.Bold;
            checkBox2.Checked = wordsStructToUpdate.Style.StyleFont.Italic;
            checkBox3.Checked = wordsStructToUpdate.Style.StyleFont.Strikeout;
            checkBox4.Checked = wordsStructToUpdate.Style.StyleFont.Underline;

            return ShowDialog();
        }

        public new DialogResult ShowDialog()
        { // Polymorphed to inject preview label update
            Update_Preview();
            return base.ShowDialog();
        }

        private void close_Dialog(object sender, EventArgs e) 
        {
            var button = sender as Button;
            if (button != null)
            {
                //fill properties with current form data
                if (button.DialogResult == DialogResult.OK)
                {
                    DialogResultWordStruct.Name = textBox2.Text;
                    DialogResultWordStruct.Keywords = textBox1.Lines;

                    DialogResultWordStruct.Style.StyleColor = colorDialog1.Color;

                    DialogResultWordStruct.Style.StyleFont = new Font(DialogResultWordStruct.Style.StyleFont, BoxesToStyle());
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

        private void Update_Preview()
        {
            label1.Font = DialogResultWordStruct.Style.StyleFont;
            label1.ForeColor = DialogResultWordStruct.Style.StyleColor;
            label1.BackColor = PrevireBackColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = DialogResultWordStruct.Style.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                DialogResultWordStruct.Style.StyleColor = colorDialog1.Color;
                Update_Preview();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            DialogResultWordStruct.Style.StyleFont = new Font(DialogResultWordStruct.Style.StyleFont, BoxesToStyle());
            Update_Preview();
        }
    }
}
