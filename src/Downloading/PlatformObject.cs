using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace NEP.MonoDirector.Downloading
{
    [JsonObject(MemberSerialization.Fields)]
    public sealed class PlatformObject
    {
        public string Platform => m_platform;
        public int FileID => m_fileID;

        [JsonProperty("platform")] private string m_platform;
        [JsonProperty("modfile_live")] private int m_fileID;
    }
}
