using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.ControlsClasses
{
    internal class AttributeItem : IComboBoxItem
    {
        private AttributeMetadata meta = null;

        public AttributeMetadata Metadata
        {
            get { return meta; }
            set { meta = value; }
        }

        public AttributeItem(AttributeMetadata Attribute)
        {
            meta = Attribute;
        }

        public override string ToString()
        {
            var result = FetchXmlBuilder.GetAttributeDisplayName(meta);
            //if (meta.IsValidForRead != true) result += " NoRead";
            //if (meta.IsManaged == true) result += " MGD";
            if (meta.IsPrimaryId == true) result += " (id)";
            //if (meta.IsPrimaryName == true) result += " PN";
            //if (meta.IsSecured == true) result += " Sec";
            //if (meta.IsValidForAdvancedFind.Value) result += " AF";
            //if (meta.AttributeType != null) result += " " + meta.AttributeType.ToString();
            return result;
        }

        public string GetValue()
        {
            return meta.LogicalName;
        }

        public static void AddAttributeToComboBox(ComboBox cmb, AttributeMetadata meta, bool allowvirtual, bool friendly)
        {
            var add = false;
            if (!friendly)
            {
                add = true;
            }
            else
            {
                add = meta.DisplayName != null && meta.DisplayName.LocalizedLabels != null && meta.DisplayName.LocalizedLabels.Count > 0;
                if (meta.AttributeType == AttributeTypeCode.Money && meta.LogicalName.EndsWith("_base"))
                {
                    add = false;
                }
            }
            if (!allowvirtual && meta.AttributeType == AttributeTypeCode.Virtual && !(meta is MultiSelectPicklistAttributeMetadata))
            {
                add = false;
            }
            if (add)
            {
                cmb.Items.Add(new AttributeItem(meta));
            }
        }
    }
}