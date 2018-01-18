using System.Runtime.Serialization;

[DataContract]
public class AiProperties
{
    [DataMember(Name = "extra1")]
    public string Extra1 { get; set; }
    [DataMember(Name = "extra2")]
    public string Extra2 { get; set; }
    [DataMember(Name = "extra3")]
    public int Extra3 { get; set; }
}