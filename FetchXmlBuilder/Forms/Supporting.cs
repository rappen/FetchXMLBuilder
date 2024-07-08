﻿using Rappen.XTB.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Mail;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.AppCode.AppInsights;
using XrmToolBox.Extensibility;
using XrmToolBox.ToolLibrary.AppCode;

namespace Rappen.XTB
{
    public partial class Supporting : Form
    {
        private const string formidcorp = "wpf17273";
        private const string formidpriv = "wpf17612";

        private const string urlcorp =
            "https://jonasr.app/supporting-prefilled/" +
            "?{formid}_1_first={firstname}" +
            "&{formid}_1_last={lastname}" +
            "&{formid}_27={company}" +
            "&{formid}_3={country}" +
            "&{formid}_4={invoiceemail}" +
            "&{formid}_19={size}" +
            "&{formid}_24={amount}" +
            "&{formid}_13={tool}" +
            "&{formid}_31={tool}" +
            "&{formid}_32={version}" +
            "&{formid}_33={instid}";

        private const string urlinde =
            "https://jonasr.app/supporting/personal-prefilled/" +
            "?{formid}_1_first={firstname}" +
            "&{formid}_1_last={lastname}" +
            "&{formid}_3={country}" +
            "&{formid}_4={email}" +
            "&{formid}_13={tool}" +
            "&{formid}_31={tool}" +
            "&{formid}_32={version}" +
            "&{formid}_33={instid}";

        private string toolname;
        private readonly RappenXTBTools tools;
        private readonly Tool tool;
        private Supporters supporters;
        private bool timetoshow = true;

        public static void MayShow(PluginControlBase plugin)
        {
            var form = new Supporting(plugin.ToolName);
            if (form.timetoshow)
            {
                form.PopulateTool();
                form.ShowDialog();
            }
        }

        private Supporting(string toolname)
        {
            InitializeComponent();
            this.toolname = toolname;
            tools = RappenXTBTools.Load();
            tool = tools[toolname];
            supporters = Supporters.DownloadMy(tools.InstallationId, toolname);
        }

        private void PopulateTool()
        {
            if (supporters?.Any(s => s.InstallationId.Equals(tools.InstallationId) && s.ToolName == toolname) == true)
            {
                lblAlready.Visible = true;
                lblLater.Visible = false;
            }
            txtCompany.Text = tools.Company;
            txtEmail.Text = tools.InvoiceEmail;
            cmbSize.SelectedIndex = tool.UserIndex;
            txtCountry.Text = tools.Country;
            txtIFirst.Text = tools.FirstName;
            txtILast.Text = tools.LastName;
            txtIEmail.Text = tools.Email;
            txtICountry.Text = tools.Country;
            if (tool.SupportType == SupportType.Personal)
            {
                rbPersonal.Checked = true;
            }
            else
            {
                rbCompany.Checked = true;
            }
        }

        private void Supporting_FormClosing(object sender, FormClosingEventArgs e)
        {
            tool.OpenedSupportingLastDate = DateTime.Now;
            tool.OpenSupportingCount++;
            tool.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            tools.Save();
            if (DialogResult != DialogResult.OK && DialogResult != DialogResult.Retry)
            {
                e.Cancel = true;
            }
        }

        private void btnCorp_Click(object sender, EventArgs e)
        {
            var url = rbPersonal.Checked ? GetUrlPersonal() : GetUrlCorp();

            if (!string.IsNullOrEmpty(url))
            {
                if (MessageBoxEx.Show(this, "You are now redirected to the website form to finish the support.", "Supporting", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
                {
                    return;
                }
                Process.Start(url);
                tool.SupportType = rbPersonal.Checked ? SupportType.Personal : SupportType.Company;
                DialogResult = DialogResult.Yes;
            }
        }

        private string GetUrlCorp()
        {
            if (string.IsNullOrEmpty(tools.Company) ||
                string.IsNullOrEmpty(tools.InvoiceEmail) ||
                string.IsNullOrEmpty(tools.Country) ||
                tool.UserIndex < 1)
            {
                return null;
            }
            return GenerateUrl(urlcorp, formidcorp);
        }

        private string GetUrlPersonal()
        {
            if (string.IsNullOrEmpty(tools.FirstName) ||
                string.IsNullOrEmpty(tools.LastName) ||
                string.IsNullOrEmpty(tools.Email) ||
                string.IsNullOrEmpty(tools.Country))
            {
                return null;
            }
            return GenerateUrl(urlinde, formidpriv);
        }

        private string GenerateUrl(string template, string form)
        {
            return template
                .Replace("{formid}", form)
                .Replace("{firstname}", tools.FirstName)
                .Replace("{lastname}", tools.LastName)
                .Replace("{company}", tools.Company)
                .Replace("{country}", tools.Country)
                .Replace("{email}", tools.Email)
                .Replace("{invoiceemail}", tools.InvoiceEmail)
                .Replace("{size}", cmbSize.Items[tool.UserIndex].ToString())
                .Replace("{amount}", tool.Amount)
                .Replace("{tool}", tool.ToolName)
                .Replace("{version}", tool.Version.ToString())
                .Replace("{instid}", tools.InstallationId.ToString());
        }

        private void later_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }

        private void lblAlready_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void rbType_CheckedChanged(object sender, EventArgs e)
        {
            panPersonal.Left = panCorp.Left;
            panPersonal.Top = panCorp.Top;
            panPersonal.Visible = rbPersonal.Checked;
            panCorp.Visible = !panPersonal.Visible;
            btnSave.ImageIndex = rbPersonal.Checked ? 1 : 0;
        }

        private void linkHelping_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl(sender);
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            panInfo.Visible = !panInfo.Visible;
            panInfo.Left = 50;
            panInfo.Top = 10;
        }

