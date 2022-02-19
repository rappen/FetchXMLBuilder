using Microsoft.Xrm.Sdk;
using Rappen.XTB.Helpers.Extensions;
using Rappen.XTB.Helpers.XTBExtensions;
using System;
using System.ServiceModel;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class ErrorDetail : Form
    {
        private PluginControlBase owner;
        private DateTime timestamp;
        private Exception exception;
        private string extrainfo;

        public static void ShowDialog(PluginControlBase owner, Exception exception, string heading = null, string extrainfo = null)
        {
            if (owner == null || exception == null)
            {
                return;
            }
            new ErrorDetail(owner, exception, heading, extrainfo).ShowDialog(owner);
        }

        private ErrorDetail(PluginControlBase owner, Exception exception, string heading, string extrainfo)
        {
            this.owner = owner;
            this.exception = exception;
            this.extrainfo = extrainfo;
            timestamp = DateTime.Now;
            InitializeComponent();
            if (!string.IsNullOrEmpty(heading))
            {
                Text = heading;
            }
            AddErrorInfo(exception);
            Height = 200;
        }

        private void AddErrorInfo(Exception error)
        {
            try
            {
                var msg = error.Message;
                if (error is FaultException<OrganizationServiceFault> srcexc)
                {
                    msg = srcexc.Message;
                    if (srcexc.Detail is OrganizationServiceFault orgerr)
                    {
                        msg = orgerr.Message;
                        if (orgerr.InnerFault != null)
                        {
                            msg = orgerr.InnerFault.Message;
                        }
                        txtErrorCode.Text = "0x" + orgerr.ErrorCode.ToString("X");
                    }
                }
                if (msg.StartsWith("System") && msg.Contains(": ") && msg.Split(':')[0].Length < 50)
                {
                    msg = msg.Substring(msg.IndexOf(':') + 1);
                }
                if (msg.Contains("MetadataCacheDetails: "))
                {
                    msg = msg.Substring(0, msg.IndexOf("MetadataCacheDetails"));
                }
                if (msg.Contains("   at "))
                {
                    msg = msg.Substring(0, msg.IndexOf("   at "));
                }
                msg = msg.Trim();
                txtInfo.Text = msg;
                txtException.Text = error.ToTypeString();
                txtMessage.Text = error.Message;
                txtCallStack.Text = error.StackTrace.Trim();
            }
            catch
            {
                txtInfo.Text = error.Message;
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (Height < 300)
            {
                btnDetails.Text = "Hide Details";
                panDetails.Visible = true;
                btnCopy.Visible = true;
                btnIssue.Visible = true;
                Height = 550;
            }
            else
            {
                btnDetails.Text = "Show Details";
                panDetails.Visible = false;
                btnCopy.Visible = false;
                btnIssue.Visible = false;
                Height = 200;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var details = "Error Time: " + timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\n";
            details += txtException.Text;
            if (!string.IsNullOrEmpty(txtErrorCode.Text))
            {
                details += $" ({txtErrorCode.Text})";
            }
            details += $"\n{txtMessage.Text}";
            details += $"\n{txtCallStack.Text}";
            Clipboard.SetText(details);
            MessageBox.Show("Copied all details.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnIssue_Click(object sender, EventArgs e)
        {
            owner.CreateNewIssueFromError(exception, extrainfo);
            TopMost = false;
        }
    }
}
