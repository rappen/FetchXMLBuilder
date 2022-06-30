using Rappen.XTB.FetchXmlBuilder.DockControls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    public partial class fetchControl : FetchXmlElementControlBase
    {
        public fetchControl() : this(null, null, null)
        {
        }

        public fetchControl(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, fetchXmlBuilder, tree, null);
        }

        private void textPagingCookie_Leave(object sender, EventArgs e)
        {
            textPagingCookie.Text = textPagingCookie.Text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
        }

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == textTop)
            {
                if (!string.IsNullOrEmpty(textTop.Text) && !int.TryParse(textTop.Text, out int p))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Top must be a whole number");
                }
            }
            if (control == textPageSize)
            {
                if (!string.IsNullOrEmpty(textPageSize.Text) && !int.TryParse(textPageSize.Text, out int p))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Page Size must be a whole number");
                }
            }
            if (control == textPage)
            {
                if (!string.IsNullOrEmpty(textPage.Text) && !int.TryParse(textPage.Text, out int p))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Page must be a whole number");
                }
            }
            return base.ValidateControl(control);
        }

        private void helpIcon_Click(object sender, EventArgs e)
        {
            FetchXmlBuilder.HelpClick(sender);
        }

        public override void Focus()
        {
            textTop.Focus();
        }
    }
}