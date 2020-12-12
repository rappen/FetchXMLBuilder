using System;

namespace XrmToolBox.Constants
{
    // Guids of tools, to be used when referencing tool A from tool B in integration scenarios.
    // Feel free to submit your own tool ids here, to help grow integration possibilities!
    //
    // Original file found at: https://gist.github.com/rappen/eeb2a9633de39fe5a0bfcf74643d777d

    public static class XrmToolBoxToolIds
    {
        public static Guid FetchXMLBuilder = Guid.Parse("46657463-6858-4D4C-2042-75696C646572");
        public static Guid BulkDataUpdater = Guid.Parse("42756C6B-2044-6174-6120-557064617465");
        public static Guid PluginTraceViewer = Guid.Parse("506C7567-696E-2054-7261-636520566965");
    }
}
