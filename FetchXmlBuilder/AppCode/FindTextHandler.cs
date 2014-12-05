using Cinteros.Xrm.FetchXmlBuilder.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    class FindTextHandler
    {
        public static string HandleFindKeyPress(KeyEventArgs e, RichTextBox textBox, string findtext)
        {
            var result = findtext;
            var findHandled = false;
            if (e.KeyCode == Keys.F && e.Modifiers == Keys.Control)
            {
                findHandled = true;
                result = Prompt.ShowDialog("Enter text to find", "Find text", result);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    FindTheText(textBox, result, 0);
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                findHandled = true;
                FindTheText(textBox, result, textBox.SelectionStart + 1);
            }
            if (findHandled)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            return result;
        }

        private static int FindTheText(RichTextBox textBox, string text, int start)
        {
            // Initialize the return value to false by default. 
            int returnValue = -1;

            // Ensure that a search string has been specified and a valid start point. 
            if (text.Length > 0 && start >= 0)
            {
                if (!textBox.Focused)
                {
                    textBox.Focus();
                }
                // Obtain the location of the search string in richTextBox1. 
                int indexToText = textBox.Find(text, start, RichTextBoxFinds.None);
                // Determine whether the text was found in richTextBox1. 
                if (indexToText >= 0)
                {
                    returnValue = indexToText;
                }
            }
            if (returnValue == -1)
            {
                if (start == 0)
                {
                    MessageBox.Show("Text \"" + text + "\" was not found.", "Find text", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (MessageBox.Show("No more occurence of \"" + text + "\" was found.\nSearch from the beginning?", "Find text", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    FindTheText(textBox, text, 0);
                }
            }
            return returnValue;
        }

    }
}
