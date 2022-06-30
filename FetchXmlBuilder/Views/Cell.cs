using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using System.Windows.Forms;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.Views
{
    public class Cell
    {
        public string Name;
        public int Width;
        public int Index;
        public TreeNode Attribute;
        public LayoutXML Parent;

        internal Cell(LayoutXML layoutxml, TreeNode attribute, int index)
        {
            Parent = layoutxml;
            Attribute = attribute;
            Index = index;
            Name = attribute.GetAttributeLayoutName();
            Width = attribute.Value("name") == Parent.EntityMeta?.PrimaryIdAttribute ? 0 : 100;
        }

        internal Cell(LayoutXML layoutxml, XmlNode cell, int index)
        {
            Parent = layoutxml;
            Name = cell.AttributeValue("name");
            if (int.TryParse(cell.AttributeValue("width"), out int width))
            {
                Width = width;
            }
            else
            {
                Width = 100;
            }
            Index = index;
        }

        public string ToXML()
        {
            return $"<cell name='{Name}' width='{Width}' />";
        }
    }
}