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
    public partial class SpanStyleDialog : Form
    {
        public SpanStruct DialogOutput;
        private Color _previewBackColor;
        private bool _updatingCheckboxes;
        public SpanStyleDialog()
        {
            InitializeComponent();
        }
        public DialogResult ShowDialog(SpanStruct structToUpdate, Color backGroundColor)
        {
            _previewBackColor = backGroundColor;
            DialogOutput = structToUpdate;

            return ShowDialog();
        }

        public new DialogResult ShowDialog()
        { // Polymorphed to include actual style
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
                    DialogOutput.Name = textBox2.Text;

                    DialogOutput.StartKeyword = textBox1.Text;
                    DialogOutput.StopKeyword = textBox3.Text;
                    DialogOutput.EscapeChar = (textBox4.Text == "") ? '\n' : textBox4.Text[0];

                    DialogOutput.Style.StyleColor = colorDialog1.Color;

                    DialogOutput.Style.StyleFont = new Font(label1.Font, BoxesToStyle());
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
            label1.Font = DialogOutput.Style.StyleFont;
            label1.ForeColor = colorDialog1.Color;
            label1.BackColor = _previewBackColor;
            label1.Refresh();
        }

        private void Update_All()
        {
            textBox2.Text = DialogOutput.Name;
            //textBox2.ReadOnly = (DialogOutput.Name != "<new>");

            textBox1.Text = DialogOutput.StartKeyword;
            textBox3.Text = DialogOutput.StopKeyword;

            textBox4.Text = (DialogOutput.EscapeChar == '\n') ? "" : new string(DialogOutput.EscapeChar, 1);

            textBox1.Select(0, 0);

            colorDialog1.Color = DialogOutput.Style.StyleColor;
            Update_Preview();

            _updatingCheckboxes = true;
            checkBox1.Checked = DialogOutput.Style.StyleFont.Bold;
            checkBox2.Checked = DialogOutput.Style.StyleFont.Italic;
            checkBox3.Checked = DialogOutput.Style.StyleFont.Strikeout;
            checkBox4.Checked = DialogOutput.Style.StyleFont.Underline;
            _updatingCheckboxes = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = DialogOutput.Style.StyleColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                DialogOutput.Style.StyleColor = colorDialog1.Color;
                Update_Preview();
            }
        }

        private void checkBoxes_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updatingCheckboxes)
            {
                var oldFont = label1.Font;
                DialogOutput.Style.StyleFont = new Font(oldFont, BoxesToStyle());
                Update_Preview();
            }
        }
    }
}
