using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoScrip.Scheduler.Tasks;

internal unsafe class TaskAethernet
{
    internal static void Enqueue(string aethernetName, uint aethernetId, uint aethernetIndex, uint zoneId, IGameObject gameObject)
    {
        var address = (GameObject*)(void*)gameObject.Address;
        var gameObjectId = address->GetGameObjectId().ObjectId;
        if (aethernetName != string.Empty && gameObjectId != aethernetId)
        {
            Generic.PluginDebugInfoEnqueue("Using Aethernet");
            TaskMoveTo.Enqueue(gameObject.Position, gameObject.Name.ToString(), 7f);
            Plugin.taskManager.Enqueue(() => Aethernet(aethernetName, aethernetIndex));
            Plugin.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.BetweenAreas] && ZonesHelper.IsInZone(zoneId));
        }
    }

    internal unsafe static bool Aethernet(string aethernetName, uint aethernetIndex)
    {
        AtkUnitBase* addon;
        if (aethernetName == string.Empty || Svc.Condition[ConditionFlag.BetweenAreas])
            return true;

        if (TryGetAddonByName<AtkUnitBase>("TelepotTown", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("Aethernet"))
                Callback.Fire(addon, true, 11, aethernetIndex);
            return false;
        }
        else if (TryGetAddonByName<AtkUnitBase>("SelectString", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("Aethernet"))
                Callback.Fire(addon, true, 0);
            return false;
        }
        else if (!TryGetAddonByName<AtkUnitBase>("TelepotTown", out addon) || !IsAddonReady(addon))
        {
            IGameObject? gameObject;
            if ((gameObject = ObjectHelper.GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte)) == null)
                return false;

            if ((addon = ObjectHelper.InteractWithObjectUntilAddon(gameObject, "SelectString")) == null)
                return false;
        }
        return false;
    }
}
