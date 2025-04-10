using AutoScrip.Helpers;
using Dalamud.Game.Gui.Toast;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.Sheets;

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
        ImGui.Text($"{C.UseAndOperator}");
        if (Plugin.toast.GetLastToast() != null)
            ImGui.Text(Plugin.toast.GetLastToast());
        else
            ImGui.Text("Toast hasn't been fired yet");

        if (TryGetAddonByName<AtkUnitBase>("CollectablesShop", out var addon) && IsAddonReady(addon))
        {
            var radioButton = addon->GetNodeById(13);
            var radioButton2 = radioButton->GetAsAtkComponentRadioButton();
            ImGui.Text($"Fish Collectables Selected: {radioButton2->IsSelected}");
        }
    }
}
