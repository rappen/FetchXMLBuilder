using Rappen.XTB.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class About : Form
    {
        private List<AssemblyName> assemblies;

        public About()
        {
            InitializeComponent();
            PopulateAssemblies();
        }

        private void PopulateAssemblies()
        {
            assemblies = GetReferencedAssemblies();
            var items = assemblies.Select(a => GetListItem(a)).ToArray();
            listAssemblies.Items.Clear();
            listAssemblies.Items.AddRange(items);
        }

        private ListViewItem GetListItem(AssemblyName a)
        {
            var assembly = Assembly.Load(a);
            var fi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var item = new ListViewItem(a.Name);
            item.SubItems.Add(fi.FileVersion);
            return item;
        }

        private List<AssemblyName> GetReferencedAssemblies()
        {
            var names = Assembly.GetExecutingAssembly().GetReferencedAssemblies().ToList();
            if (!chkAllAssemblies.Checked)
            {
                names = names.Where(a =>
                    !a.Name.Equals("mscorlib") &&
                    !a.Name.StartsWith("System") &&
                    !a.Name.Contains("CSharp")).ToList();
            }
            names.Add(Assembly.GetEntryAssembly().GetName());
            names.Add(Assembly.GetExecutingAssembly().GetName());
            names = names.OrderBy(a => assemblyPrioritizer(a.Name)).ToList();
            return names;
        }

        private static string assemblyPrioritizer(string assemblyName)
        {
            return
                assemblyName.Equals("XrmToolBox") ? "AAAAAAAAAAAA" :
                assemblyName.Contains("XrmToolBox") ? "AAAAAAAAAAAB" :
                assemblyName.Equals(Assembly.GetExecutingAssembly().GetName().Name) ? "AAAAAAAAAAAC" :
                assemblyName.Contains("Jonas") ? "AAAAAAAAAAAD" :
                assemblyName.Contains("Rappen") ? "AAAAAAAAAAAE" :
                assemblyName.Contains("Innofactor") ? "AAAAAAAAAAAF" :
                assemblyName.Contains("Cinteros") ? "AAAAAAAAAAAG" :
                assemblyName;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl("https://fetchxmlbuilder.com");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl("https://jonasr.app");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl("https://x.com/FetchXMLBuilder");
        }

        private void llShowWelcome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Welcome.ShowWelcome(this);
            Close();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl("https://icons8.com");
        }

        private void chkAllAssemblies_CheckedChanged(object sender, System.EventArgs e)
        {
            PopulateAssemblies();
        }

        private void lnkCopyAssemblyinfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var dlls = new List<List<string>>();
            foreach (var dll in assemblies)
            {
                var assembly = Assembly.Load(dll);
                var fi = FileVersionInfo.GetVersionInfo(assembly.Location);
                dlls.Add(new List<string> { dll.Name, fi.FileVersion, assembly.Location });
            }
            dlls = dlls.Distinct().ToList();
            var length0 = dlls.Select(d => d[0].Length).Max() + 1;
            var length1 = dlls.Select(d => d[1].Length).Max() + 1;
            var length2 = dlls.Select(d => d[2].Length).Max();
            dlls.Insert(0, new List<string> { "----".PadRight(length0, '-'), "-------".PadRight(length1, '-'), "----".PadRight(length2, '-') });
            dlls.Insert(0, new List<string> { "Name", "Version", "File" });
            var assemblyInfo = string.Join("\n", dlls.Select(d => d[0].PadRight(length0) + d[1].PadRight(length1) + d[2].PadRight(length2)));
            Clipboard.SetText(assemblyInfo);
            Cursor = Cursors.Default;
            MessageBoxEx.Show(this, "Assembly information copied to clipboard.", "Assembly Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}