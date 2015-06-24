using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.XmlEditorUtils
{
    public abstract class TreeNodeCapabilities
    {
        public string Name = "";
        public bool Delete = false;
        public bool Comment = true;
        public bool Uncomment = false;
        public List<ChildNodeCapabilities> ChildTypes = new List<ChildNodeCapabilities>();

        public TreeNodeCapabilities() { }

        protected TreeNodeCapabilities(TreeNode node)
        {
            Name = node.Name;
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

    public class ChildNodeCapabilities
    {
        public string Name;
        public bool Multiple;

        public ChildNodeCapabilities(string name, bool multiple)
        {
            Name = name;
            Multiple = multiple;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
