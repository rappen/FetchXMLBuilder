using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Forms;

namespace Cinteros.Xrm.XmlEditorUtils
{
    public static class VersionCheck
    {
        public static Task LaunchVersionCheck(string currentVersion, string ghUser, string ghRepo, string dlUrl, DateTime lastCheck, UserControl sender)
        {
            return new Task(() =>
            {
                var cvc = new XrmToolBox.AppCode.GithubVersionChecker(currentVersion, ghUser, ghRepo);

                cvc.Run();

                if (cvc.Cpi != null && !string.IsNullOrEmpty(cvc.Cpi.Version))
                {
                    if (lastCheck != DateTime.Now.Date)
                    {
                        sender.Invoke(new Action(() =>
                        {
                            var nvForm = new NewVersionForm(currentVersion, cvc.Cpi.Version, cvc.Cpi.Description, ghUser, ghRepo, new Uri(string.Format(dlUrl, currentVersion)));
                            nvForm.ShowDialog(sender);
                        }));
                    }
                }
            });
        }
    }
}
