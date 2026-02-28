using Newtonsoft.Json;

namespace NEP.MonoDirector.Downloading
{
    [JsonObject(MemberSerialization.Fields)]
    public sealed class ModObject
    {
        public string Version => m_version;
        public PlatformObject[] Platforms => m_platforms;
        
        [JsonProperty("version")] private string m_version;
        [JsonProperty("platforms")] private PlatformObject[] m_platforms;
    }
}
