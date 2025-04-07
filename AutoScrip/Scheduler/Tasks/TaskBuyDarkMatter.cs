using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskBuyDarkMatter
{
    internal static void Enqueue()
    {
        Generic.PluginDebugInfoEnqueue("Repairing Mender");
        if (DistanceHelper.GetDistanceToPlayer(C.SelectedFish.RepairLocation) > 100)
        {
            if (C.SelectedScripColor == ScripColor.Orange)
                TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedFish.ZoneId), C.SelectedFish.ZoneId, true);
            else
                TaskTeleport.Enqueue(ZonesHelper.GetAetheryteId(C.SelectedFish.ZoneId) + 1, C.SelectedFish.ZoneId, true);
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
        Plugin.taskManager.Enqueue(() => Buy());
    }

    internal unsafe static bool Buy()
    {
        AtkUnitBase* addon;
        if (InventoryHelper.HasDarkMatter() || InventoryHelper.GetGil()/280 == 0)
        {
            if (TryGetAddonByName<AtkUnitBase>("Shop", out addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("ExitShop"))
                    Callback.Fire(addon, true, -1);
                return false;
            }
            if (TryGetAddonByName<AtkUnitBase>("SelectIconString", out addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("ExitSelect"))
                    Callback.Fire(addon, true, -1);
                return false;
            }
            return true;
        }
        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("SelectYes"))
                Callback.Fire(addon, true, 0);
            return false;
        }
        else if (TryGetAddonByName<AtkUnitBase>("SelectIconString", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("SelectPurcahseCategory"))
                Callback.Fire(addon, true, 6);
        }
        else if (TryGetAddonByName<AtkUnitBase>("Shop", out addon) && IsAddonReady(addon))
        {
            var amountToBuy = C.BuyMaxDarkMatter ? 999 : 99;
            var canBuy = Math.Min(amountToBuy, InventoryHelper.GetGil()/280);
            if (EzThrottler.Throttle("PurchaseItem"))
                Callback.Fire(addon, true, 0, 0, canBuy);
        }
        else if (!TryGetAddonByName<AtkUnitBase>("Shop", out addon) || !IsAddonReady(addon))
        {
            IGameObject? gameObject;
            if ((gameObject = ObjectHelper.GetObjectByGameObjectId(C.SelectedFish.MerchantGameObjectId)) == null)
                return false;

            if ((addon = ObjectHelper.InteractWithObjectUntilAddon(gameObject, "SelectString")) == null)
                return false;
        }
            return false;
    }
}
