namespace Rappen.XTB.LCG
{
    public class CommonSettings
    {
        public CommonSettings()
        {
        }

        internal string InlineConfigBegin = @"/***** LCG-configuration-BEGIN *****\";
        internal string InlineConfigEnd = @"\***** LCG-configuration-END   *****/";
        public string[] CamelCaseWords { get; set; } = new string[] { "parent", "customer", "owner", "state", "status", "name", "phone", "address", "code", "postal", "mail", "modified", "created", "permission", "type", "method", "verson", "number", "first", "last", "middle", "contact", "account", "system", "user", "fullname", "preferred", "processing", "annual", "plugin", "step", "key", "details", "message", "description", "constructor", "execution", "secure", "configuration", "behalf", "count", "percent", "internal", "external", "trace", "entity", "primary", "secondary", "lastused", "credit", "credited", "donot", "exchange", "import", "invoke", "invoked", "private", "market", "marketing", "revenue", "business", "price", "level", "pricelevel", "territory", "version", "conversion", "workorder", "team" };
        public string[] CamelCaseWordEnds { get; set; } = new string[] { "id" };
    }
}