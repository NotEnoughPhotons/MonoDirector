using Newtonsoft.Json;

namespace NEP.MonoDirector.Downloading
{
    [JsonObject(MemberSerialization.Fields)]
    public class DataResult
    {
        public ModObject[] Data => m_data;

        [JsonProperty("data")] private ModObject[] m_data;
    }
}
