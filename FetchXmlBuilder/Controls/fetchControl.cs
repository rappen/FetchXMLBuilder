using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class fetchControl : FetchXmlElementControlBase
    {
        public fetchControl() : this(null, null)
        {
        }

        public fetchControl(Dictionary<string, string> collection, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, null, tree, null);
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
    }
}