using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoScrip.Helpers;

internal class ActionHelper
{
    internal unsafe static void ExecuteAction(ActionType actionType, uint actionId) => ActionManager.Instance()->UseAction(actionType, actionId);
}
