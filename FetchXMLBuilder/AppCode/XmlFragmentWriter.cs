using System.IO;
using System.Xml;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
	/// <summary>
	/// XmlFragmentWriter is needed as only XmlTextWriter has the ability to specify
	/// the QuoteChar, but unfortunately does not have a way to skip the xmldeclaration
	/// which XMLWriter can via its XmlWriterSettings object.
	/// see: http://www.hanselman.com/blog/XmlFragmentWriterOmitingTheXmlDeclarationAndTheXSDAndXSINamespaces.aspx
	/// </summary>
	public class XmlFragmentWriter : XmlTextWriter
	{
		public XmlFragmentWriter(StringWriter stringWriter) : base(stringWriter)
		{
		}

		public override void WriteStartDocument()
		{
			//Do nothing so we omit the xml declaration.
		}
	}
}