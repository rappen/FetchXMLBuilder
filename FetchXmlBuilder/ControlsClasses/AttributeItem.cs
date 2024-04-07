using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.XmlEditorUtils;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.ControlsClasses
{
    internal class AttributeItem : IComboBoxItem
    {
        private AttributeMetadata meta = null;
        private bool includetypeindisplayname;

        public AttributeMetadata Metadata
        {
            get { return meta; }
            set { meta = value; }
        }

        public AttributeItem(AttributeMetadata Attribute, bool IncludeTypeInDisplayName)
        {
            meta = Attribute;
            includetypeindisplayname = IncludeTypeInDisplayName;
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
            if (includetypeindisplayname) result += $" ({meta.ToTypeName(FetchXmlBuilder.friendlyNames)})";
            return result;
        }

        public string GetValue()
        {
            return meta.LogicalName;
        }

        public static void AddAttributeToComboBox(ComboBox cmb, AttributeMetadata meta, bool includetypeindisplayname, bool allowvirtual, bool friendly)
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
                cmb.Items.Add(new AttributeItem(meta, includetypeindisplayname));
            }
        }
    }
}