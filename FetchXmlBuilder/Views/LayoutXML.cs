using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.Views
{
    public class LayoutXML
    {
        public string EntityName;
        public EntityMetadata EntityMeta;
        public List<Cell> Cells;
        private int cellindex = 0;

        public LayoutXML(string layoutxml, TreeNode entity, FetchXmlBuilder fxb)
        {
            if (!string.IsNullOrEmpty(layoutxml))
            {
                var doc = new XmlDocument();
                doc.LoadXml(layoutxml);
                if (doc.SelectSingleNode("grid") is XmlElement grid &&
                    int.TryParse(grid.AttributeValue("object"), out int entityid) &&
                    fxb.GetEntity(entityid) is EntityMetadata entitymeta)
                {
                    EntityMeta = entitymeta;
                    Cells = grid.SelectSingleNode("row")?
                        .ChildNodes.Cast<XmlNode>()
                        .Where(n => n.Name == "cell")
                        .Select(c => new Cell(this, c, cellindex++)).ToList();
                }
            }
            EntityName = EntityMeta?.LogicalName ?? entity.Value("name");
            if (EntityMeta == null)
            {
                EntityMeta = fxb.GetEntity(EntityName);
            }
            var attributes = fxb.dockControlBuilder.GetAllLayoutValidAttributes();
            AdjustAllCells(attributes);
        }

        public void AdjustAllCells(IEnumerable<TreeNode> attributes)
        {
            if (Cells == null)
            {
                Cells = new List<Cell>();
            }
            attributes.Where(a => GetCell(a) == null).ToList().ForEach(a => AddCell(a));
            Cells.Where(c => c.Attribute == null).ToList().ForEach(c => c.Attribute = attributes.FirstOrDefault(a => c.Name == a.GetAttributeLayoutName()));
            Cells.Where(c => c.Attribute == null).ToList().ForEach(c => Cells.Remove(c));
        }

        public string ToXML()
        {
            var result = $@"<grid name='resultset' object='{EntityMeta.ObjectTypeCode}' jump='{EntityMeta.PrimaryNameAttribute}' select='1' icon='1' preview='1'>
  <row name='result' id='{EntityMeta.PrimaryIdAttribute}'>
    {string.Join("\n    ", Cells?.Where(c => c.Width > 0).OrderBy(c => c.Index).Select(c => c.ToXML()))}
  </row>
</grid>";
            return result;
        }

        public Cell GetCell(TreeNode node)
        {
            var result = Cells.FirstOrDefault(c => c.Attribute == node);
            if (result == null)
            {
                result = Cells.FirstOrDefault(c => c.Name == node.GetAttributeLayoutName());
                if (result != null)
                {
                    result.Attribute = node;
                }
            }
            return result;
        }

        public Cell AddCell(TreeNode attribute)
        {
            if (EntityMeta?.Attributes?.Any(a => a.LogicalName.Equals(attribute.Value("name"))) != true)
            {
                return null;
            }
            var cell = new Cell(this, attribute, cellindex++);
            Cells.Add(cell);
            return cell;
        }
    }
}