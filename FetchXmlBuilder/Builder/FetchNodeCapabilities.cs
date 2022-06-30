using Cinteros.Xrm.XmlEditorUtils;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class FetchNodeCapabilities
    {
        public string Name;
        public bool Multiple = true;
        public bool Delete = true;
        public bool Attributes = false;
        public bool Comment = true;
        public bool Uncomment = false;
        public List<FetchNodeCapabilities> ChildTypes;

        public FetchNodeCapabilities(string name, bool addchildren) : this(name)
        {
            if (addchildren)
            {
                AddChildren();
            }
        }

        private FetchNodeCapabilities(string name)
        {
            Name = name;
            switch (Name)
            {
                case "fetch":
                    Multiple = false;
                    Delete = false;
                    Comment = false;
                    break;
                case "entity":
                    Multiple = false;
                    Attributes = true;
                    break;
                case "link-entity":
                    Attributes = true;
                    break;
                case "all-attributes":
                    Multiple = false;
                    break;
                case "attribute":
                case "order":
                case "filter":
                case "condition":
                case "value":
                    break;
                case "#comment":
                    Comment = false;
                    Uncomment = true;
                    break;
            }
        }

        private void AddChildren()
        {
            ChildTypes = new List<FetchNodeCapabilities>();
            switch (Name)
            {
                case "fetch":
                    ChildTypes.Add(new FetchNodeCapabilities("entity"));
                    ChildTypes.Add(new FetchNodeCapabilities("-"));
                    ChildTypes.Add(new FetchNodeCapabilities("#comment"));
                    break;
                case "entity":
                case "link-entity":
                    ChildTypes.Add(new FetchNodeCapabilities("-"));
                    ChildTypes.Add(new FetchNodeCapabilities("all-attributes"));
                    ChildTypes.Add(new FetchNodeCapabilities("attribute"));
                    ChildTypes.Add(new FetchNodeCapabilities("filter"));
                    ChildTypes.Add(new FetchNodeCapabilities("order"));
                    ChildTypes.Add(new FetchNodeCapabilities("link-entity"));
                    ChildTypes.Add(new FetchNodeCapabilities("-"));
                    ChildTypes.Add(new FetchNodeCapabilities("#comment"));
                    break;
                case "all-attributes":
                case "attribute":
                case "order":
                    ChildTypes.Add(new FetchNodeCapabilities("#comment"));
                    break;
                case "filter":
                    ChildTypes.Add(new FetchNodeCapabilities("condition"));
                    ChildTypes.Add(new FetchNodeCapabilities("filter"));
                    ChildTypes.Add(new FetchNodeCapabilities("-"));
                    ChildTypes.Add(new FetchNodeCapabilities("#comment"));
                    break;
                case "condition":
                    ChildTypes.Add(new FetchNodeCapabilities("value"));
                    ChildTypes.Add(new FetchNodeCapabilities("-"));
                    ChildTypes.Add(new FetchNodeCapabilities("#comment"));
                    break;
                case "value":
                    ChildTypes.Add(new FetchNodeCapabilities("#comment"));
                    break;
                case "#comment":
                    break;
            }
        }

        public int IndexOfChild(string name)
        {
            var index = 0;
            while (index < ChildTypes.Count && ChildTypes[index].Name != name)
            {
                index++;
            }
            if (index >= ChildTypes.Count)
            {
                index = -1;
            }
            return index;
        }

        public override string ToString()
        {
            return Name + " (" + (ChildTypes != null ? ChildTypes.Count.ToString() : "?") + ")";
        }
    }
}