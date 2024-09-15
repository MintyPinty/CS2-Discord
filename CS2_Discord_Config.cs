using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using System.Text.Json.Serialization;

namespace CS2_Discord2CS;

[MinimumApiVersion(228)]

public class CS2_Discord_Config : BasePluginConfig
{
    [JsonPropertyName("prefix")] public string Prefix { get; set; } = "{Default}[{DarkBlue}DISCORD{Default}]";
    [JsonPropertyName("webhook_url")] public string webhook_url { get; set; } = "https://discord.com/api/webhooks/";
    
}