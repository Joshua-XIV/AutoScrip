using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoScrip.Helpers;

internal unsafe class ObjectHelper
{
    internal static unsafe void InteractWithObject(IGameObject? gameObject)
    {
        try
        {
            if (gameObject == null || !gameObject.IsTargetable)
                return;
            var gameObjectPointer = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)gameObject.Address;
            TargetSystem.Instance()->InteractWithObject(gameObjectPointer, false);
        }
        catch (Exception ex)
        {
            Svc.Log.Info($"InteractWithObject: Exception: {ex}");
        }
    }
    internal static IGameObject? GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind objectKind) => Svc.Objects.OrderBy(gameObject => DistanceHelper.GetDistanceToPlayer(gameObject.Position)).FirstOrDefault(gameObject => gameObject.ObjectKind == objectKind);
    internal static IGameObject? GetObjectByGameObjectId(uint gameObjectId)
    {
        var gameObject = Svc.Objects.FirstOrDefault(gameO =>
        {
            if (gameO == null) return false;
            var address = (GameObject*)(void*)gameO.Address;
            if (address->GetGameObjectId().ObjectId != gameObjectId) return false;
            return true;
        });

        if (gameObject == null)
        {
            gameObject = null;
            return gameObject;
        }
        return gameObject;
    }

    internal static unsafe AtkUnitBase* InteractWithObjectUntilAddon(IGameObject? gameObject, string addonName)
    {
        if (TryGetAddonByName<AtkUnitBase>(addonName, out var addon) && IsAddonReady(addon))
            return addon;

        if (EzThrottler.Throttle("InteractWithObjectUntilAddon"))
            InteractWithObject(gameObject);

        return null;
    }
}
