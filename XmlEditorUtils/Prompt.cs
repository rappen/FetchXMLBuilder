using System.Windows.Forms;

namespace Cinteros.Xrm.XmlEditorUtils
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string startvalue = "")
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.Text = caption;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            Label textLabel = new Label() { Left = 50, Top = 20, Width = 300, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 45, Width = 400, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Text = startvalue };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            //confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            string result = null;
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                result = textBox.Text;
            }
            return result;
        }
    }
}