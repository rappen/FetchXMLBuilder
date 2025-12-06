using Rappen.XTB.FetchXmlBuilder.Builder;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.ControlsClasses
{
    public class EntityNode
    {
        #region Public Fields

        public TreeNode Node { get; }
        public string LogicalName { get; }
        public string Alias { get; }

        #endregion Public Fields

        #region Private Fields

        private string name;

        #endregion Private Fields

        #region Public Constructors

        public EntityNode(TreeNode node)
        {
            Node = node;
            LogicalName = node.Value("name");
            Alias = node.Value("alias");
            name = !string.IsNullOrEmpty(Alias) ? Alias : LogicalName;
        }

        #endregion Public Constructors

        #region Public Methods

        public override string ToString() => name;

        #endregion Public Methods
    }
}