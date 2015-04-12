﻿using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public class FXBMessageBusArgument
    {
        /// <summary>Defines what is requested to be returned</summary>
        public FXBMessageBusRequest Request { get; set; }

        /// <summary>FetchXML to initiate FXB with, and returned if Request is FetchXML</summary>
        public string FetchXML { get; set; }

        /// <summary>QueryExpression to be returned if Request is QueryExpression</summary>
        public QueryExpression QueryExpression { get; set; }

        /// <summary>OData query to be returned if Request is OData</summary>
        public string OData { get; set; }

        /// <summary>Constructor for the FXBMessageBusArgument class</summary>
        /// <param name="Request">Requested type to return from FXB</param>
        public FXBMessageBusArgument(FXBMessageBusRequest Request)
        {
            this.Request = Request;
        }
    }

    public enum FXBMessageBusRequest
    {
        FetchXML,
        QueryExpression,
        OData,
        None
    }
}
