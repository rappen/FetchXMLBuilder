using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace FXBTests.Metadata
{
    [EntityLogicalName("account")]
    class Account
    {
        [AttributeLogicalName("accountid")]
        public Guid Id { get; set; }

        [AttributeLogicalName("accountid")]
        public Guid AccountId { get; set; }

        [AttributeLogicalName("name")]
        public string Name { get; set; }
    }
}