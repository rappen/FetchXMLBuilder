using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class ViewItem : IComboBoxItem
    {
        private Entity view = null;

        public ViewItem(Entity View)
        {
            view = View;
        }

        public override string ToString()
        {
            return view.Contains("name") ? view["name"].ToString() : view.Contains("listname") ? view["listname"].ToString() : "?";
        }

        public Entity GetView()
        {
            return view;
        }

        public string GetValue()
        {
            return view.Id.ToString();
        }

        public string GetFetch()
        {
            if (view.Contains("fetchxml"))
            {
                return view["fetchxml"].ToString();
            }
            else if (view.Contains("query"))
            {
                return view["query"].ToString();
            }
            return "";
        }
    }
}
