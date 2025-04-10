using ECommons.DalamudServices;
using ECommons.GameHelpers;

namespace AutoScrip.Helpers;

internal static class StateConditionHelper
{
    internal static bool ErrorConditions()
    {
        if (!InventoryHelper.HasFishingBait() && !C.BuyBait)
        {
            DuoLog.Error("No fishing bait in inventory, and auto-buying is disabled.");
            return true;
        }

        if (!InventoryHelper.HasFishingBait() && C.BuyBait && InventoryHelper.GetGil() < 300)
        {
            DuoLog.Error("No fishing bait in inventory, and you don’t have enough gil (need at least 300).");
            return true;
        }

        if (!InventoryHelper.HasFishingBait() && C.BuyBait && InventoryHelper.GetFreeInventorySlots() == 0)
        {
            DuoLog.Error("No fishing bait in inventory, and there are no free inventory slots.");
            return true;
        }

        if (!InventoryHelper.HasInvetorySpace())
        {
            DuoLog.Error("Not enough inventory space.");
            return true;
        }
        return false;
    }

    internal static bool NeedSwapJobs()
    {
        if (Svc.ClientState.LocalPlayer.GetJob() != ECommons.ExcelServices.Job.FSH && StatusesHelper.PlayerNotBusy())
            return true;
        else
            return false;
	}}
