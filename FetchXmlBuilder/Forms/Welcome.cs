using Rappen.XTB.Helpers;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class Welcome : Form
    {
        private Control caller;

        public bool WebView2Success { get; private set; }

        public static void ShowWelcome(Control owner, string oldversion = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var showversion = $"{version}";
            if (!string.IsNullOrEmpty(oldversion))
            {
                showversion = "New version: " + showversion + $"\nOld version: {oldversion}";
            }

            var releasenotes = GetReleaseNotesUrl(version);
            try
            {
                var form = new Welcome();
                form.caller = owner;
                form.lblVersion.Text = showversion;
                form.webRelease.Source = new Uri(releasenotes);
                form.ShowDialog(owner);
            }
            catch
            {
                UrlUtils.OpenUrl(releasenotes);
            }
        }

        public static string GetReleaseNotesUrl(Version version)
        {
            var verurl = $"{version.Major}-{version.Minor}-{version.Build}";
            return $"https://fetchxmlbuilder.com/releases/{verurl}#content";
        }

        private Welcome()
        {
            InitializeComponent();
        }

        private void llTwitter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl("http://x.com/FetchXMLBuilder");
        }

        private void llWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl("https://fetchxmlbuilder.com");
        }

        private void llStats_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBoxEx.Show(this, @"The evolution of FetchXML Builder is based on feedback issues and anonymous statistics collected about usage.
The statistics are a valuable source of information for continuing the development to make the tool even easier to use and improve the most popular features.

Thank You,
Jonas", "Anonymous statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void webRelease_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            panLoading.Visible = false;
            webRelease.Visible = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl(webRelease.Source);
            Close();
        }

        private void Welcome_Shown(object sender, EventArgs e)
        {
            timerLoading.Start();
        }

        private void timerLoading_Tick(object sender, EventArgs e)
        {
            timerLoading.Stop();
            linkCantLoad.Visible = panLoading.Visible;
            btnClose.Visible = true;
        }
    }
}