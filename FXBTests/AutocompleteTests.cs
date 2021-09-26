using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinteros.Xrm.FetchXmlBuilder;
using MarkMpn.XmlSchemaAutocomplete;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FXBTests
{
    [TestClass]
    public class AutocompleteTests
    {
        [TestMethod]
        public void SuggestsFetch()
        {
            // If this throws an exception due to a circular reference while building the schema from the
            // classes in fetch.cs, the file has probably been regenerated from the schema file and needs
            // the AnonymousType = true settings removing from the attributes on each class.
            var text = "<";
            var suggestions = new Autocomplete<FetchType>().GetSuggestions(text, out _);
            CollectionAssert.AreEqual(new[] { "fetch" }, suggestions.Cast<AutocompleteElementSuggestion>().Select(s => s.Name).ToArray());
        }
    }
}
