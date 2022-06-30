using Rappen.XTB.FetchXmlBuilder.Builder;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.ControlsClasses
{
    public class EntityNode
    {
        #region Public Fields

        public string EntityName;

        #endregion Public Fields

        #region Private Fields

        private string name;

        #endregion Private Fields

        #region Public Constructors

        public EntityNode(TreeNode node)
        {
            EntityName = node.Value("name");
            var alias = node.Value("alias");
            name = !string.IsNullOrEmpty(alias) ? alias : EntityName;
        }

        #endregion Public Constructors

        #region Public Methods

        public override string ToString()
        {
            return name;
        }

        #endregion Public Methods
    }
}