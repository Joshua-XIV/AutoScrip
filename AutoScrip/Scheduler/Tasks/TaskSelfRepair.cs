using AutoScrip.Helpers;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoScrip.Scheduler.Tasks;

internal class TaskSelfRepair
{
    internal static void Enqueue()
    {
        Generic.PluginLogInfoEnqueue("Self Repairing");
        Plugin.taskManager.Enqueue(OpenSelfRepair);
        Plugin.taskManager.Enqueue(SelfRepair);
    }

    internal unsafe static bool? SelfRepair()
    {
        AtkUnitBase* addon;
        if (!RepairAndExtractHelper.NeedsRepair(C.RepairThreshold) || !InventoryHelper.HasDarkMatter())
        {
            if (TryGetAddonByName<AtkUnitBase>("Repair", out addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("GlobalTurnInGenericThrottle"))
                {
                    Callback.Fire(addon, true, -1);
                }
                return false;
            }
            return true;
        }
        else if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("SelectYesnoThrottle"))
            {
                Callback.Fire(addon, true, 0);
            }
        }
        else if (TryGetAddonByName<AtkUnitBase>("Repair", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("GlobalTurnInGenericThrottle"))
            {
                Generic.PluginDebugInfoInstant("Repair Callback");
                Callback.Fire(addon, true, 0);
            }
        }
        return false;
    }

    internal unsafe static bool? OpenSelfRepair()
    {
        if (TryGetAddonByName<AtkUnitBase>("Repair", out var addon3) && IsAddonReady(addon3))
        {
            return true;
        }
        if (EzThrottler.Throttle("Opening Self Repair", 1000))
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 6);
        return false;
    }
}
