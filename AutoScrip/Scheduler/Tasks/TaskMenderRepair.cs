using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskMenderRepair
{
    internal static void Enqueue()
    {
        Generic.PluginDebugInfoEnqueue("Repairing Mender");
        if (DistanceHelper.GetDistanceToPlayer(C.SelectedFish.RepairLocation) > 100)
        {
            if (C.SelectedScripColor == ScripColor.Orange)
                TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedFish.ZoneId), C.SelectedFish.ZoneId, true);
            else
                TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedFish.ZoneId ) + 1, C.SelectedFish.ZoneId, true);
        }
        if (DistanceHelper.GetDistanceToPlayer(C.SelectedFish.RepairLocation) > 20)
        {
            TaskMount.Enqueue();
            TaskFlight.Enqueue();
            TaskMoveTo.Enqueue(C.SelectedFish.RepairLocation, "Repair", 1f, true);
        }
        else
        {
            TaskMoveTo.Enqueue(C.SelectedFish.RepairLocation, "Repair", 1f, false);
        }
        Plugin.taskManager.Enqueue(Repair);
    }

    internal unsafe static bool? Repair()
    {
        AtkUnitBase* addon;
        if (!RepairAndExtractHelper.NeedsRepair(C.RepairThreshold))
        {
            if (TryGetAddonByName<AtkUnitBase>("Repair", out addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("ExitRepair"))
                    Callback.Fire(addon, true, -1);
            }
            return true;
        }
        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("SelectYes"))
                Callback.Fire(addon, true, 0);
            return false;
        }
        else if (TryGetAddonByName<AtkUnitBase>("Repair", out addon) && IsAddonReady(addon))
        {
            if(EzThrottler.Throttle("RepairAll"))
                Callback.Fire(addon, true, 0);
            return false;
        }
        else if (!TryGetAddonByName<AtkUnitBase>("Repair", out addon) || !IsAddonReady(addon))
        {
            IGameObject? gameObject;
            if ((gameObject = ObjectHelper.GetObjectByGameObjectId(C.SelectedFish.MenderGameObjectId)) == null)
                return false;

            Svc.Targets.SetTarget(gameObject);

            if ((addon = ObjectHelper.InteractWithObjectUntilAddon(gameObject, "Repair")) == null)
                return false;
        }
        return false;
    }
}
