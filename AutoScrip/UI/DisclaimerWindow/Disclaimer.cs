using Dalamud.Interface.Colors;
using ImGuiNET;
using System.Numerics;

namespace AutoScrip.UI.DisclaimerWindow;

internal class Disclaimer
{
    public static void Draw()
    {
        ImGui.TextColored(ImGuiColors.DalamudYellow, "DISCLAIMER!");
        ImGui.Separator();
        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
        ImGui.TextWrapped("To avoid unnecessary consequences please follow these rules when using AutoScrip\n");
        ImGui.PopStyleColor();
        ImGui.TextWrapped("1. Do not acknowledge your use of AutoScrip in game chat");
        ImGui.TextWrapped("2. Do not leave Treasure Maps unattended for a prolonged period of time");
        ImGui.TextWrapped("3. Never reply back to players that are trying to do so-called bot checks by trading or chatting with you; instead, blacklist these players immediately");
        ImGui.TextWrapped("4. If questioned by a game master, always claim that you are doing all actions manually and never admit plugin usage.");
        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
        ImGui.TextWrapped("Failure to follow these rules may result in actions taken against your account.");
        ImGui.PopStyleColor();
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0, ImGui.GetContentRegionMax().Y - ImGui.GetCursorPosY() - 40));
        if (ImGui.Button("Accept and Continue", new Vector2(ImGui.GetContentRegionAvail().X, 30)))
        {
            C.AcceptedDisclaimer = true;
            C.Save();
            PluginLog.Information("User accepted the rules.");
        }
    }
}