        private void btnInfoClose_Click(object sender, EventArgs e)
        {
            panInfo.Visible = false;
        }

        private void lblLater_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }

        private void txtCompany_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tools.Company = txtCompany.Text.Trim().Length >= 3 ? txtCompany.Text.Trim() : "";
            lblCompany.ForeColor = string.IsNullOrEmpty(tools.Company) ? Color.DeepPink : Color.Yellow;
        }

        private void txtEmail_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                tools.InvoiceEmail = new MailAddress(txtEmail.Text).Address.Trim();
            }
            catch { }
            lblInvoiceemail.ForeColor = string.IsNullOrEmpty(tools.InvoiceEmail) ? Color.DeepPink : Color.Yellow;
        }

        private void txtCountry_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tools.Country = txtCountry.Text.Trim().Length >= 2 ? txtCountry.Text.Trim() : "";
            lblCountry.ForeColor = string.IsNullOrEmpty(tools.Country) ? Color.DeepPink : Color.Yellow;
        }

        private void cmbSize_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tool.UserIndex = cmbSize.SelectedIndex;
            tool.UserCount = cmbSize.Text;
            lblSize.ForeColor = tool.UserIndex < 1 ? Color.DeepPink : Color.Yellow;
        }

        private void txtIFirst_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tools.FirstName = txtIFirst.Text.Trim().Length >= 1 ? txtIFirst.Text.Trim() : "";
            tools.LastName = txtILast.Text.Trim().Length >= 2 ? txtILast.Text.Trim() : "";
            lblIName.ForeColor = string.IsNullOrEmpty(tools.FirstName) || string.IsNullOrEmpty(tools.LastName) ? Color.DeepPink : Color.Yellow;
        }

        private void txtIEmail_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                tools.Email = new MailAddress(txtIEmail.Text).Address.Trim();
            }
            catch { }
            lblIEmail.ForeColor = string.IsNullOrEmpty(tools.Email) ? Color.DeepPink : Color.Yellow;
        }

        private void txtICountry_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tools.Country = txtICountry.Text.Trim().Length >= 2 ? txtICountry.Text.Trim() : "";
            lblICountry.ForeColor = string.IsNullOrEmpty(tools.Country) ? Color.DeepPink : Color.Yellow;
        }
    }

    public class Supporters : List<Supporter>
    {
        private const string guidregex = @"([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})";

        public static Supporters DownloadMy(Guid InstallationId, string toolname)
        {
            Supporters result;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var webRequestXml = HttpWebRequest.Create("https://jonasr.app/xtb/supporters.xml") as HttpWebRequest;
            webRequestXml.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Revalidate);
            webRequestXml.Accept = "text/html, application/xhtml+xml, */*";
            try
            {
                using (var response = webRequestXml.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    var strContent = reader.ReadToEnd();
                    result = (Supporters)XmlSerializerHelper.Deserialize(strContent, typeof(Supporters));
                    result.Where(s => s.InstallationId != InstallationId || s.ToolName != toolname).ToList().ForEach(s => result.Remove(s));
                    return result;
                }
            }
            catch
            {
                return new Supporters();
            }
        }
    }

    public class Supporter
    {
        public Guid InstallationId { get; set; }
        public string ToolName { get; set; }
        public SupportType SupportType { get; set; }
    }

    public class RappenXTBTools
    {
        public Guid InstallationId { get; set; } = Guid.Empty;
        public List<Tool> Tools { get; set; } = new List<Tool>();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string InvoiceEmail { get; set; }

        public static RappenXTBTools Load()
        {
            string path = Path.Combine(Paths.SettingsPath, "Rappen.XTB.Tools.xml");
            var result = new RappenXTBTools();
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(path);
                    result = (RappenXTBTools)XmlSerializerHelper.Deserialize(xmlDocument.OuterXml, typeof(RappenXTBTools));
                }
                catch { }
            }
            if (result.InstallationId.Equals(Guid.Empty))
            {
                result.InstallationId = InstallationInfo.Instance.InstallationId;
            }
            return result;
        }

        public void Save()
        {
            if (!Directory.Exists(Paths.SettingsPath))
            {
                Directory.CreateDirectory(Paths.SettingsPath);
            }
            string path = Path.Combine(Paths.SettingsPath, "Rappen.XTB.Tools.xml");
            XmlSerializerHelper.SerializeToFile(this, path);
        }

        public Tool this[string name]
        {
            get
            {
                if (!Tools.Any(t => t.ToolName == name))
                {
                    Tools.Add(new Tool { ToolName = name });
                }
                return Tools.FirstOrDefault(t => t.ToolName == name);
            }
        }
    }

    public class Tool
    {
        public string ToolName { get; set; }
        public string Version { get; set; }
        public DateTime OpenedSupportingLastDate { get; set; } = DateTime.MinValue;
        public int OpenSupportingCount { get; set; } = 0;
        public SupportType SupportType { get; set; } = SupportType.None;
        public int UserIndex { get; set; }
        public string UserCount { get; set; }

        public string Amount
        {
            get
            {
                switch (UserIndex)
                {
                    case 1: return "X-Small";
                    case 2: return "Small";
                    case 4: return "Large";
                    case 5: return "X-Large";
                    default: return "Medium";
                }
            }
        }
    }

    public enum SupportType
    {
        None,
        Personal,
        Company
    }
}