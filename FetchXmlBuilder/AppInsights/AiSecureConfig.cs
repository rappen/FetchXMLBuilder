using System.Runtime.Serialization;

/*
 {
 "InstrumentationKey": "Your Key",
 "AiEndpoint": "https://dc.services.visualstudio.com/v2/track",
 "LogTraces": true,
 "LogMetrics": true,
 "LogEvents": true,
 "LogExceptions": true
}
*/


[DataContract]
public class AiSecureConfig
{
    [DataMember]
    public string InstrumentationKey { get; set; }
    [DataMember]
    public string AiEndpoint { get; set; }
    [DataMember]
    public bool LogTraces { get; set; }
    [DataMember]
    public bool LogMetrics { get; set; }
    [DataMember]
    public bool LogEvents { get; set; }
    [DataMember]
    public bool LogExceptions { get; set; }
}
