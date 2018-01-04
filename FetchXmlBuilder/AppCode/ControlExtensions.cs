using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class ControlExtensions
    {
        #region Public Methods

        public static void GroupBoxSetState(this Label link, ToolTip tt = null, bool? expanded = null)
        {
            if (expanded == true)
            {
                link.GroupBoxExpand(tt);
            }
            else if (expanded == false)
            {
                link.GroupBoxCollapse(tt);
            }
            else
            {
                link.GroupBoxToggle(tt);
            }
        }

        public static bool IsExpanded(this GroupBox gb)
        {
            return gb.GetDockedContainer().Height > 20;
        }

        public static void StoreMaxHeights(this Control control, Type controltype)
        {
            foreach (GroupBox gb in control.GetAll(controltype))
            {
                gb.GetDockedContainer().SetMaxHeigh();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static IEnumerable<Control> GetAll(this Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        private static Control GetDockedContainer(this Control control)
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

        private static void GroupBoxCollapse(this Label link, ToolTip tt = null)
        {
            // ↑↓–+˄˅
            link.GetDockedContainer().Height = 18;
            link.Text = "+";
            tt?.SetToolTip(link, "Open");
        }

        private static void GroupBoxExpand(this Label link, ToolTip tt = null)
        {
            var container = link.GetDockedContainer();
            container.Height = container.MaximumSize.Height;
            link.Text = "–";
            tt?.SetToolTip(link, "Close");
        }

        private static void GroupBoxToggle(this Label link, ToolTip tt = null)
        {
            if (link.GetDockedContainer().Height > 20)
            {
                link.GroupBoxCollapse(tt);
            }
            else
            {
                link.GroupBoxExpand(tt);
            }
        }

        private static void SetMaxHeigh(this Control control)
        {
            var maxH = control.MaximumSize.Height == 0 ? control.Height : control.MaximumSize.Height;
            var maxW = control.MaximumSize.Width == 0 ? 10000 : control.MaximumSize.Width;
            control.MaximumSize = new Size(maxW, maxH);
        }

        #endregion Private Methods
    }
}