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
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            var releaseresources = assembly.GetManifestResourceNames()
                .Where(n => n.ToLowerInvariant().Contains(".releasenotes."))
                .Select(n => new ReleaseDoc(n))
                .Where(n => n.Version <= version)
                .OrderByDescending(n => n.Version);
            
            var welcome = new Welcome();
            welcome.cmbVersions.Items.AddRange(releaseresources.ToArray());
            welcome.txtWelcome.Text = welcome.txtWelcome.Text.Replace("{version}", version.ToString());
            welcome.ShowDialog(owner);
        }

        private Welcome()
        {
            InitializeComponent();
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            if (cmbVersions.Items.Count > 0)
            {
                cmbVersions.SelectedIndex = 0;
            }
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
            MessageBox.Show(@"The evolution of FetchXML Builder is based on feedback issues and anonymous statistics collected about usage.
The statistics are a valuable source of information for continuing the development to make the tool even easier to use and improve the most popular features.

Thank You,
Jonas", "Anonymous statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtNotes_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var url = e.LinkText;
            if (url.StartsWith("#"))
            {
                url = "https://github.com/rappen/FetchXMLBuilder/issues/" + url.Substring(1);
            }
            if (url.StartsWith("@"))
            {
                url = "https://twitter.com/" + url.Substring(1);
            }
            System.Diagnostics.Process.Start(url);
        }

        private void cmbVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            var notes = string.Empty;
            if (cmbVersions.SelectedItem is ReleaseDoc releaseresource)
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(releaseresource.ResourceName))
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
            txtNotes.Rtf = notes.ToString();
        }
    }

    public class ReleaseDoc
    {
        public string ResourceName;
        public Version Version;

        public ReleaseDoc(string resourcename)
        {
            ResourceName = resourcename;
            var docver = resourcename.ToLowerInvariant().Split(new string[] { ".releasenotes." }, StringSplitOptions.None)[1].Replace(".rtf", "");
            Version = new Version(docver);
        }

        public override string ToString()
        {
            return Version.ToString();
        }
    }
}
