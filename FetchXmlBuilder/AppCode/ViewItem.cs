using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;

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

        public bool IsCustomizable => view.IsCustomizable();

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

    public static class EntityExtensions
    {
        public static bool IsCustomizable(this Entity entity)
        {
            return entity != null
                && (entity.LogicalName == "userquery"
                || (entity.Contains("iscustomizable")
                    && entity["iscustomizable"] is BooleanManagedProperty iscust
                    && iscust.Value));
        }
    }
}