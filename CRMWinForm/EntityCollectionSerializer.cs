using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Cinteros.Xrm.CRMWinForm
{
    public class EntityCollectionSerializer
    {
        public static XmlDocument Serialize(EntityCollection collection)
        {
            var result = new XmlDocument();
            XmlNode root = result.CreateNode(XmlNodeType.Element, "Entities", "");
            var entityname = result.CreateAttribute("EntityName");
            entityname.Value = collection.EntityName;
            root.Attributes.Append(entityname);
            var more = result.CreateAttribute("MoreRecords");
            more.Value = collection.MoreRecords.ToString();
            root.Attributes.Append(more);
            var total = result.CreateAttribute("TotalRecordCount");
            total.Value = collection.TotalRecordCount.ToString();
            root.Attributes.Append(total);
            var paging = result.CreateAttribute("PagingCookie");
            paging.Value = collection.PagingCookie;
            root.Attributes.Append(paging);
            foreach (var entity in collection.Entities)
            {
                EntitySerializer.Serialize(entity, root);
            }
            result.AppendChild(root);
            return result;
        }

        public static string ToJSON(EntityCollection collection, Formatting format)
        {
            var space = format == Formatting.Indented ? " " : "";
            StringBuilder sb = new StringBuilder();
            sb.Append("{" + EntitySerializer.Sep(format, 1) + "\"entities\":" + space + "[");
            List<string> entities = new List<string>();
            foreach (Entity entity in collection.Entities)
            {
                entities.Add(EntitySerializer.ToJSON(entity, format, 2));
            }
            sb.Append(string.Join(",", entities));
            sb.Append(EntitySerializer.Sep(format, 1) + "]" + EntitySerializer.Sep(format, 0) + "}");
            return sb.ToString();
        }
    }
}
