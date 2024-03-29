namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchType
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string datasource;
    }

    public partial class filter
    {
        [System.Xml.Serialization.XmlAttributeAttribute("link-type")]
        public string linktype;
    }
}