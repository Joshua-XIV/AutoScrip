using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Security.Principal;

namespace AutoScrip.Helpers;

internal class ActionHelper
{
    internal unsafe static void ExecuteAction(ActionType actionType, uint actionId) => ActionManager.Instance()->UseAction(actionType, actionId);
}
