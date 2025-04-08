using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoScrip.Scheduler.Tasks;

internal class TaskAppraise
{
    internal static void Enqueue()
    {
        Generic.PluginDebugInfoEnqueue("Appraising");
        Plugin.taskManager.Enqueue(Appraise, 1000 * 60);
    }

    internal unsafe static bool? Appraise()
    {
        AtkUnitBase* addon;
        if (InventoryHelper.GetFishItemCount(C.SelectedFish.FishId) == 0 || (4000 - InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId)) < C.SelectedFish.MaxScripTurnIn)
        {
            if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("ExitYesNo"))
                {
                    Callback.Fire(addon, true, -1);
                }
                return false;
            }
            if (TryGetAddonByName<AtkUnitBase>("CollectablesShop", out addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("ExitShop"))
                {
                    Callback.Fire(addon, true, -1);
                }
                return false;
            }
            return true;
        }

        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("ExitYesNo"))
            {
                Callback.Fire(addon, true, -1);
            }
            return false;
        }

        if (!TryGetAddonByName<AtkUnitBase>("CollectablesShop", out addon) || !IsAddonReady(addon))
        {
            IGameObject? gameObject;
            if ((gameObject = ObjectHelper.GetObjectByGameObjectId(C.SelectedCity.ScripExchange.CollectableAppraiserGameObjectId)) == null)
                return false;

            Svc.Targets.SetTarget(gameObject);

            if ((addon = ObjectHelper.InteractWithObjectUntilAddon(gameObject, "CollectablesShop")) == null)
                return false;
        }

        if (TryGetAddonByName<AtkUnitBase>("CollectablesShop", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("Appraise"))
            {
                Callback.Fire(addon, true, 12, C.SelectedFish.CollectiblesTurnInListIndex);
            }
            if (EzThrottler.Throttle("Trade", 300))
            {
                Callback.Fire(addon, true, 15, 0);
            }
        }

        return false;
    }
}
