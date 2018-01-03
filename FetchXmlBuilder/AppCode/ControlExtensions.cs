using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class ControlExtensions
    {
        public static IEnumerable<Control> GetAll(this Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public static Control GetDockedContainer(this Control control)
        {
            if (control.Dock == DockStyle.None && control.Parent != null)
            {
                return control.Parent.GetDockedContainer();
            }
            else
            {
                return control;
            }
        }
    }
}
