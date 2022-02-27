using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Microsoft.Web.WebView2.Core;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class Welcome : Form
    {
        public static void ShowWelcome(Control owner, string oldversion = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var showversion = $"{version}";
            if (!string.IsNullOrEmpty(oldversion))
            {
                showversion = "New version: " + showversion + $"\nOld version: {oldversion}";
            }

            var verurl = $"{version.Major}-{version.Minor}-{version.Build}";
            var url = $"https://jonasr.app/fxb/releases/{verurl}#content";
            url = Utils.ProcessURL(url);

            new Welcome(showversion, url).ShowDialog(owner);
        }

        private Welcome(string version, string releasenotes)
        {
            InitializeComponent();

            lblVersion.Text = version;
         
            try
            {
                // Using this to test available of WebView2 - tips by Guido Preite!
                CoreWebView2Environment.GetAvailableBrowserVersionString();
                webRelease.Source = new Uri(releasenotes);
            }
            catch
            {
                FetchXmlBuilder.OpenURL(releasenotes);
            }
        }

        private void llTwitter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL("http://twitter.com/FetchXMLBuilder");
        }

        private void llWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL("https://fetchxmlbuilder.com");
        }

        private void llStats_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@"The evolution of FetchXML Builder is based on feedback issues and anonymous statistics collected about usage.
The statistics are a valuable source of information for continuing the development to make the tool even easier to use and improve the most popular features.

Thank You,
Jonas", "Anonymous statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void webRelease_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            lblLoading.Visible = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL(webRelease.Source.ToString());
            Close();
        }
    }
}
