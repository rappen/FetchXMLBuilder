﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Constants;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Rappen.XTB.FetchXmlBuilder
{
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "FetchXML Builder"),
        ExportMetadata("Description", "Build queries for Microsoft Dataverse. Run them. Get code. Let AI fix what you can't. Empower yourself to achieve more."),
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAFXRFWHRDcmVhdGlvbiBUaW1lAAfjAwoIIBLJNr4aAAAAB3RJTUUH4wMKCQIj65lxRwAAAAlwSFlzAAAK8AAACvABQqw0mAAAAPxQTFRF//+93ue9rca9jKW1e5y1hKW1pb291t69IVqtAEKtAEq9c5S17/e9a5S1AEK1AFLWAFrnAGP3CGv3AGPvWoS1vc69GFKtAFreEEqtlK21AFLOc5y1EGvvGHPnEHPvnLW9vdZC//8A9/cIOYTG3u8hIXPeCEqt1uchpcZaSnu15++9hLV7lL1rUpStxt45xta9IXvext4xOYzG5+8Y7/cIKXvWKWO1rc5SWpyl7/cQ1ucpY6WcjL1rCFrW9/+99/8AhLVze62EjL1ze617nMZjSpS1zt4xjK21Qoy93uchAErGc62Ma6WUzucxMYTOtdZKCGvvUoS1Y4y1QnO1yZjMPgAAAAF0Uk5TAEDm2GYAAAIKSURBVHjabVNre5owGEWLl6QKErWigiN0pa6ztB0yCmvtdF6ptSv9//9lbxKoOHu+wJNznpz3ciJJe0xuuv0kOe22J9JnqCQ6UnAcYwU5rZsjuvCGcKdJOLQORu+VQ76s44ZG9mhiZ5XnS6iXp9ktKkomOb5DjtBAScYXOb9b7zLOPD/nitTlSu+xU4vSe8EbG0pN+HYcUWmiaKkgNLlgSIVAw++8wbSALRwH7Ocafu5EL6gNgr64gBivlJuYHqX2wgf8JfEbCKrq3PO80KYLj5swg19QBcjmj04NHJpzGsxmo/lu/QQmzGBIorHvr1/pH/AoIm1O16L8KZAgigyy9FjjNFBW0koho0zAOoGbYR5uOCAXD3Qb1yUZkxmN3JeNBYJbGwTbTEmXJG6lAstdshZ9Jpgygf28fbboFAQHFktuAXN2WQ1kvAELXuRC8DNGU3qZFmnaERR5hRpDek2yEY7uRZtPv6ezCNqEaP3sBWJLfsg7DNigojAM7fDhUYdMlBTz1mCCO2B+iFGPfeGJW2zbjljWd2aQOblpZnh46x/rvjQ+1n3B1/2N56F2xgNp8f4YBl+FQHUKIlJtpLJ1m0YWOcMcsEChL1kou0j9L9Sc7+9j3z2OfQ/J+YfRPlM6OYnWwHr58GnV6khRxePSmipGrcLx65SrCGEAQtXTivQpCsUTWS6VD9h/FCldZUhWaJEAAAAASUVORK5CYII="),
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "#FFFFC0"),
        ExportMetadata("PrimaryFontColor", "#0000C0"),
        ExportMetadata("SecondaryFontColor", "#0000FF")]
    public partial class FetchXMLBuilderPlugin : PluginBase, IPayPalPlugin
    {
        public string DonationDescription => "FetchXML Builder Fan Club";

        public string EmailAccount => "jonas@rappen.net";

        public override IXrmToolBoxPluginControl GetControl() => new FetchXmlBuilder();

        public override Guid GetId() => XrmToolBoxToolIds.FetchXMLBuilder;

        public FetchXMLBuilderPlugin()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        private readonly HashSet<string> _redirectAssemblies = new HashSet<string>
        {
            "Microsoft.Bcl.AsyncInterfaces",
            "System.Text.Json",
            "System.Memory.Data",
            "System.Diagnostics.DiagnosticSource",
            "System.Text.Encodings.Web",
            "System.Buffers",
            "System.ClientModel",
            "System.IO.Pipelines"
        };

        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null || _redirectAssemblies.Contains(argName))
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}