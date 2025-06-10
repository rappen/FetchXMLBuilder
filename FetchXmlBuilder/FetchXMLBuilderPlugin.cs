using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using XrmToolBox.Constants;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Rappen.XTB.FetchXmlBuilder
{
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "FetchXML Builder"),
        ExportMetadata("Description", "Build queries for Microsoft Dataverse and the Power Platform. Investigate data. Fix the layouts. Get code. Empower yourself to achieve more."),
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

        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // Parse the assembly name to get individual components
            var requestedAssemblyName = new AssemblyName(args.Name);
            var assemblyName = requestedAssemblyName.Name;
            var requestedVersion = requestedAssemblyName.Version;

            Console.WriteLine($"Resolving: {assemblyName}, Version: {requestedVersion}");

            // Define your binding redirects programmatically
            var bindingRedirects = new Dictionary<string, BindingRedirect>
    {
        {
            "Microsoft.Bcl.AsyncInterfaces",
            new BindingRedirect
            {
                AssemblyName = "Microsoft.Bcl.AsyncInterfaces",
                PublicKeyToken = "cc7b13ffcd2ddd51",
                OldVersionMin = new Version("0.0.0.0"),
                OldVersionMax = new Version("9.0.0.2"),
                NewVersion = new Version("9.0.0.2")
            }
        },
         {
            "System.Text.Json",
            new BindingRedirect
            {
                AssemblyName = "System.Text.Json",
                PublicKeyToken = "cc7b13ffcd2ddd51",
                OldVersionMin = new Version("0.0.0.0"),
                OldVersionMax = new Version("9.0.0.2"),
                NewVersion = new Version("9.0.0.2")
            }
        },
         {
            "System.Memory.Data",
            new BindingRedirect
            {
                AssemblyName = "System.Memory.Data",
                PublicKeyToken = "cc7b13ffcd2ddd51",
                OldVersionMin = new Version("0.0.0.0"),
                OldVersionMax = new Version("8.0.0.1"),
                NewVersion = new Version("8.0.0.1")
            }
        },
         {
            "System.Diagnostics.DiagnosticSource",
            new BindingRedirect
            {
                AssemblyName = "System.Diagnostics.DiagnosticSource",
                PublicKeyToken = "cc7b13ffcd2ddd51",
                OldVersionMin = new Version("0.0.0.0"),
                OldVersionMax = new Version("8.0.0.1"),
                NewVersion = new Version("8.0.0.1")
            }
        },
         {
            "System.Text.Encodings.Web",
            new BindingRedirect
            {
                AssemblyName = "System.Text.Encodings.Web",
                PublicKeyToken = "cc7b13ffcd2ddd51",
                OldVersionMin = new Version("0.0.0.0"),
                OldVersionMax = new Version("9.0.0.2"),
                NewVersion = new Version("9.0.0.2")
            }
        },
         {
            "System.Buffers",
            new BindingRedirect
            {
                AssemblyName = "System.Buffers",
                PublicKeyToken = "cc7b13ffcd2ddd51",
                OldVersionMin = new Version("0.0.0.0"),
                OldVersionMax = new Version("4.0.3.0"),
                NewVersion = new Version("4.0.3.0")
            }
        }
        // Add more binding redirects as needed
    };

            // Check if this assembly needs a binding redirect
            if (bindingRedirects.ContainsKey(assemblyName))
            {
                var redirect = bindingRedirects[assemblyName];

                // Verify public key token matches (if specified)
                if (!string.IsNullOrEmpty(redirect.PublicKeyToken))
                {
                    var requestedToken = BitConverter.ToString(requestedAssemblyName.GetPublicKeyToken() ?? new byte[0]).Replace("-", "").ToLower();
                    if (requestedToken != redirect.PublicKeyToken.ToLower())
                    {
                        Console.WriteLine($"Public key token mismatch for {assemblyName}");
                        return null;
                    }
                }

                // Check if the requested version falls within the redirect range
                if (requestedVersion >= redirect.OldVersionMin && requestedVersion <= redirect.OldVersionMax)
                {
                    Console.WriteLine($"Applying binding redirect: {assemblyName} {requestedVersion} -> {redirect.NewVersion}");

                    // Try to load the redirected version
                    loadAssembly = TryLoadAssemblyVersion(assemblyName, redirect.NewVersion, redirect.PublicKeyToken);

                    if (loadAssembly != null)
                    {
                        Console.WriteLine($"Successfully loaded redirected assembly: {loadAssembly.FullName}");
                        return loadAssembly;
                    }
                }
            }

            // Fall back to original logic if no binding redirect applies
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == assemblyName).FirstOrDefault();

            if (refAssembly != null)
            {
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);
                var assmbPath = Path.Combine(dir, $"{assemblyName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                    Console.WriteLine($"Loaded from fallback path: {assmbPath}");
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }

        private Assembly TryLoadAssemblyVersion(string assemblyName, Version targetVersion, string publicKeyToken)
        {
            try
            {
                // Method 1: Try to load from GAC or current context first
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var existingAssembly = assemblies.FirstOrDefault(a =>
                    a.GetName().Name == assemblyName &&
                    a.GetName().Version >= targetVersion);

                if (existingAssembly != null)
                {
                    return existingAssembly;
                }

                // Method 2: Try to construct full assembly name and load
                var fullName = $"{assemblyName}, Version={targetVersion}, Culture=neutral";
                if (!string.IsNullOrEmpty(publicKeyToken))
                {
                    fullName += $", PublicKeyToken={publicKeyToken}";
                }

                try
                {
                    return Assembly.Load(fullName);
                }
                catch (FileNotFoundException)
                {
                    // Method 3: Try loading from application directory
                    Assembly currAssembly = Assembly.GetExecutingAssembly();
                    string dir = Path.GetDirectoryName(currAssembly.Location);
                    string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                    dir = Path.Combine(dir, folder);
                    var assmbPath = Path.Combine(dir, $"{assemblyName}.dll");

                    if (File.Exists(assmbPath))
                    {
                        var loadedAssembly = Assembly.LoadFrom(assmbPath);

                        // Verify the loaded assembly version is compatible
                        var loadedVersion = loadedAssembly.GetName().Version;
                        if (loadedVersion >= targetVersion)
                        {
                            return loadedAssembly;
                        }
                    }

                    // Method 4: Try common framework locations
                    var frameworkPaths = new[]
                    {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "shared"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet", "shared"),
                RuntimeEnvironment.GetRuntimeDirectory()
            };

                    foreach (var frameworkPath in frameworkPaths.Where(Directory.Exists))
                    {
                        var potentialPaths = Directory.GetFiles(frameworkPath, $"{assemblyName}.dll", SearchOption.AllDirectories);
                        foreach (var path in potentialPaths)
                        {
                            try
                            {
                                var testAssembly = Assembly.LoadFrom(path);
                                if (testAssembly.GetName().Version >= targetVersion)
                                {
                                    return testAssembly;
                                }
                            }
                            catch
                            {
                                // Continue searching
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assembly {assemblyName} version {targetVersion}: {ex.Message}");
            }

            return null;
        }

        // Helper class to define binding redirects
        public class BindingRedirect
        {
            public string AssemblyName { get; set; }
            public string PublicKeyToken { get; set; }
            public Version OldVersionMin { get; set; }
            public Version OldVersionMax { get; set; }
            public Version NewVersion { get; set; }
        }

    }
}