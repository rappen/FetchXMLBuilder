using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    }
}
