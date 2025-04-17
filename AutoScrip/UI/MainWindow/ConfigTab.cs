using Dalamud.Interface.Components;
using System.Numerics;
using ImGuiNET;
using AutoScrip.Helpers;
using ECommons.DalamudServices;
using AutoScrip.Scheduler.Tasks;
using AutoScrip.Data;

namespace AutoScrip.UI.MainWindow;

internal class ConfigTab
{
    // Debug
    private static bool Debug = C.DEBUG;
    private static bool additionalLogging = C.AdditionalLogging;

    // Autohook Conditions
    private static bool turnOffAutoHook = C.TurnOffAutoHook;

    // Turn-In Conditions
    private static bool setTurnInCondition = C.SetTurnInConditions;
    private static int freeRemainingSlots = C.FreeRemainingSlots;
    private static int minimumFishToTurnin = C.MinimumFishToTurnin;

    // Fish Conditions
    private static bool setExactTime = C.SetExactTime;
    private static int initialTime = C.InitialTime;
    private static int finalTime = C.FinalTime;
    private static bool buyBait = C.BuyBait;
    private static bool buyMaxBait = C.BuyMaxBait;
    private static bool useAndOperator = C.UseAndOperator;
    

    // Set Name
    private static string fishSetName = C.FishSetName;

    // Cordial
    private static bool useCordial = C.UseCordial;
    private static bool reverseCordialPrio = C.ReverseCordialPrio;

    // Extract
    private static bool extractMateria = C.ExtractMateria;
    private static bool extractDuringFishing = C.ExtractDuringFishing;

    // GP
    private static int GPThreshold = C.GPThreshold;
    private static bool setGPThreshold = C.SetGPThreshold;
    private static bool aboveThreshold = C.AboveThreshold;

    // Repair
    private static bool repair = C.RepairGear;
    private static bool selfRepair = C.SelfRepair;
    private static int repairThrehold = C.RepairThreshold == 99 ? 100 : C.RepairThreshold;
    private static bool buyDarkMatter = C.BuyDarkMatter;
    private static bool buyMaxDarkMatter = C.BuyMaxDarkMatter;
    private static bool repairDuringFishing = C.RepairDuringFishing;

    // Waypoints
    private static bool setCustomZorgorWaypoints = C.SetCustomZorgorWaypoints;
    private static bool setCustomFleetingBrandWaypoints = C.SetCustomFleetingBrandWaypoints;

