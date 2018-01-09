using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class Welcome : Form
    {
        public static void ShowWelcome(Control owner)
        {
            var notes = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            var releaseresource = assembly.GetManifestResourceNames()
                .Where(n => n.ToLowerInvariant().Contains(".releasenotes.") && ExtractReleaseVersions(n) <= version)
                .OrderByDescending(n => ExtractReleaseVersions(n))
                .FirstOrDefault();

            if (releaseresource != null)
            {
                using (Stream stream = assembly.GetManifestResourceStream(releaseresource))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            notes = reader.ReadToEnd();
                        }
                    }
                }
            }
            var welcome = new Welcome();
            welcome.txtWelcome.Text = welcome.txtWelcome.Text.Replace("{version}", version.ToString());
            welcome.txtNotes.Rtf = notes.ToString();
            welcome.ShowDialog(owner);
        }

        private static Version ExtractReleaseVersions(string filename)
        {
            filename = filename.ToLowerInvariant();
            if (filename.Contains(".releasenotes.") && !filename.EndsWith(".releasenotes."))
            {
                filename = filename.Substring(filename.IndexOf(".releasenotes.") + 14);
            }
            if (filename.EndsWith(".rtf"))
            {
                filename = filename.Substring(0, filename.IndexOf(".rtf"));
            }
            var version = new Version(filename);
            return version;
        }

        private Welcome()
        {
            InitializeComponent();
        }

        private void llTwitter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://twitter.com/FetchXMLBuilder");
        }

        private void llWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://fxb.xrmtoolbox.com");
        }

        private void lblContinue_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void llStats_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@"The evolution of FetchXML Builder is based on feedback issues and anonymous statistics collected from users.
The statistics are a valuable source of information for continuing the development to make the tool even easier to use and improve the most popular features.

By default the usage statistics is turned on. If you do not want to participate in this, it can be turned off under Options from the main window.

I am very grateful for anyone who allows statistics to be collected!

Thank You,
Jonas", "Anonymous statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtNotes_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var url = e.LinkText;
            if (url.StartsWith("#"))
            {
                url = "https://github.com/Innofactor/FetchXMLBuilder/issues/" + url.Substring(1);
            }
            if (url.StartsWith("@"))
            {
                url = "https://twitter.com/" + url.Substring(1);
            }
            System.Diagnostics.Process.Start(url);
        }
    }
}
