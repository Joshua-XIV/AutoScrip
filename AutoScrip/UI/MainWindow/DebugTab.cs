using AutoScrip.Data;
using AutoScrip.Helpers;
using AutoScrip.IPC;
using AutoScrip.Scheduler.Tasks;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace AutoScrip.UI.MainWindow;

internal class DebugTab
{
    public unsafe static void Draw()
    {
        var scripItem = C.SelectedScripColor == ScripColor.Orange ? C.OrangeScripItem : C.PurpleScripItem;
        ImGui.Text($"Can Buy {InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) / scripItem.Price} {scripItem.ItemName}");
        ImGui.Text($"Item Index: {scripItem.ListIndex}");
        ImGui.Text($"Scrip Total {C.SelectedScripColor}: {InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId).ToString()}");
        ImGui.Text($"Dark Matter Total: {InventoryHelper.GetItemCount(33916)}");
        ImGui.Text($"Selected Fish: {C.SelectedFish.FishName}");
        ImGui.Text($"Current Selected Scrip Item: {scripItem.ItemName}");
        ImGui.Text($"Aethernet Index: {C.SelectedCity.Aethernet.AethernetIndex}");
        ImGui.Text($"Aethernet Zone ID: {C.SelectedCity.Aethernet.AethernetZoneId}");
        ImGui.Text($"Fish Count: {InventoryHelper.GetFishItemCount(C.SelectedFish.FishId)}");
        if (C.SelectedCity.Aethernet.AethernetName != string.Empty)
            ImGui.Text($"Aethernet Name: {C.SelectedCity.Aethernet.AethernetName}");
        else
            ImGui.Text("N/A");
        
    }
}
