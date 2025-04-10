using AutoScrip.Data;
using AutoScrip.Helpers;
using AutoScrip.IPC;
using AutoScrip.Scheduler.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameHelpers;

namespace AutoScrip.Scheduler;

internal static class SchedulerMain
{
    private static DateTime? fishingStartTime;
    private static TimeSpan fishingDuration;

    internal static string GetFishingTimeRemaining()
    {
        if (fishingStartTime == null || !fishingSession)
            return null;

        TimeSpan remaining = fishingDuration - (DateTime.Now - fishingStartTime.Value);
        if (remaining <= TimeSpan.Zero)
            return null;
        else
            return $"{(int)remaining.TotalMinutes}m {remaining.Seconds}s";
    }

    private static bool runAutoHook = false;
    private static bool fishingSession = false;
    private static int swapCounter = 0;
    internal static bool extractDuringFishSession = false;
    internal static bool repairDuringFishSession = false;
    internal static State CurrentState = State.Idle;
    internal static bool runPluginInternal;
    internal static bool runPlugin
    {
        get { return runPluginInternal; }
        private set { runPluginInternal = value; }
    }

    internal enum State
    {
        Idle,
        GoingToExchange,
        Exchanging,
        GoingToFishZone,
        GoingToFishLocation,
        Fishing,
        Extracting,
        Repairing,
        SelfRepairing,
        BuyingDarkMatter,
        GoingToBuyBait,
        BuyingBait,
        GoingToReatiner,
        InsideRetainer,
        SwapJobs,
        WaitingForVnav,
        Error
    }

    internal static bool EnablePlugin()
    {
        if (!Generic.IsPluginInstalled("vnavmesh") || !Generic.IsPluginInstalled("AutoHook"))
        {
            DuoLog.Error("Missing Required Plugin(s): " +
                (!Generic.IsPluginInstalled("vnavmesh") ? "vnavmesh " : "") +
                (!Generic.IsPluginInstalled("AutoHook") ? "AutoHook" : ""));
            DisablePlugin();
        }
        DuoLog.Information("Starting Plugin");
        runPlugin = true;
        runAutoHook = false;
        swapCounter = 0;
        CurrentState = State.Idle;
        return true;
    }

    internal static bool DisablePlugin()
    {
        Plugin.taskManager.Abort();
        DuoLog.Information("Disabling Plugin");
        runPlugin = false;
        fishingSession = false;
        CurrentState = State.Idle;
        Plugin.runCommandTask = false;
        Plugin.CurrentCommandState = AutoScrip.CommandState.End;
        Plugin.navmeshIPC.Stop();
        return true;
    }

    private static int FishingTime()
    {
        var random = new Random();
        if (C.SetExactTime)
        {
            return random.Next(C.InitialTime * 60, C.FinalTime * 60);
        }
        else
        {
            return random.Next(15*60, 20*60);
        }
    }

    private static void SetExtractDuringFishSession(bool setBool = false)
    {
        extractDuringFishSession = setBool;
    }

    private static void SetRepairDuringFishSession(bool setBool = false)
    {
        repairDuringFishSession = setBool;
    }