    public static void Draw()
    {
        if (ImGui.TreeNode("General Config"))
        {
            if (ImGui.Checkbox("###Use Cordiral", ref useCordial))
            {
                C.UseCordial = useCordial;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNodeEx("Use Cordial"))
            {
                if (ImGui.Checkbox("Reverse Cordial Priority: Watered-Cordial > Cordial > HI-Cordials", ref reverseCordialPrio))
                {
                    C.ReverseCordialPrio = reverseCordialPrio;
                    C.Save();
                }
#if IGNORE
                /*if (ImGui.Checkbox("###Set GP Threshold", ref setGPThreshold))
                {
                    C.SetGPThreshold = setGPThreshold;
                    C.Save();
                }
                ImGui.SameLine();
                if (ImGui.TreeNodeEx("Set GP Threshold"))
                {
                    ImGui.Text("GP Threshold:");
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(100);

                    if (ImGui.InputInt("##GPThreshold", ref GPThreshold, 25, 50))
                    {
                        GPThreshold = Math.Max(0, Math.Min(1200, GPThreshold));
                        C.GPThreshold = GPThreshold;
                        C.Save();
                    }

                    if (ImGui.RadioButton("Use Above Threshold", aboveThreshold))
                    {
                        aboveThreshold = true;
                        C.AboveThreshold = true;
                        C.Save();
                    }

                    ImGui.SameLine();

                    if (ImGui.RadioButton("Use Below Threshold", !aboveThreshold))
                    {
                        aboveThreshold = false;
                        C.AboveThreshold = false;
                        C.Save();
                    }
                    ImGui.TreePop();
                }*/
#endif
                ImGui.Separator();
                ImGui.TreePop();
            }

            if (ImGui.Checkbox("##Repair Gear", ref repair))
            {
                C.RepairGear = repair;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNode("Repair Gear"))
            {
                if (ImGui.Checkbox("Repair between fish catches", ref repairDuringFishing))
                {
                    C.RepairDuringFishing = repairDuringFishing;
                    C.Save();
                }

                if (ImGui.Checkbox("Self Repair", ref selfRepair))
                {
                    C.SelfRepair = selfRepair;
                    C.Save();
                }

                ImGui.BeginDisabled(!selfRepair);
                if (ImGui.Checkbox("Buy Dark Matter", ref buyDarkMatter))
                {
                    C.BuyDarkMatter = buyDarkMatter;
                    C.Save();
                }
                ImGui.EndDisabled();

                if (selfRepair)
                {
                    ImGuiComponents.HelpMarker("Buys up to 99 Grade 8 Dark Matter when none in Inventory");
                }

                if (!selfRepair)
                {
                    ImGuiComponents.HelpMarker("Self Repair must be enabled to use Dark Matter");
                }

                ImGui.BeginDisabled(!buyDarkMatter || !selfRepair);
                if (ImGui.Checkbox("Buy 999 Dark Matter", ref buyMaxDarkMatter))
                {
                    C.BuyMaxDarkMatter = buyMaxDarkMatter;
                    C.Save();
                }
                ImGui.EndDisabled();

                if (buyDarkMatter && selfRepair)
                {
                    ImGuiComponents.HelpMarker("Buys up to 999 Grade 8 Dark Matter when none in Inventory instead of 99");
                }

                if (!buyDarkMatter || !selfRepair)
                {
                    ImGuiComponents.HelpMarker("Enable Buy Dark Matter to use this option");
                }

                ImGui.Text("Repair Threshold:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);

                if (ImGui.SliderInt("##RepairThreshold", ref repairThrehold, 0, 100))
                {
                    repairThrehold = Math.Max(0, Math.Min(100, repairThrehold));
                    C.RepairThreshold = repairThrehold;
                    if (repairThrehold == 100)
                    {
                        C.RepairThreshold = 99;
                    }
                    C.Save();
                }
                ImGui.Separator();
                ImGui.TreePop();
            }

            if (ImGui.Checkbox("###BuyBait", ref buyBait))
            {
                C.BuyBait = buyBait;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNode("Buy Bait (Versatile Lures)"))
            {
                if (ImGui.Checkbox("Buy 999 Versatile Lures", ref buyMaxBait))
                {
                    C.BuyMaxBait = buyMaxBait;
                    C.Save();
                }
                ImGui.SameLine();
                ImGuiComponents.HelpMarker("Buys 999 Versatile Lures instead of 99");
                ImGui.Separator();
                ImGui.TreePop();
            }

            if (ImGui.Checkbox("###ExtractMateria", ref extractMateria))
            {
                C.ExtractMateria = extractMateria;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNode("Extract Materia"))
            {
                if (ImGui.Checkbox("Extract between fish catches", ref extractDuringFishing))
                {
                    C.ExtractDuringFishing = extractDuringFishing;
                    C.Save();
                }
                ImGui.Separator();
                ImGui.TreePop();
            }

            if (ImGui.Checkbox("DDisable AutoHook on AutoScrip Stop", ref turnOffAutoHook))
            {
                C.TurnOffAutoHook = turnOffAutoHook;
                C.Save();
            }

            ImGui.SetNextItemWidth(125);
            if (ImGui.InputText("Fisher Set Name", ref fishSetName, 15))
            {
                if (!string.IsNullOrWhiteSpace(fishSetName))
                {
                    C.FishSetName = fishSetName;
                    C.Save();
                }
                else
                {
                    DuoLog.Warning("Fish set name cannot be empty!");
                }
            }
            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Name of your fishing set to be able to swap to FSH if already not on the job");

            ImGui.Separator();
            ImGui.TreePop();
        }

        if (ImGui.TreeNode("Fishing Conditions"))
        {
            if (ImGui.Checkbox("##SetExactTime", ref setExactTime))
            {
                C.SetExactTime = setExactTime;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNode("Set Custom Time Condition"))
            {
                initialTime = Math.Clamp(initialTime, 5, 30);
                finalTime = Math.Clamp(finalTime, initialTime, 30);

                if (ImGui.BeginTable("TimeSettings", 2, ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.SizingFixedFit))
                {
                    ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed, 150);
                    ImGui.TableSetupColumn("Control", ImGuiTableColumnFlags.WidthStretch);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("Start Time (minutes):");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.SetNextItemWidth(150);
                    if (ImGui.SliderInt("##InitialTime", ref initialTime, 5, 30))
                    {
                        C.InitialTime = Math.Clamp(initialTime, 5, 30);
                        C.FinalTime = Math.Max(initialTime, finalTime);
                    }
                    ImGui.SameLine();
                    ImGuiComponents.HelpMarker("Minimum Time to stay at a single fishing Location");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("End Time (minutes):");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.SetNextItemWidth(150);
                    if (ImGui.SliderInt("##FinalTime", ref finalTime, initialTime, 30))
                    {
                        C.FinalTime = Math.Clamp(finalTime, initialTime, 30);
                    }
                    ImGui.SameLine();
                    ImGuiComponents.HelpMarker("Maximum Time to stay at a single fishing Location");

                    ImGui.EndTable();
                }

                ImGui.Text($"Time Range: {initialTime} — {finalTime} minutes");
                ImGui.Separator();
                ImGui.TreePop();
            }

            if (ImGui.Checkbox("##SetTurnInCondition", ref setTurnInCondition))
            {
                C.SetTurnInConditions = setTurnInCondition;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNode("Set Custom Turn In Conditions"))
            {
                ImGui.Text("Minimum Free Slots:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);

                if (ImGui.InputInt("##MinimumSlots", ref freeRemainingSlots, 1, 50))
                {
                    freeRemainingSlots = Math.Max(0, Math.Min(130, freeRemainingSlots));
                    C.FreeRemainingSlots = freeRemainingSlots;
                    C.Save();
                }
                ImGuiComponents.HelpMarker("Only turns in collectables when you have at least this many free inventory slots remaining.\n" +
                                           "* Set at 0 will only turn in when inventory is FULL.");

                ImGui.Text("AND/OR Conidition: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);
                if (ImGui.Button(useAndOperator ? "&&" : "||"))
                {
                    useAndOperator = !useAndOperator;
                    C.UseAndOperator = useAndOperator;
                    C.Save();
                }

                ImGui.Text("Minimum Fish Count:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);

                if (ImGui.InputInt("##MinimumFish", ref minimumFishToTurnin, 1, 50))
                {
                    minimumFishToTurnin = Math.Max(0, Math.Min(130, minimumFishToTurnin));
                    C.MinimumFishToTurnin = minimumFishToTurnin;
                    C.Save();
                }
                ImGuiComponents.HelpMarker("Only turns in collectables when you have at least this many fish in your inventory.\n" +
                                           "* Set at 0 will ignore this condition");
                ImGui.TreePop();
            }

            ImGui.Separator();
            ImGui.TreePop();
        }

        if (ImGui.TreeNode("Set Custom Waypoints"))
        {
            if (ImGui.Checkbox("##SetZorgorCondorWaypoints", ref setCustomZorgorWaypoints))
            {
                C.SetCustomZorgorWaypoints = setCustomZorgorWaypoints;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNode("Set Zorgor Condor Waypoints"))
            {
                for (int i = 0; i < C.CustomZorgorWaypoints.Count; i++)
                {
                    ImGui.PushID($"zorgor_{i}");
                    var wp = C.CustomZorgorWaypoints[i];
                    float[] pos = { wp.X, wp.Y, wp.Z };

                    ImGui.SetNextItemWidth(200);
                    if (ImGui.InputFloat3($"Waypoint {i + 1}", ref wp))
                    {
                        C.CustomZorgorWaypoints[i] = wp;
                        C.Save();
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Use Current Postition"))
                    {
                        C.CustomZorgorWaypoints[i] = Svc.ClientState.LocalPlayer.Position;
                        C.Save();
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Test"))
                    {
                        if (ZonesHelper.IsInZone(FishTable.Table[0].ZoneId))
                        {
                            TaskMount.Enqueue();
                            TaskFlight.Enqueue();
                            TaskMoveTo.Enqueue(C.CustomZorgorWaypoints[i], "Fishing Location", 1f, true);
                            TaskDisMount.Enqueue();
                            Plugin.taskManager.Enqueue(TaskGoToFishLocation.GoToFishLocation, 1000 * 20);
                            TaskQuitFish.Enqueue(true);
                        }
                        else
                        {
                            Generic.AllWarningEnqueue("Incorrect Zone Location");
                        }
                    }

                    ImGui.SameLine();
                    ImGui.BeginDisabled(C.CustomZorgorWaypoints.Count <= 2);
                    if (ImGui.Button("Remove"))
                    {
                        C.CustomZorgorWaypoints.RemoveAt(i);
                        C.Save();
                        ImGui.PopID();
                        break;
                    }
                    ImGui.EndDisabled();

                    ImGui.PopID();
                }
                
                if (ImGui.Button("Add"))
                {
                    C.CustomZorgorWaypoints.Add(Vector3.Zero);
                    C.Save();
                }

                ImGui.SameLine();
                if (ImGui.Button("Add Current Position"))
                {
                    C.CustomZorgorWaypoints.Add(Svc.ClientState.LocalPlayer.Position);
                    C.Save();
                }

                ImGui.Separator();
                ImGui.TreePop();
            }

            if (ImGui.Checkbox("##SetFleetingBrandWaypoints", ref setCustomFleetingBrandWaypoints))
            {
                C.SetCustomFleetingBrandWaypoints = setCustomFleetingBrandWaypoints;
                C.Save();
            }
            ImGui.SameLine();

            if (ImGui.TreeNode("Set Fleeting Brand Waypoints"))
            {
                for (int i = 0; i < C.CustomFleetingBrandWaypoints.Count; i++)
                {
                    ImGui.PushID($"zorgor_{i}");
                    var wp = C.CustomFleetingBrandWaypoints[i];
                    float[] pos = { wp.X, wp.Y, wp.Z };

                    ImGui.SetNextItemWidth(250);
                    if (ImGui.InputFloat3($"Waypoint {i + 1}", ref wp))
                    {
                        C.CustomFleetingBrandWaypoints[i] = wp;
                        C.Save();
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Use Current Postition"))
                    {
                        C.CustomFleetingBrandWaypoints[i] = Svc.ClientState.LocalPlayer.Position;
                        C.Save();
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Test"))
                    {
                        if (ZonesHelper.IsInZone(FishTable.Table[1].ZoneId))
                        {
                            TaskMount.Enqueue();
                            TaskFlight.Enqueue();
                            TaskMoveTo.Enqueue(C.CustomFleetingBrandWaypoints[i], "Fishing Location", 1f, true);
                            TaskDisMount.Enqueue();
                            Plugin.taskManager.Enqueue(TaskGoToFishLocation.GoToFishLocation, 1000 * 20);
                            TaskQuitFish.Enqueue(true);
                        }
                        else
                        {
                            Generic.AllWarningEnqueue("Incorrect Zone Location");
                        }
                    }

                    ImGui.SameLine();
                    ImGui.BeginDisabled(C.CustomFleetingBrandWaypoints.Count <= 2);
                    if (ImGui.Button("Remove"))
                    {
                        C.CustomFleetingBrandWaypoints.RemoveAt(i);
                        C.Save();
                        ImGui.PopID();
                        break;
                    }
                    ImGui.EndDisabled();
                    ImGui.PopID();
                }

                if (ImGui.Button("Add"))
                {
                    C.CustomFleetingBrandWaypoints.Add(Vector3.Zero);
                    C.Save();
                }

                ImGui.SameLine();
                if (ImGui.Button("Add Current Position"))
                {
                    C.CustomFleetingBrandWaypoints.Add(Svc.ClientState.LocalPlayer.Position);
                    C.Save();
                }
                ImGui.TreePop();
            }
            ImGui.Separator();
            ImGui.TreePop();
        }

        if (ImGui.TreeNode("Debug"))
        {
            if (ImGui.Checkbox("Enable Debug", ref Debug))
            {
                C.DEBUG = Debug;
                C.Save();
            }
            if (ImGui.Checkbox("Additonal Logging", ref additionalLogging))
            {
                C.AdditionalLogging = additionalLogging;
                C.Save();
            }
            ImGui.Separator();
            ImGui.TreePop();
        }
    }
}
