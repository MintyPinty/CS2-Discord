using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace CS2_Discord2CS;
public class CS2_Discord : BasePlugin, IPluginConfig<CS2_Discord_Config>
{
    public required CS2_Discord_Config Config { get; set; }

    public override string ModuleName => "CS2-Discord2CS";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "mintyx";
    public override string ModuleDescription => "Relays messages sent from a discord channel to CS2.";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("CS2-Discord2CS Loaded! -> mintyx");
    }
    public void OnConfigParsed(CS2_Discord_Config config)
    {
        Config = config;
    }

    private Dictionary<int, CCSPlayerController> playerData = new Dictionary<int, CCSPlayerController>();


    [GameEventHandler]
    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;

        ulong playerid = player.SteamID;

        int userid = player.UserId ?? -1;

        if (!playerData.ContainsKey(userid))
        {
            playerData.Add(userid, player);
            Logger.LogInformation($"Player connected: userid = {userid}, CSSPlayerController = {player}");
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;

        int userid = player.UserId ?? -1;

        if (playerData.ContainsKey(userid))
        {
            playerData.Remove(userid);
            Logger.LogInformation($"Player disconnected: userid = {userid}");
        }

        return HookResult.Continue;
    }

    private string ReplaceMessageColors(string input)
    {
        string[] ColorAlphabet = { "{GREEN}", "{BLUE}", "{RED}", "{SILVER}", "{MAGENTA}", "{GOLD}", "{DEFAULT}", "{LIGHTBLUE}", "{LIGHTPURPLE}", "{LIGHTRED}", "{LIGHTYELLOW}", "{YELLOW}", "{GREY}", "{LIME}", "{OLIVE}", "{ORANGE}", "{DARKRED}", "{DARKBLUE}", "{BLUEGREY}", "{PURPLE}" };
        string[] ColorChar = { $"{ChatColors.Green}", $"{ChatColors.Blue}", $"{ChatColors.Red}", $"{ChatColors.Silver}", $"{ChatColors.Magenta}", $"{ChatColors.Gold}", $"{ChatColors.Default}", $"{ChatColors.LightBlue}", $"{ChatColors.LightPurple}", $"{ChatColors.LightRed}", $"{ChatColors.LightYellow}", $"{ChatColors.Yellow}", $"{ChatColors.Grey}", $"{ChatColors.Lime}", $"{ChatColors.Olive}", $"{ChatColors.Orange}", $"{ChatColors.DarkRed}", $"{ChatColors.DarkBlue}", $"{ChatColors.BlueGrey}", $"{ChatColors.Purple}" };

        for (int z = 0; z < ColorAlphabet.Length; z++)
        {
            input = Regex.Replace(input, Regex.Escape(ColorAlphabet[z]), ColorChar[z], RegexOptions.IgnoreCase);
        }
        return input;
    }

    [ConsoleCommand("print", "Prints a message to all chat.")]
    [CommandHelper(minArgs: 1, usage: "[message]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void printCommand(CCSPlayerController? caller, CommandInfo command)
    {
        string input = command.GetCommandString;
        string msg = input.Substring(input.IndexOf(' ') + 1);
        string message = ReplaceMessageColors(msg);

        string prefix = ReplaceMessageColors(Config.Prefix);

        Server.NextFrame(() =>
        {
            Server.PrintToChatAll($"{prefix} {message}");
        });

    }
    public async static Task<string> GetProfilePicture(string steamId64, string defaultImage)
    {
        try
        {
            using var client = new HttpClient();
            string apiUrl = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string xmlResponse = await response.Content.ReadAsStringAsync();
                int startIndex = xmlResponse.IndexOf("<avatarFull><![CDATA[") + "<avatarFull><![CDATA[".Length;
                int endIndex = xmlResponse.IndexOf("]]></avatarFull>", startIndex);

                if (endIndex >= 0)
                {
                    string profilePictureUrl = xmlResponse.Substring(startIndex, endIndex - startIndex);
                    return profilePictureUrl;
                }
                else
                {
                    return defaultImage;
                }
            }
            else
            {
                return null!;
            }
        }
        catch
        {
            return null!;
        }
    }

    [GameEventHandler]
    public HookResult OnPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {

        if (@event.Userid == null)
        {
            return HookResult.Continue;
        }

        var userid = @event.Userid;
        var textcontent = @event.Text;
        var teamonly = @event.Teamonly;

        string teamtext;

        if (teamonly == true)
        {
            teamtext = "TEAM";
        }
        else
        {
            teamtext = "ALL";
        }

        CCSPlayerController player = playerData[userid];
        var steamid = player.SteamID;
        var name = player.PlayerName;

        Logger.LogInformation($"[{teamtext}] {player}: {textcontent}");
        Logger.LogInformation($"{name} ({player.SteamID})");

        var webhookUrl = Config.webhook_url;
        Task task = SendToDiscord(webhookUrl, textcontent, steamid, name, teamtext);

        return HookResult.Continue;
    }

    public static async Task<string> GetProfilePictureAsync(ulong steamId64, string defaultImage)
    {
        try
        {
            var client = new HttpClient();
            string apiUrl = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string xmlResponse = await response.Content.ReadAsStringAsync();
                int startIndex = xmlResponse.IndexOf("<avatarFull><![CDATA[") + "<avatarFull><![CDATA[".Length;
                int endIndex = xmlResponse.IndexOf("]]></avatarFull>", startIndex);

                if (endIndex >= 0)
                {
                    string profilePictureUrl = xmlResponse.Substring(startIndex, endIndex - startIndex);
                    return profilePictureUrl;
                }
                else
                {
                    return defaultImage;
                }
            }
            else
            {
                return null!;
            }
        }
        catch
        {
            return null!;
        }
    }

    public static async Task SendToDiscord(string webhookUrl, string message, ulong steamUserId, string STEAMNAME, string teamtext)
    {
        try
        {
            string profileLink = $"https://steamcommunity.com/profiles/{steamUserId}";
            string profilePictureUrl = await GetProfilePictureAsync(steamUserId, "https://i.imgur.com/7w4muvd.jpeg");
            int colorss = int.Parse("00FFFF", System.Globalization.NumberStyles.HexNumber);
            Color color = Color.FromArgb(colorss >> 16, (colorss >> 8) & 0xFF, colorss & 0xFF);
            using (var httpClient = new HttpClient())
            {
                var embed = new
                {
                    type = "rich",
                    description = "",
                    color = color.ToArgb() & 0xFFFFFF,
                    author = new
                    {
                        name = $"[{teamtext}] {STEAMNAME}: {message}",
                        url = profileLink,
                        icon_url = profilePictureUrl
                    }
                };

                var payload = new
                {
                    embeds = new[] { embed }
                };

                string jsonPayload = JsonSerializer.Serialize(payload);
                using StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);
            }
        }
        catch
        {

        }

    }
}
