using Rappen.XTB.FetchXmlBuilder.Extensions;
using System.Windows.Forms;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.Views
{
    public class Cell
    {
        public string Name;
        public int Width;
        public bool IsHidden;
        public TreeNode Attribute;
        public LayoutXML Parent;

        internal Cell(LayoutXML layoutxml)
        {
            Parent = layoutxml;
        }

        internal Cell(LayoutXML layoutxml, XmlNode cell)
        {
            Parent = layoutxml;
            Name = cell.AttributeValue("name");
            var ishiddenstr = cell.AttributeValue("ishidden").ToLowerInvariant();
            if (!int.TryParse(cell.AttributeValue("width"), out int width))
            {
                width = 100;
            }
            IsHidden = ishiddenstr == "1" || ishiddenstr == "true" || width == 0;
            Width = IsHidden ? 0 : width;
        }

        public override string ToString() => $"{Name} ({Width})";

        public string ToXML()
        {
            var result = $"<cell name='{Name}' ";
            if (IsHidden || Width < 1)
            {
                result += "ishidden='1' ";
            }
            else
            {
                result += $"width='{Width}' ";
            }
            result += "/>";
            return result;
        }

        public int DisplayIndex => (Parent?.Cells.IndexOf(this) ?? 0) + 1;
    }
}