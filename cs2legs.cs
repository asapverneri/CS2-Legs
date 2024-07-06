using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2Legs;

[MinimumApiVersion(247)]
public class CS2Legs : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "CS2Legs";
    public override string ModuleDescription => "Disable lower body by command";
    public override string ModuleAuthor => "verneri";
    public override string ModuleVersion => "1.2.0";

    public Config Config { get; set; } = new();

    public void OnConfigParsed(Config config)
	{
        Console.WriteLine($"[CS2Legs] Loaded config!");
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine($"[CS2Legs] loaded without errors!");
    }

    HashSet<ulong> Hiding = new HashSet<ulong>();

    [ConsoleCommand("css_legs", "hide legs command")]
    public void HideLegsCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || !player.IsValid || !player.PawnIsAlive || player.Team == CsTeam.Spectator || player.Team == CsTeam.None)
            return;

        var Name = player.PlayerName;
        var Language = player.GetLanguage();

        if (!AdminManager.PlayerHasPermissions(player, Config.VipFlag))
        {
            command.ReplyToCommand($"{Localizer["no.access", Name, Language]}");
            return;
        }


        if (!Hiding.Contains(player.SteamID))
        {
            Hiding.Add(player.SteamID);
            player.PlayerPawn.Value.Render = Color.FromArgb(254, 255, 255, 255);
            Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseModelEntity", "m_clrRender");
            command.ReplyToCommand($"{Localizer["legs.hidden", Name, Language]}");
        }
        else
        {
            Hiding.Remove(player.SteamID);
            player.PlayerPawn.Value.Render = Color.FromArgb(255, 255, 255, 255);
            Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseModelEntity", "m_clrRender");
            command.ReplyToCommand($"{Localizer["legs.shown", Name, Language]}");
        }
    }

    // Credits to exkludera
    [GameEventHandler]
    public HookResult PlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (Hiding.Contains(@event.Userid.SteamID))
            SpawnFixnextround(@event.Userid);

        return HookResult.Continue;
    }

    private void SpawnFixnextround(CCSPlayerController? player)
    {
        AddTimer(0.66f, () =>
        {
            player.PlayerPawn.Value.Render = Color.FromArgb(254, 255, 255, 255);
            Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseModelEntity", "m_clrRender");
            //Server.PrintToChatAll("look down");
        });
    }
}