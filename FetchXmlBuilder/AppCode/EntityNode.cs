using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
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
            EntityName = TreeNodeHelper.GetAttributeFromNode(node, "name");
            var alias = TreeNodeHelper.GetAttributeFromNode(node, "alias");
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