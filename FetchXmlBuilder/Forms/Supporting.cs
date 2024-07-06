using Rappen.XTB.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Mail;
using System.Reflection;
using System.Windows.Forms;
using XrmToolBox.AppCode.AppInsights;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class Supporting : Form
    {
        private const string tool = "FetchXML%20Builder";
        private const string formidcorp = "wpf17273";
        private const string formidpriv = "wpf17612";
        private const string urlcorp = "https://jonasr.app/supporting/?{formid}_1_first={firstname}&{formid}_1_last={lastname}&{formid}_27={company}&{formid}_3={country}&{formid}_4={email}&{formid}_19={size}&{formid}_24={amount}&{formid}_13={tool}&{formid}_31={tool}&{formid}_32={version}&{formid}_33={instid}";

        private const string urlinde = "https://jonasr.app/supporting/personal/?{formid}_1_first={firstname}&{formid}_1_last={lastname}&{formid}_3={country}&{formid}_4={email}&{formid}_13={tool}&{formid}_31={tool}&{formid}_32={version}&{formid}_33={instid}";
        private Version version;
        private Guid installationid;

        public Supporting(Rappen.XTB.FetchXmlBuilder.FetchXmlBuilder fxb)
        {
            InitializeComponent();
            version = Assembly.GetExecutingAssembly().GetName().Version;
            installationid = InstallationInfo.Instance.InstallationId;
            cmbSize.SelectedIndex = 0;
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
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private string GetUrlCorp()
        {
            var sendit = true;
            var company = txtCompany.Text.Trim().Length >= 4 ? txtCompany.Text.Trim() : "";
            lblCompany.ForeColor = string.IsNullOrEmpty(company) ? Color.DeepPink : Color.Yellow;
            sendit = sendit && company.Length >= 4;

            var email = "";
            try
            {
                email = new MailAddress(txtInvoiceemail.Text).Address;
            }
            catch { }
            lblInvoiceemail.ForeColor = string.IsNullOrEmpty(email) ? Color.DeepPink : Color.Yellow;
            sendit = sendit && email.Length >= 5;

            var amount = "";
            switch (cmbSize.SelectedIndex)
            {
                case 1: amount = "X-Small"; break;
                case 2: amount = "Small"; break;
                case 3: amount = "Medium"; break;
                case 4: amount = "Large"; break;
                case 5: amount = "X-Large"; break;
            }
            lblSize.ForeColor = string.IsNullOrEmpty(amount) ? Color.DeepPink : Color.Yellow;
            sendit = sendit && amount.Length >= 1;

            var country = txtCountry.Text.Trim().Length >= 2 ? txtCountry.Text.Trim() : "";
            lblCountry.ForeColor = string.IsNullOrEmpty(country) ? Color.DeepPink : Color.Yellow;
            sendit = sendit && country.Length >= 2;
            if (sendit)
            {
                return GenerateUrl(urlcorp, formidcorp, "", "", company, email, country, cmbSize.Text, amount);
            }
            return string.Empty;
        }

        private string GetUrlPersonal()
        {
            var sendit = true;

            var first = txtIFirst.Text.Trim().Length >= 1 ? txtIFirst.Text.Trim() : "";
            var last = txtILast.Text.Trim().Length >= 2 ? txtILast.Text.Trim() : "";
            lblIName.ForeColor = string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last) ? Color.DeepPink : Color.Yellow;
            sendit = sendit && first.Length >= 1 && last.Length >= 2;

            var email = "";
            try
            {
                email = new MailAddress(txtIEmail.Text).Address;
            }
            catch { }
            lblIEmail.ForeColor = string.IsNullOrEmpty(email) ? Color.DeepPink : Color.Yellow;
            sendit = sendit && email.Length >= 5;

            var country = txtICountry.Text.Trim().Length >= 2 ? txtICountry.Text.Trim() : "";
            lblICountry.ForeColor = string.IsNullOrEmpty(country) ? Color.DeepPink : Color.Yellow;
            sendit = sendit && country.Length >= 2;

            if (sendit)
            {
                return GenerateUrl(urlinde, formidpriv, first, last, "", email, country, "", "");
            }
            return string.Empty;
        }

        private string GenerateUrl(string template, string form, string first, string last, string company, string email, string country, string size, string amount)
        {
            return template
                .Replace("{formid}", form)
                .Replace("{firstname}", first)
                .Replace("{lastname}", last)
                .Replace("{company}", company)
                .Replace("{country}", country)
                .Replace("{email}", email)
                .Replace("{size}", size)
                .Replace("{amount}", amount)
                .Replace("{tool}", tool)
                .Replace("{version}", version.ToString())
                .Replace("{instid}", installationid.ToString());
        }

        private void later_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }

        private void Supporting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK && DialogResult != DialogResult.Retry)
            {
                e.Cancel = true;
            }
        }

        private void rbI_CheckedChanged(object sender, EventArgs e)
        {
            panPersonal.Left = panCorp.Left;
            panPersonal.Top = panCorp.Top;
            panPersonal.Visible = rbPersonal.Checked;
            panCorp.Visible = !panPersonal.Visible;
            btnSave.Image = rbPersonal.Checked ?
                Properties.Resources.I_Support_Tools_narrow_200 :
                Properties.Resources.Corporate_Supports_Tools_2_narrow_200;
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
    }
}