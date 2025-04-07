using AutoScrip.Data;
using AutoScrip.Helpers;
using AutoScrip.IPC;
using AutoScrip.Scheduler;
using System.Numerics;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Textures.TextureWraps;

namespace AutoScrip.UI.MainWindow;

internal class MainTab
{
    private static readonly string[] scripColorList = Enum.GetNames(typeof(ScripColor));
    private static int selectedColorIndex = (int)C.SelectedScripColor;
    private static List<ScripItem> filteredItems = new();
    private static string[] filterItemsList = Array.Empty<string>();
    private static string[] citiesList = Enum.GetNames(typeof(City));
    private static int selectedCityIndex = Array.FindIndex(HubCities.Cities.ToArray(), city => city.ZoneName == C.SelectedCity.ZoneName);
    public static void Load()
    {
        UpdateItemSelection();
        UpdateFish();
        UpdateCitySelection();
    }

    public static void Draw()
    {
        var tex = C.SelectedScripColor == ScripColor.Orange ? Plugin.orangeImagePath : Plugin.purpleImagePath;
        var text2 = Plugin.TextureProvider.GetFromFile(tex).GetWrapOrDefault();
        ImGui.SetNextItemWidth(300);
        if (ImGui.Combo("Select Scrip Color", ref selectedColorIndex, scripColorList, scripColorList.Length))
        {
            C.SelectedScripColor = (ScripColor)selectedColorIndex;
            C.SelectedFish = FishTable.Table.FirstOrDefault(fish => fish.ScripColor == (ScripColor)selectedColorIndex)!;
            UpdateItemSelection();
            C.Save();
        }

        if (text2 != null)
        {
            ImGui.SameLine();
            ImGui.Image(text2.ImGuiHandle, new Vector2(20f, 20f));
        }

        ImGui.SetNextItemWidth(300);
        int currentIndex = C.SelectedScripColor == ScripColor.Orange ? C.SelectedOrangeItemIndex : C.SelectedPurpleItemIndex;

        if (ImGui.Combo("Select Exchange Item", ref currentIndex, filterItemsList, filterItemsList.Length))
        {
            if (filteredItems.Count > 0)
            {
                var selectedItem = filteredItems[currentIndex];

                if (C.SelectedScripColor == ScripColor.Orange)
                {
                    C.OrangeScripItem = selectedItem;
                    C.SelectedOrangeItemIndex = currentIndex;
                }
                else
                {
                    C.PurpleScripItem = selectedItem;
                    C.SelectedPurpleItemIndex = currentIndex;
                }

                C.Save();
            }
        }

        ImGui.SetNextItemWidth(300);

        if (ImGui.Combo("Select City", ref selectedCityIndex, citiesList, citiesList.Length))
        {
            C.SelectedCity = HubCities.Cities[selectedCityIndex];
            C.Save();
        }

        ImGui.Text("Required Plugins: ");
        Generic.CheckMarkTipString(NavmeshIPC.Name, NavmeshIPC.Repo);
        Generic.CheckMarkTipString(AutoHookIPC.Name, AutoHookIPC.Repo);
        ImGui.NewLine();
        ImGui.Text($"Current Task: {SchedulerMain.CurrentState.ToString()}");
        if (SchedulerMain.CurrentState == SchedulerMain.State.Fishing)
        {
            ImGui.Text($"Time Remaining at Fishing Location: {SchedulerMain.GetFishingTimeRemaining()}");
        }
        ImGui.SetCursorPosY(ImGui.GetWindowSize().Y - 40);

        if (ImGui.Button(!SchedulerMain.runPlugin ? "Start AutoScrip" : "Stop AutoScrip", new Vector2(ImGui.GetContentRegionAvail().X, 30)))
        {
            if (!SchedulerMain.runPlugin)
            {
                SchedulerMain.EnablePlugin();
            }
            else
            {
                SchedulerMain.DisablePlugin();
            }
        }
    }

    private static void UpdateItemSelection()
    {
        filteredItems = ScripExchangeItems.Items
            .Where(item => item.ScripColor == C.SelectedScripColor)
            .ToList();

        filterItemsList = filteredItems.Select(item => item.ItemName).ToArray();

        if (C.SelectedScripColor == ScripColor.Orange)
        {
            C.SelectedOrangeItemIndex = Math.Clamp(C.SelectedOrangeItemIndex, 0, filteredItems.Count - 1);
        }
        else
        {
            C.SelectedPurpleItemIndex = Math.Clamp(C.SelectedPurpleItemIndex, 0, filteredItems.Count - 1);
        }
    }

    private static void UpdateFish()
    {
        if (C.SelectedScripColor == ScripColor.Orange)
        {
            C.SelectedFish = FishTable.Table.FirstOrDefault(fish => fish.ScripColor == ScripColor.Orange)!;
        }
        else
        {
            C.SelectedFish = FishTable.Table.FirstOrDefault(fish => fish.ScripColor == ScripColor.Purple)!;
        }
    }

    private static void UpdateCitySelection()
    {
        C.SelectedCity = HubCities.Cities.FirstOrDefault(city => city.ZoneName == C.SelectedCity.ZoneName)!;
    }
}
