using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class ErrorDetail : Form
    {
        public static void ShowDialog(Control owner, Exception error, string heading = null)
        {
            if (error == null)
            {
                return;
            }
            var form = new ErrorDetail();
            if (!string.IsNullOrEmpty(heading))
            {
                form.Text = heading;
            }
            form.AddErrorInfo(error);
            form.Height = 200;
            form.ShowDialog(owner);
        }

        public ErrorDetail()
        {
            InitializeComponent();
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
                        txtErrorCode.Text = orgerr.ErrorCode.ToString();
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
                txtException.Text = error.GetType().ToString();
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
                Height = 600;
            }
            else
            {
                Height = 200;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL("https://github.com/rappen/FetchXMLBuilder/discussions/617");
        }
    }
}
