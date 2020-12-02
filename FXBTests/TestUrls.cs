using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FXBTests
{
    [TestClass]
    public class TestUrls
    {
        private const string docsUrl = "https://docs.microsoft.com/en-us/powerapps/developer/model-driven-apps/developer-tools";
        private const string docsUrlResult = "https://docs.microsoft.com/en-us/powerapps/developer/model-driven-apps/developer-tools?WT.mc_id=BA-MVP-5002475";
        private const string docsUrlWithAnchor = "https://docs.microsoft.com/en-us/powerapps/developer/model-driven-apps/clientapi/client-scripting-best-practices#write-your-code-for-multiple-browsers";
        private const string docsUrlWithAnchorResult = "https://docs.microsoft.com/en-us/powerapps/developer/model-driven-apps/clientapi/client-scripting-best-practices?WT.mc_id=BA-MVP-5002475#write-your-code-for-multiple-browsers";
        private const string docsUrlWithQueryAndAnchor = "https://docs.microsoft.com/en-us/powerapps/developer/model-driven-apps/clientapi/client-scripting-best-practices?hey=ho#write-your-code-for-multiple-browsers";
        private const string docsUrlWithQueryAndAnchorResult = "https://docs.microsoft.com/en-us/powerapps/developer/model-driven-apps/clientapi/client-scripting-best-practices?hey=ho&WT.mc_id=BA-MVP-5002475#write-your-code-for-multiple-browsers";
        private const string randomUrl = "https://fetchxmlbuilder.com";
        private const string randomUrlResult = "https://fetchxmlbuilder.com";

        [TestMethod]
        public void TestDocsUrl()
        {
            var result = Utils.ProcessURL(docsUrl);
            Assert.AreEqual(result, docsUrlResult);
        }

        [TestMethod]
        public void TestDocsUrlWithAnchor()
        {
            var result = Utils.ProcessURL(docsUrlWithAnchor);
            Assert.AreEqual(result, docsUrlWithAnchorResult);
        }

        [TestMethod]
        public void TestDocsUrlWithQueryAndAnchor()
        {
            var result = Utils.ProcessURL(docsUrlWithQueryAndAnchor);
            Assert.AreEqual(result, docsUrlWithQueryAndAnchorResult);
        }

        [TestMethod]
        public void TestRandomUrl()
        {
            var result = Utils.ProcessURL(randomUrl);
            Assert.AreEqual(result, randomUrlResult);
        }
    }
}
