using Rappen.AI.WinForm;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    /// <summary>
    /// Modal dialog that runs the GitHub device-code sign-in flow. On success,
    /// <see cref="GitHubToken"/> holds the long-lived GitHub OAuth token.
    /// </summary>
    public class GitHubDeviceAuthForm : Form
    {
        private readonly Label lblInfo;
        private readonly TextBox txtUserCode;
        private readonly Button btnCopy;
        private readonly Button btnOpen;
        private readonly Label lblStatus;
        private readonly Button btnCancel;
        private readonly BackgroundWorker worker;

        private GitHubCopilotAuth.DeviceCodeInfo device;

        /// <summary>The GitHub OAuth token obtained on success, otherwise null.</summary>
        public string GitHubToken { get; private set; }

        public GitHubDeviceAuthForm()
        {
            Text = "Sign in to GitHub Copilot";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(420, 240);

            lblInfo = new Label
            {
                Location = new Point(16, 16),
                Size = new Size(388, 40),
                Text = "Contacting GitHub..."
            };

            txtUserCode = new TextBox
            {
                Location = new Point(16, 62),
                Size = new Size(180, 30),
                Font = new Font("Consolas", 16, FontStyle.Bold),
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Center
            };

            btnCopy = new Button
            {
                Location = new Point(206, 62),
                Size = new Size(90, 30),
                Text = "Copy",
                Enabled = false
            };
            btnCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(txtUserCode.Text))
                {
                    Clipboard.SetText(txtUserCode.Text);
                }
            };

            btnOpen = new Button
            {
                Location = new Point(304, 62),
                Size = new Size(100, 30),
                Text = "Open GitHub",
                Enabled = false
            };
            btnOpen.Click += (s, e) => OpenVerificationUri();

            lblStatus = new Label
            {
                Location = new Point(16, 110),
                Size = new Size(388, 76),
                Text = string.Empty
            };

            btnCancel = new Button
            {
                Location = new Point(304, 198),
                Size = new Size(100, 28),
                Text = "Cancel",
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += (s, e) =>
            {
                if (worker.IsBusy)
                {
                    worker.CancelAsync();
                }
            };

            Controls.AddRange(new Control[] { lblInfo, txtUserCode, btnCopy, btnOpen, lblStatus, btnCancel });
            CancelButton = btnCancel;

            worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            device = GitHubCopilotAuth.RequestDeviceCode();
            worker.ReportProgress(0);
            var token = GitHubCopilotAuth.WaitForAccessToken(device, () => worker.CancellationPending);
            if (worker.CancellationPending || string.IsNullOrEmpty(token))
            {
                e.Cancel = true;
                return;
            }
            e.Result = token;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblInfo.Text = "1. Copy the code below.\n2. Open GitHub and paste it to authorize the Copilot sign-in.";
            txtUserCode.Text = device.UserCode;
            btnCopy.Enabled = true;
            btnOpen.Enabled = true;
            lblStatus.Text = "Waiting for you to authorize in the browser...\nGitHub will show \"Visual Studio Code\" — that's expected; it's the Copilot sign-in used by FetchXML Builder.";
            OpenVerificationUri();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            if (e.Error != null)
            {
                lblStatus.Text = "Sign-in failed.";
                MessageBox.Show(this, e.Error.Message, "GitHub Copilot sign-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            GitHubToken = e.Result as string;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OpenVerificationUri()
        {
            if (device != null && !string.IsNullOrEmpty(device.VerificationUri))
            {
                try
                {
                    Process.Start(device.VerificationUri);
                }
                catch
                {
                    // Ignore; the user can open the URL manually from the label.
                }
            }
        }
    }
}
