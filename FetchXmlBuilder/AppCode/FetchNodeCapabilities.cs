using Cinteros.Xrm.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class FetchNodeCapabilities : TreeNodeCapabilities
    {
        public FetchNodeCapabilities(TreeNode node)
            : base(node)
        {
            switch (Name)
            {
                case "fetch":
                    ChildTypes.Add(new ChildNodeCapabilities("entity", false));
                    break;
                case "entity":
                case "link-entity":
                    Delete = true;
                    ChildTypes.Add(new ChildNodeCapabilities("all-attributes", false));
                    ChildTypes.Add(new ChildNodeCapabilities("attribute", true));
                    ChildTypes.Add(new ChildNodeCapabilities("filter", false));
                    ChildTypes.Add(new ChildNodeCapabilities("order", true));
                    ChildTypes.Add(new ChildNodeCapabilities("link-entity", true));
                    break;
                case "all-attributes":
                case "attribute":
                case "order":
                    Delete = true;
                    break;
                case "filter":
                    Delete = true;
                    ChildTypes.Add(new ChildNodeCapabilities("condition", true));
                    ChildTypes.Add(new ChildNodeCapabilities("filter", true));
                    break;
                case "condition":
                    Delete = true;
                    ChildTypes.Add(new ChildNodeCapabilities("value", true));
                    break;
            }
        }
    }
}
