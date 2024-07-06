using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace CS2Legs
{
    public class Config : BasePluginConfig
    {
        public override int Version { get; set; } = 1;

        [JsonPropertyName("flag")]
        public string VipFlag { get; set; } = "";


    }
}
