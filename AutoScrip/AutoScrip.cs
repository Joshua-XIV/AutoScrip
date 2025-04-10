using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Automation.LegacyTaskManager;
using ECommons.Configuration;
using AutoScrip.Configuration;
using AutoScrip.UI.MainWindow;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using AutoScrip.Scheduler;
using AutoScrip.IPC;
using AutoRetainerAPI;
using System.Diagnostics;
using AutoScrip.Scheduler.Tasks;
using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Objects.Types;
using static AutoScrip.Scheduler.SchedulerMain;
using AutoScrip.Data;
using ECommons.ChatMethods;
using AutoScrip.Toast;
namespace AutoScrip;

public class AutoScrip : IDalamudPlugin
{
    public string Name => "AutoScrip";
    internal static AutoScrip Plugin = null!;
    private Config config;
    public static Config C => Plugin.config;

    [PluginService] 
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService]
    internal IDataManager DataManager { get; init; } = null!;
    [PluginService]
    internal IGameGui GameGui { get; init; } = null!;
    [PluginService] 
    internal ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService]
    internal IGameInteropProvider GameInteropProvider { get; init; } = null!;
    [PluginService]
    internal IToastGui ToastGui { get; private init; } = null!;

    internal WindowSystem windowSystem;
    internal MainWindow mainWindow;
    internal TaskManager taskManager;
    internal NavmeshIPC navmeshIPC;
    internal AutoHookIPC autoHookIPC;
    internal AutoRetainerApi autoRetainerApi;
    internal Stopwatch stopwatch;
    internal ToastHelper toast { get; }
    public AutoScrip(IToastGui toastGui)
    {
        Plugin = this;
        ToastGui = toastGui;
        toast = new ToastHelper();
        ECommonsMain.Init(PluginInterface, Plugin, Module.DalamudReflector, Module.ObjectFunctions);
        new ECommons.Schedulers.TickScheduler(Load);
    }

    public void Load()
    {
        EzConfig.Migrate<Config>();
        this.config = EzConfig.Init<Config>();

        windowSystem = new();
        mainWindow = new();
        taskManager = new();
        navmeshIPC = new();
        autoHookIPC = new();
        autoRetainerApi = new();
        stopwatch = new();

        MainTab.Load();

        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;

        Svc.PluginInterface.UiBuilder.OpenMainUi += () =>
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        };

        Svc.Framework.Update += Tick;
        EzCmd.Add("/ats", OnCommand, "");
        EzCmd.Add("/autoscrip", OnCommand, """ 
                /autoscrip|/ats -> both can be used interchangeably
                /autoscrip -> Opens Plugin Interface
                /autoscrip r|run|on -> Starts Plugin
                /autoscrip s|stop|off -> Stops Plugin
                /autoscrip gotoexchange -> Goes to selected city's exchange location
                /autoscrip turnin -> Goes to appraise selected collectable fish in inventory
                /autoscrip turnincycle -> Goes to appraise and exchange selected collectable fish and item
                /autoscrip color <color> -> Sets the scrip color you're using
                /autoscrip city <city> -> Sets the active city used for turn-ins
                """);
    }

    private void Tick(object _)
    {
        if (SchedulerMain.runPlugin && !runCommandTask && Svc.ClientState.LocalPlayer != null)
        {
            SchedulerMain.Tick();
        }
        else if (!SchedulerMain.runPlugin && runCommandTask && Svc.ClientState.LocalPlayer != null)
        {
            onCommandTask();
        }
    }

    public void Dispose()
    {
        toast.Dispose();
        ECommonsMain.Dispose();
    }

    public bool runCommandTask = false;
    public CommandState CurrentCommandState = CommandState.End;
    public enum CommandState
    {
        End,
        GoToExchange,
        TurnIn,
        TurnInCycle,
        GoToTurnIn,
        WaitingForVnav
    }
    private void OnCommand(string command, string args)
    {
        if (!C.AcceptedDisclaimer)
            DuoLog.Error("Cannot Run Command Unil Disclaimer is Accepted!");
        else
        {
            string[] argParts = args.Split(' ');
            string primaryCommand = argParts[0].ToLowerInvariant();

            switch (primaryCommand)
            {
                case "r":
                case "run":
                case "runs":
                case "on":
                    if (!runCommandTask)
                        EnablePlugin();
                    else
                        DuoLog.Error("Command is currently running.");
                    break;

                case "s":
                case "stop":
                case "stops":
                case "off":
                    DisablePlugin();
                    CurrentCommandState = CommandState.End;
                    runCommandTask = false;
                    break;

                case "gotoexchange":
                    if (!runPlugin && !runCommandTask)
                    {
                        runCommandTask = true;
                        CurrentCommandState = CommandState.GoToExchange;
                        ChatPrinter.Green($"[AutoScrip] Going to {C.SelectedCity.ZoneName} exchange location");
                    }
                    else
                    {
                        ChatPrinter.Red("[AutoScrip] AutoScrip is Busy");
                    }
                    break;

                case "turnin":
                    if (!runPlugin && !runCommandTask)
                    {
                        if (InventoryHelper.GetFishItemCount(C.SelectedFish.FishId) > 0 && (4000 - InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId)) > C.SelectedFish.MaxScripTurnIn)
                        {
                            runCommandTask = true;
                            CurrentCommandState = CommandState.TurnIn;
                            ChatPrinter.Green($"[AutoScrip] Going to appraise {C.SelectedFish.FishName} in invetory for {C.SelectedScripColor} scrips");
                        }
                        else if (InventoryHelper.GetFishItemCount(C.SelectedFish.FishId) > 0 && (4000 - InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId)) < C.SelectedFish.MaxScripTurnIn)
                        {
                            ChatPrinter.Yellow($"[AutoScrip] {C.SelectedScripColor} Scrips are almost maxed");
                        }
                        else
                            DuoLog.Error($"No {C.SelectedFish.FishName} in inventory to Appraise");
                    }
                    else
                    {
                        ChatPrinter.Red("[AutoScrip] AutoScrip is Busy");
                    }
                    break;

                case "turnincycle":
                    if (!runPlugin && !runCommandTask)
                    {
                        if (InventoryHelper.GetFishItemCount(C.SelectedFish.FishId) > 0)
                        {
                            var itemToBuy = C.SelectedScripColor == ScripColor.Orange ? C.OrangeScripItem : C.PurpleScripItem;
                            runCommandTask = true;
                            CurrentCommandState = CommandState.GoToTurnIn;
                            ChatPrinter.Green($"[AutoScrip] Going to appraise {C.SelectedFish.FishName} in invetory and exchange {C.SelectedScripColor} scrips for {itemToBuy.ItemName}");
                        }
                        else
                            DuoLog.Error($"No {C.SelectedFish.FishName} in inventory to Appraise");
                    }
                    else
                    {
                        ChatPrinter.Red("[AutoScrip] AutoScrip is Busy");
                    }
                    break;

                case "color":
                    if (argParts.Length > 1)
                    {
                        string input = argParts[1].ToLowerInvariant();

                        if ("purple".StartsWith(input))
                        {
                            C.SelectedScripColor = ScripColor.Purple;
                            ChatPrinter.Green("[AutoScrip] Set color to Purple");
                            MainTab.Load();
                        }
                        else if ("orange".StartsWith(input))
                        {
                            C.SelectedScripColor = ScripColor.Orange;
                            ChatPrinter.Green("[AutoScrip] Set color to Orange");
                            MainTab.Load();
                        }
                        else
                        {
                            DuoLog.Error("Invalid color.");
                        }
                    }
                    else
                    {
                        DuoLog.Error("Missing color argument.");
                    }
                    break;

                case "item":
                    break;

                case "city":
                    if (argParts.Length > 1)
                    {
                        string input = argParts[1].ToLowerInvariant();

                        var match = HubCities.Cities
                            .FirstOrDefault(c => c.ZoneName.ToString().ToLowerInvariant().StartsWith(input));

                        if (match != null)
                        {
                            C.SelectedCity = match; // or however you store it
                            ChatPrinter.Green($"[AutoScrip] Set city to {match.ZoneName}");
                            MainTab.Load();
                        }
                        else
                        {
                            DuoLog.Error($"Could not prase {input}.");
                        }
                    }
                    else
                    {
                        DuoLog.Error("Missing city argument.");
                    }
                    break;

                default:
                    mainWindow.IsOpen = !mainWindow.IsOpen;
                    break;
            }
        }
    }

    private void onCommandTask()
    {
        if (runCommandTask)
        {
            if (!Plugin.taskManager.IsBusy)
            {
                switch (CurrentCommandState)
                {
                    case CommandState.End:
                        runCommandTask = false;
                        Plugin.taskManager.Abort();
                        Plugin.navmeshIPC.Stop();
                        break;

                    case CommandState.TurnIn:
                        if (!Plugin.navmeshIPC.IsReady())
                        {
                            CurrentCommandState = CommandState.WaitingForVnav;
                        }
                        else if (ZonesHelper.IsInZone(C.SelectedCity.ZoneId) && StatusesHelper.PlayerNotBusy())
                        {
                            IGameObject gameObject = ObjectHelper.GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte);
                            TaskAethernet.Enqueue(C.SelectedCity.Aethernet.AethernetName, C.SelectedCity.Aethernet.AethernetGameObjectId, C.SelectedCity.Aethernet.AethernetIndex, C.SelectedCity.Aethernet.AethernetZoneId, gameObject);
                            TaskMoveTo.Enqueue(C.SelectedCity.ScripExchange.Position, "Scrip Exchange", 1f);
                            TaskAppraise.Enqueue();
                            Plugin.taskManager.Enqueue(() => CurrentCommandState = CommandState.End);
                        }
                        else
                        {
                            TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedCity.ZoneId), C.SelectedCity.ZoneId);
                        }
                        break;

                    case CommandState.TurnInCycle:
                        ScripItem itemToBuy = C.SelectedScripColor == ScripColor.Orange ? C.OrangeScripItem : C.PurpleScripItem;
                        if (InventoryHelper.GetFishItemCount(C.SelectedFish.FishId) > 0 || InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) >= itemToBuy.Price)
                        {
                            if (C.SelectedCity.ScripExchange.Position2 != default && InventoryHelper.GetFishItemCount(C.SelectedFish.FishId) > 0)
                                TaskMoveTo.Enqueue(C.SelectedCity.ScripExchange.Position, "Collectable Exchange", 1f);
                            TaskAppraise.Enqueue();
                            if (C.SelectedCity.ScripExchange.Position2 != default)
                                TaskMoveTo.Enqueue(C.SelectedCity.ScripExchange.Position2, "Exchange", 1f);
                            TaskExchange.Enqueue();
                        }
                        else
                        {
                            CurrentCommandState = CommandState.End;
                        }
                        break;

                    case CommandState.GoToExchange:
                        if (!Plugin.navmeshIPC.IsReady())
                        {
                            CurrentCommandState = CommandState.WaitingForVnav;
                        }
                        else if (ZonesHelper.IsInZone(C.SelectedCity.ZoneId) && StatusesHelper.PlayerNotBusy())
                        {
                            IGameObject gameObject = ObjectHelper.GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte);
                            TaskAethernet.Enqueue(C.SelectedCity.Aethernet.AethernetName, C.SelectedCity.Aethernet.AethernetGameObjectId, C.SelectedCity.Aethernet.AethernetIndex, C.SelectedCity.Aethernet.AethernetZoneId, gameObject);
                            if (C.SelectedCity.ScripExchange.Position2 != default)
                                TaskMoveTo.Enqueue(C.SelectedCity.ScripExchange.Position2, "Scrip Exchange", 1f);
                            else
                                TaskMoveTo.Enqueue(C.SelectedCity.ScripExchange.Position, "Scrip Exchange", 1f);
                            Plugin.taskManager.Enqueue(() => CurrentCommandState = CommandState.End);
                        }
                        else
                        {
                            TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedCity.ZoneId), C.SelectedCity.ZoneId);
                        }
                        break;

                    case CommandState.GoToTurnIn:
                        if (!Plugin.navmeshIPC.IsReady())
                        {
                            CurrentCommandState = CommandState.WaitingForVnav;
                        }
                        else if (ZonesHelper.IsInZone(C.SelectedCity.ZoneId) && StatusesHelper.PlayerNotBusy())
                        {
                            IGameObject gameObject = ObjectHelper.GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte);
                            TaskAethernet.Enqueue(C.SelectedCity.Aethernet.AethernetName, C.SelectedCity.Aethernet.AethernetGameObjectId, C.SelectedCity.Aethernet.AethernetIndex, C.SelectedCity.Aethernet.AethernetZoneId, gameObject);
                            TaskMoveTo.Enqueue(C.SelectedCity.ScripExchange.Position, "Scrip Exchange", 1f);
                            Plugin.taskManager.Enqueue(() => CurrentCommandState = CommandState.TurnInCycle);
                        }
                        else
                        {
                            TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedCity.ZoneId), C.SelectedCity.ZoneId);
                        }
                        break;
                }
            }
        }
    }
}
