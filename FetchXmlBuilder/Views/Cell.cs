using Rappen.XRM.Helpers.Extensions;
using System.Windows.Forms;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.Views
{
    public class Cell
    {
        public string Name;
        public int Width;
        public bool IsHidden;
        public bool disableSorting;
        public string imageproviderwebresource;
        public string imageproviderfunctionname;
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
            bool.TryParse(cell.AttributeValue("disableSorting").Replace("1", "True").Replace("0", "False"), out disableSorting);
            imageproviderfunctionname = cell.AttributeValue("imageproviderfunctionname");
            imageproviderwebresource = cell.AttributeValue("imageproviderwebresource");
        }

        public override string ToString() => $"{Name} ({(IsHidden ? "Hidden" : Width.ToString())})";

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
            if (disableSorting)
            {
                result += "disableSorting='1' ";
            }
            if (!string.IsNullOrWhiteSpace(imageproviderfunctionname))
            {
                result += $"imageproviderfunctionname='{imageproviderfunctionname}' ";
            }
            if (!string.IsNullOrWhiteSpace(imageproviderwebresource))
            {
                result += $"imageproviderwebresource='{imageproviderwebresource}' ";
            }
            result += "/>";
            return result;
        }

        public int DisplayIndex => (Parent?.Cells.IndexOf(this) ?? 0) + 1;
    }
}