    internal static void Tick()
    {
        if (!runPlugin) return;
        if (Plugin.taskManager.IsBusy) return;
        switch (CurrentState)
        {
            case State.Idle:
                if (Svc.Condition[ConditionFlag.BoundByDuty])
                    CurrentState = State.Error;
                else if (StateConditionHelper.NeedSwapJobs())
                    CurrentState = State.SwapJobs;
                else if (StateConditionHelper.ErrorConditions())
                    CurrentState = State.Error;
                else if (InventoryHelper.CanTurnIn() && StatusesHelper.PlayerNotBusy())
                    CurrentState = State.GoingToExchange;
                else if (StatusesHelper.IsFishing() || (Svc.Condition[ConditionFlag.Gathering] && Svc.ClientState.LocalPlayer.GetJob() == ECommons.ExcelServices.Job.FSH))
                    CurrentState = State.Fishing;
                //else if (AutoRetainerHelper.ARRetainersWaitingToBeProcessed() && C.enableRetainers)
                //
                else if (!InventoryHelper.HasFishingBait() && StatusesHelper.PlayerNotBusy() && InventoryHelper.GetGil() / 300 != 0 && InventoryHelper.GetFreeInventorySlots() != 0)
                    CurrentState = State.GoingToBuyBait;
                else if (!Plugin.navmeshIPC.IsReady())
                    CurrentState = State.WaitingForVnav;
                else if (!ZonesHelper.IsInZone(C.SelectedFish.ZoneId) && StatusesHelper.PlayerNotBusy())
                    CurrentState = State.GoingToFishZone;
                else if (ZonesHelper.IsInZone(C.SelectedFish.ZoneId) && !InventoryHelper.HasDarkMatter() && C.SelfRepair && C.BuyDarkMatter & C.RepairGear && StatusesHelper.PlayerNotBusy() && !(InventoryHelper.GetGil() / 280 == 0) && InventoryHelper.GetFreeInventorySlots() != 0)
                    CurrentState = State.BuyingDarkMatter;
                else if (ZonesHelper.IsInZone(C.SelectedFish.ZoneId) && RepairAndExtractHelper.NeedsRepair(C.RepairThreshold) && C.RepairGear && C.SelfRepair && InventoryHelper.HasDarkMatter() && StatusesHelper.PlayerNotBusy())
                    CurrentState = State.SelfRepairing;
                else if (ZonesHelper.IsInZone(C.SelectedFish.ZoneId) && RepairAndExtractHelper.NeedsRepair(C.RepairThreshold) && C.RepairGear && StatusesHelper.PlayerNotBusy() && !C.SelfRepair)
                    CurrentState = State.Repairing;
                else if (ZonesHelper.IsInZone(C.SelectedFish.ZoneId) && RepairAndExtractHelper.NeedsExtract() && C.ExtractMateria && InventoryHelper.GetFreeInventorySlots() != 0 && StatusesHelper.PlayerNotBusy())
                    CurrentState = State.Extracting;
                else if (StatusesHelper.PlayerNotBusy())
                    CurrentState = State.GoingToFishLocation;
                break;

            case State.GoingToExchange:
                if (!Plugin.navmeshIPC.IsReady())
                {
                    CurrentState = State.WaitingForVnav;
                }
                else if (ZonesHelper.IsInZone(C.SelectedCity.ZoneId) && StatusesHelper.PlayerNotBusy())
                {
                    IGameObject gameObject = ObjectHelper.GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte);
                    TaskAethernet.Enqueue(C.SelectedCity.Aethernet.AethernetName, C.SelectedCity.Aethernet.AethernetGameObjectId, C.SelectedCity.Aethernet.AethernetIndex, C.SelectedCity.Aethernet.AethernetZoneId, gameObject);
                    TaskMoveTo.Enqueue(C.SelectedCity.ScripExchange.Position, "Scrip Exchange", 1f);
                    Plugin.taskManager.Enqueue(() => CurrentState = State.Exchanging);
                }
                else
                {
                    TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedCity.ZoneId), C.SelectedCity.ZoneId);
                }
                break;

            case State.Exchanging:
                {
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
                        CurrentState = State.Idle;
                    }
                }
                break;

            case State.GoingToFishZone:
                TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedFish.ZoneId), C.SelectedFish.ZoneId);
                Plugin.taskManager.Enqueue(() => CurrentState = State.Idle);
                break;

            case State.GoingToFishLocation:
                if (!runAutoHook)
                {
                    Plugin.taskManager.Enqueue(() => AutoHookIPC.DeleteAllAnonymousPresets());
                    Plugin.taskManager.Enqueue(() => AutoHookPresets.SelectPreset());
                    Plugin.taskManager.Enqueue(() => AutoHookIPC.SetPluginState(true));
                    runAutoHook = true;
                }
                if (StatusesHelper.IsFishing())
                    CurrentState = State.Fishing;
                else
                {
                    TaskGoToFishLocation.Enqueue();
                    Plugin.taskManager.Enqueue(() => Plugin.navmeshIPC.Stop());
                }
                break;

            case State.Fishing:
                if (!runAutoHook)
                {
                    Plugin.taskManager.Enqueue(() => AutoHookIPC.DeleteAllAnonymousPresets());
                    Plugin.taskManager.Enqueue(() => AutoHookPresets.SelectPreset());
                    Plugin.taskManager.Enqueue(() => AutoHookIPC.SetPluginState(true));
                    runAutoHook = true;
                }
                if (!fishingSession)
                {
                    fishingDuration = TimeSpan.FromSeconds(FishingTime());
                    fishingStartTime = DateTime.Now;
                    Generic.PluginLogInfoInstant($"Starting Duration of {fishingDuration} minutes");
                    fishingSession = true;
                }
                if (DateTime.Now - fishingStartTime >= fishingDuration)
                {
                    fishingSession = false;
                    fishingStartTime = null;
                    runAutoHook = false;
                    Generic.PluginLogInfoInstant($"Duration Ended");
                    TaskPreserveCollectable.Enqueue();
                    TaskQuitFish.Enqueue();
                    Plugin.taskManager.Enqueue(() => CurrentState = State.Idle);
                }
                else if (Plugin.toast.GetLastToast() == "The fish sense something amiss. Perhaps it is time to try another location.")
                {
                    fishingSession = false;
                    fishingStartTime = null;
                    runAutoHook = false;
                    Generic.PluginLogInfoInstant($"Duration Canceled, must reset fishing hole");
                    TaskPreserveCollectable.Enqueue();
                    TaskQuitFish.Enqueue();
                    TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedCity.ZoneId), C.SelectedCity.ZoneId);
                    Plugin.taskManager.Enqueue(() => CurrentState = State.Idle);
                }
                else if (RepairAndExtractHelper.NeedsExtract() && !StatusesHelper.IsFishing() && InventoryHelper.GetFreeInventorySlots() != 0 && C.ExtractDuringFishing && C.ExtractMateria)
                {
                    Generic.PluginLogInfoInstant($"Fishing Stopped to Extract Materia");
                    TaskQuitFish.Enqueue();
                    Plugin.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.Gathering] && !Svc.Condition[ConditionFlag.Fishing]);
                    Plugin.taskManager.Enqueue(() => extractDuringFishSession = true);
                    Plugin.taskManager.Enqueue(() => CurrentState = State.Extracting);
                }
                else if (RepairAndExtractHelper.NeedsRepair(C.RepairThreshold) && !StatusesHelper.IsFishing() && C.RepairDuringFishing && C.RepairGear)
                {
                    Generic.PluginLogInfoInstant($"Fishing Stopped to Repair");
                    TaskQuitFish.Enqueue();
                    Plugin.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.Gathering] && !Svc.Condition[ConditionFlag.Fishing]);
                    Plugin.taskManager.Enqueue(() => repairDuringFishSession = true);
                    Plugin.taskManager.Enqueue(() => CurrentState = C.SelfRepair ? State.SelfRepairing : State.Repairing);
                }
                else if (!InventoryHelper.HasFishingBait() && !StatusesHelper.IsFishing())
                {
                    fishingSession = false;
                    fishingStartTime = null;
                    runAutoHook = false;
                    Generic.PluginLogInfoInstant($"Duration Ended, No Bait");
                    TaskQuitFish.Enqueue();
                    Plugin.taskManager.Enqueue(() => CurrentState = State.Idle);
                }
                else if (InventoryHelper.GetFreeInventorySlots() == 0)
                {
                    fishingSession = false;
                    fishingStartTime = null;
                    runAutoHook = false;
                    Generic.PluginLogInfoInstant($"Duration Ended, Inventory Full");
                    TaskQuitFish.Enqueue();
                    Plugin.taskManager.Enqueue(() => CurrentState = State.Idle);
                }
                Generic.SelectYes();
                break;

            case State.Repairing:
                if (!RepairAndExtractHelper.NeedsRepair(C.RepairThreshold) && StatusesHelper.PlayerNotBusy())
                    if (!repairDuringFishSession)
                    {
                        CurrentState = State.Idle;
                    }
                    else
                    {
                        TaskGoToFishLocation.Enqueue();
                        Plugin.taskManager.Enqueue(() => CurrentState = State.Fishing);
                        Plugin.taskManager.Enqueue(() => SetRepairDuringFishSession());
                    }
                else
                    TaskMenderRepair.Enqueue();
                break;

            case State.SelfRepairing:
                if ((!RepairAndExtractHelper.NeedsRepair(C.RepairThreshold) || !InventoryHelper.HasDarkMatter()) && StatusesHelper.PlayerNotBusy())
                    if (!repairDuringFishSession)
                    {
                        CurrentState = State.Idle;
                    }
                    else
                    {
                        Plugin.taskManager.Enqueue(TaskGoToFishLocation.GoToFishLocation);
                        Plugin.taskManager.Enqueue(() => CurrentState = State.Fishing);
                        Plugin.taskManager.Enqueue(() => SetRepairDuringFishSession());
                    }
                else
                    TaskSelfRepair.Enqueue();
                break;

            case State.Extracting:
                if (!RepairAndExtractHelper.NeedsExtract() && InventoryHelper.GetFreeInventorySlots() != 0 && StatusesHelper.PlayerNotBusy())
                {
                    if (!extractDuringFishSession)
                    {
                        CurrentState = State.Idle;
                    }
                    else
                    {
                        Plugin.taskManager.Enqueue(TaskGoToFishLocation.GoToFishLocation);
                        Plugin.taskManager.Enqueue(() => CurrentState = State.Fishing);
                        Plugin.taskManager.Enqueue(() => SetExtractDuringFishSession());
                    }
                }
                else
                    TaskExtractMateria.Enqueue();
                break;

            case State.BuyingDarkMatter:
                if ((InventoryHelper.HasDarkMatter() || InventoryHelper.GetGil() > 280) && StatusesHelper.PlayerNotBusy())
                    CurrentState = State.Idle;
                TaskBuyDarkMatter.Enqueue();
                break;

            case State.GoingToBuyBait:
                if (ZonesHelper.IsInZone(BaitData.ZoneId) && StatusesHelper.PlayerNotBusy())
                {
                    IGameObject gameObject = ObjectHelper.GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte);
                    TaskAethernet.Enqueue(BaitData.Aethernet.AethernetName, BaitData.Aethernet.AethernetGameObjectId, BaitData.Aethernet.AethernetIndex, BaitData.ZoneId, gameObject);
                    TaskMoveTo.Enqueue(BaitData.VendorLocation, "Lure Merchant", 1f);
                    Plugin.taskManager.Enqueue(() => CurrentState = State.BuyingBait);
                }
                else if (InventoryHelper.GetFreeInventorySlots() == 0 && !InventoryHelper.HasFishingBait())
                {
                    CurrentState = State.Error;
                }
                else
                {
                    TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(BaitData.ZoneId), BaitData.ZoneId);
                }
                break;

            case State.BuyingBait:
                if (!InventoryHelper.HasFishingBait() && InventoryHelper.GetGil() > 300)
                    TaskBuyBait.Enqueue();
                else
                    CurrentState = State.Idle;
                break;


            case State.SwapJobs:
                if (Svc.ClientState.LocalPlayer.GetJob() == ECommons.ExcelServices.Job.FSH)
                {
                    CurrentState = State.Idle;
                }
                if (swapCounter >= 3 && Svc.ClientState.LocalPlayer.GetJob() != ECommons.ExcelServices.Job.FSH)
                {
                    DuoLog.Error("Failed to swap to Fisher");
                    DisablePlugin();
                }
                else
                {
                    Plugin.taskManager.Enqueue(() => Chat.Instance.ExecuteCommand($"/gearset change \"{C.FishSetName}\""));
                    Plugin.taskManager.DelayNext(500);
                    Plugin.taskManager.Enqueue(() => swapCounter++);
                }
                break;

            case State.WaitingForVnav:
                if (Plugin.navmeshIPC.IsReady())
                    CurrentState = State.Idle;
                else
                    Plugin.taskManager.DelayNext(500);
                break;

            case State.Error:
                if (Svc.Condition[ConditionFlag.BoundByDuty])
                    DuoLog.Error("Can not enable AutoScrip inside duty.");
                DisablePlugin();
                break;

            default:
                CurrentState = State.Idle;
                break;
        }
    }
}