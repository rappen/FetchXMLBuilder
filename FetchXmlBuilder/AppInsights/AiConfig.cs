using System;
using System.Reflection;

public class AiConfig
{
    public string PluginName { get; set; } = GetLastDotPart(Assembly.GetExecutingAssembly().GetName().Name);
    public string PluginVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    public string XTBVersion { get; set; } = GetLastDotPart(Assembly.GetEntryAssembly().GetName().Name) + " " + Assembly.GetEntryAssembly().GetName().Version;
    public string OperationName { get; set; }
    public Guid SessionId { get; } = Guid.NewGuid();
    public string InstrumentationKey { get; }
    public string AiEndpoint { get; }
    public bool LogTraces { get; set; } = true;
    public bool LogMetrics { get; set; } = true;
    public bool LogEvents { get; set; } = true;
    public bool LogExceptions { get; set; } = true;

    public AiConfig(string endpoint, string ikey)
    {
        AiEndpoint = endpoint;
        InstrumentationKey = ikey;
    }

    private static string GetLastDotPart(string identifier)
    {
        return identifier == null ? null : !identifier.Contains(".") ? identifier : identifier.Substring(identifier.LastIndexOf('.') + 1);
    }
}