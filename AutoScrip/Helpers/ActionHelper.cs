using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Helpers;

internal class ActionHelper
{
    internal unsafe static void ExecuteAction(ActionType actionType, uint actionId) => ActionManager.Instance()->UseAction(actionType, actionId);
}
