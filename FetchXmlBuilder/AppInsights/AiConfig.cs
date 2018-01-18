using System;
using System.Reflection;

public class AiConfig
{
    public string DeviceType { get; set; } = Assembly.GetEntryAssembly().GetName().Name;
    public Version OSVersion { get; set; } = Assembly.GetEntryAssembly().GetName().Version;
    public string OperationName { get; set; } = Assembly.GetExecutingAssembly().GetName().Name;
    public Version ApplicationVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version;
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public string InstrumentationKey { get; set; } = null;
    public string AiEndpoint { get; set; } = null;
    public bool LogTraces { get; set; } = true;
    public bool LogMetrics { get; set; } = true;
    public bool LogEvents { get; set; } = true;
    public bool LogExceptions { get; set; } = true;

    public AiConfig() { }

    public AiConfig(string endpoint, string ikey)
    {
        AiEndpoint = endpoint;
        InstrumentationKey = ikey;
    }
}