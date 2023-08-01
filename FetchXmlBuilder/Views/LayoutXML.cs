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
        private FetchXmlBuilder fxb;
        private EntityMetadata entitymeta;
        private string entityname;
        private int entityotc;
        private bool createdfromaview = false;

        public EntityMetadata EntityMeta
        {
            get
            {
                if (entitymeta == null)
                {
                    if (entityotc > 0)
                    {
                        entitymeta = fxb.GetEntity(entityotc);
                    }
                    else if (!string.IsNullOrEmpty(entityname))
                    {
                        entitymeta = fxb.GetEntity(entityname);
                    }
                }
                return entitymeta;
            }
        }

        public List<Cell> Cells;

        public LayoutXML(FetchXmlBuilder fxb)
        {
            this.fxb = fxb;
        }

        public LayoutXML(EntityMetadata entity, FetchXmlBuilder fxb)
        {
            this.fxb = fxb;
            entitymeta = entity;
            Cells = new List<Cell>();
            MakeSureAllCellsExistForAttributes();
        }

        public LayoutXML(string layoutxmlstr, FetchXmlBuilder fxb)
        {
            this.fxb = fxb;
            var entity = string.Empty;
            if (!string.IsNullOrEmpty(layoutxmlstr))
            {
                createdfromaview = fxb.View != null;
                var doc = new XmlDocument();
                doc.LoadXml(layoutxmlstr);
                if (doc.SelectSingleNode("grid") is XmlElement grid)
                {
                    if (int.TryParse(grid.AttributeValue("object"), out int entityid))
                    {
                        entityotc = entityid;
                    }
                    Cells = grid.SelectSingleNode("row")?
                        .ChildNodes.Cast<XmlNode>()
                        .Where(n => n.Name == "cell")
                        .Select(c => new Cell(this, c)).ToList();
                }
            }
            if (EntityMeta == null)
            {
                entityname = fxb.dockControlBuilder.RootEntityName;
            }
            if (Cells == null)
            {
                Cells = new List<Cell>();
            }
            MakeSureAllCellsExistForAttributes();
        }

        public override string ToString() => $"{EntityMeta?.LogicalName} {Cells?.Where(c => c.Width > 0).Count()}/{Cells?.Count} cells";

        internal void MakeSureAllCellsExistForAttributes()
        {
            var attributes = fxb.dockControlBuilder.GetAllLayoutValidAttributes();
            if (Cells == null)
            {
                Cells = new List<Cell>();
            }
            // Add missing Cells
            attributes.Where(a => GetCell(a) == null).ToList().ForEach(a => AddCell(a));
            // Update Cells that missing the Attribute
            Cells.Where(c => c.Attribute == null).ToList().ForEach(c => c.Attribute = attributes.FirstOrDefault(a => c.Name == a.GetAttributeLayoutName()));
            // Remove unused Cells
            Cells.Where(c => c.Attribute?.TreeView == null).ToList().ForEach(c => Cells.Remove(c));
        }

        internal void MakeSureAllCellsExistForColumns(Dictionary<string, int> namewidths)
        {
            if (Cells == null)
            {
                Cells = new List<Cell>();
            }
            // Add these missing
            namewidths.Where(n => Cells
                .FirstOrDefault(c => c.Name == n.Key) == null)
                .Where(nw => fxb.dockControlBuilder.GetAttributeNodeFromLayoutName(nw.Key) != null)
                .ToList().ForEach(nw => Cells.Add(new Cell(this)
                {
                    Name = nw.Key,
                    Width = nw.Value,
                    Attribute = fxb.dockControlBuilder.GetAttributeNodeFromLayoutName(nw.Key)
                }));
            Cells.ToList().ForEach(c => c.Width = namewidths.ContainsKey(c.Name) ? namewidths[c.Name] : 0);
            int index = 0;
            foreach (var nw in namewidths)
            {
                var cell = Cells.FirstOrDefault(c => c.Name == nw.Key);
                Cells.Move(cell, index++);
            }
        }

        public string ToXML()
        {
            var result = $@"<grid name='resultset' object='{EntityMeta?.ObjectTypeCode}' jump='{EntityMeta?.PrimaryNameAttribute}' select='1' icon='1' preview='1'>
  <row name='result' id='{EntityMeta?.PrimaryIdAttribute}'>
    {string.Join("\n    ", Cells?.Select(c => c.ToXML()))}
  </row>
</grid>";
            return result;
        }

        public Cell GetCell(TreeNode node)
        {
            var cell = Cells.FirstOrDefault(c => c.Attribute == node);
            if (cell == null)
            {
                cell = Cells.FirstOrDefault(c => c.Name == node.GetAttributeLayoutName());
                if (cell != null)
                {
                    cell.Attribute = node;
                }
            }
            if (cell == null)
            {
                cell = AddCell(node);
            }
            return cell;
        }

        private Cell AddCell(TreeNode attribute)
        {
            if (EntityMeta?.Attributes?.Any(a => a.LogicalName.Equals(attribute.Value("name"))) != true)
            {
                return null;
            }
            var cell = new Cell(this)
            {
                Attribute = attribute,
                Name = attribute.GetAttributeLayoutName(),
                Width = createdfromaview ? 0 : 100
            };
            Cells.Add(cell);
            return cell;
        }
    }
}