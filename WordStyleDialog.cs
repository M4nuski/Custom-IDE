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
        public WordStruct DialogResultWordStruct; //Output Struct
        private Color _previewBackColor;

        public WordStyleDialog()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(WordStruct wordsStructToUpdate, Color backGroundColor)
        { // Polymorphed to include actual style
            DialogResultWordStruct = wordsStructToUpdate;
            _previewBackColor = backGroundColor;

            Update_All();
            return ShowDialog();
        }

        public DialogResult ShowDialog(FontAndColorStruct styleToUpdate, Color backGroundColor)
        { // Polymorphed to inject style
            _previewBackColor = backGroundColor;
            DialogResultWordStruct.Name = "<none>";
            DialogResultWordStruct.Keywords = new [] {"<none>"}; 
            DialogResultWordStruct.Style = styleToUpdate;

            Update_All();
            return base.ShowDialog();
        }

        public new DialogResult ShowDialog()
        { // Polymorphed to inject content update
            Update_All();
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

                    DialogResultWordStruct.Style.StyleFont = new Font(label1.Font, BoxesToStyle());
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
            label1.ForeColor = colorDialog1.Color;
            label1.BackColor = _previewBackColor;
            label1.Refresh();
        }

        private void Update_All()
        {
            textBox2.Text = DialogResultWordStruct.Name;
            textBox1.Lines = DialogResultWordStruct.Keywords;
            textBox1.Select(0, 0);

            colorDialog1.Color = DialogResultWordStruct.Style.StyleColor;
            checkBox1.Checked = DialogResultWordStruct.Style.StyleFont.Bold;
            checkBox2.Checked = DialogResultWordStruct.Style.StyleFont.Italic;
            checkBox3.Checked = DialogResultWordStruct.Style.StyleFont.Strikeout;
            checkBox4.Checked = DialogResultWordStruct.Style.StyleFont.Underline; 
  
            Update_Preview();
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

        private void checkBoxs_CheckedChanged(object sender, EventArgs e)
        {
            var oldFont = label1.Font; 
            DialogResultWordStruct.Style.StyleFont = new Font(oldFont, BoxesToStyle());
            Update_Preview();
        }
    }
}
