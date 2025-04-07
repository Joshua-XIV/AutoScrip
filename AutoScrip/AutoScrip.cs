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
using ECommons.Automation;
using AutoRetainerAPI;
using System.Diagnostics;
using Dalamud.Interface.Textures.TextureWraps;

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

    internal WindowSystem windowSystem;
    internal MainWindow mainWindow;
    internal TaskManager taskManager;
    internal NavmeshIPC navmeshIPC;
    internal AutoHookIPC autoHookIPC;
    internal AutoRetainerApi autoRetainerApi;
    internal Stopwatch stopwatch;

    internal string orangeImagePath;
    internal string purpleImagePath;
    public AutoScrip()
    {
        Plugin = this;
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

        orangeImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Assets", "OrangeScrip.png");
        purpleImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Assets", "PurpleScrip.png");

        MainTab.Load();

        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;

        Svc.PluginInterface.UiBuilder.OpenMainUi += () =>
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        };

        Svc.Framework.Update += Tick;
        EzCmd.Add("/autoscrip", OnCommand, """ 
                Opens Plugin Interface
                /autoscrip r|run|on -> Starts Plugin
                /autoscrip s|stop|off -> Stops Plugin
                """);
    }

    private void Tick(object _)
    {
        if (SchedulerMain.runPlugin && Svc.ClientState.LocalPlayer != null)
        {
            SchedulerMain.Tick();
        }
    }

    public void Dispose()
    {
        ECommonsMain.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        if (args.EqualsIgnoreCaseAny("r", "run", "runs", "on"))
        {
            if (!C.AcceptedDisclaimer)
                DuoLog.Error("Cannot Run Command Unil Disclaimer is Accepted!");
            else
                SchedulerMain.EnablePlugin();
        }
        else if (args.EqualsIgnoreCaseAny("s", "stop", "stops", "off"))
        {
            if (!C.AcceptedDisclaimer)
                DuoLog.Error("Cannot Run Command Until Disclaimer is Accepted!");
            else
                SchedulerMain.DisablePlugin();
        }
        else
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        }
    }
}
