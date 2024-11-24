using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;

using System.Linq;

using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace XIVSocket.Gui.Windows;

public class MainWindow : Window, IDisposable
{
    private string goatImagePath;
    private XIVSocketPlugin plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(XIVSocketPlugin plugin, string goatImagePath)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };


        this.goatImagePath = goatImagePath;
        this.plugin = plugin;
    }

    public void Dispose() { }

    void GetRecipeInfo(uint itemId, uint depth)
    {
        //if (depth > 10)
        //{
        //    itemInfo += "\nMaximum item depth exceeded";
        //    return;
        //}

        //List<Recipe> recipes = null;
        //if (!RecipesByResult.TryGetValue(itemId, out recipes))
        //{
        //    return;
        //}

        //var ingredients = recipes.First().Unknown1.Where(x => x.AmountIngredient > 0).ToList();
        //foreach (var ingredient in ingredients)
        //{
        //    var ingredientId = (uint)ingredient.ItemIngredient;
        //    int amount = ingredient.AmountIngredient;
        //    var row = Items.GetRow(ingredientId);
        //    if (row == null)
        //    {
        //        continue;
        //    }

        //    string curItemName = row.Name;
        //    for (var i = 0; i < depth; i++)
        //    {
        //        itemInfo += "\t";
        //    }
        //    itemInfo += amount + " " + curItemName + "\n";
        //    GetRecipeInfo(ingredientId, depth + 1);
        //}
    }

    public override void Draw()
    {
        var netMgr = plugin.NetworkManager;
        if (ImGui.Button((netMgr.SocketRunning() ? "Stop" : "Start") + " Socket"))
        {
            if (netMgr.SocketRunning())
            {
                //netMgr.Dispose();
            }
            else
            {
                //netMgr.StartSocket();
            }
        }

        string[] txt = [
            plugin.XIVStateManager.GetLocation().region.name,
            plugin.XIVStateManager.GetLocation().territory.name,
            plugin.XIVStateManager.GetLocation().area.name,
            plugin.XIVStateManager.GetLocation().subArea.name
        ];
        ImGui.Text(string.Join(", ", txt.ToList()));
        ImGui.Spacing();
        ImGui.Text(plugin.XIVStateManager.GetLocation().ToString());

        if (ImGui.Button("Show Settings"))
        {
            plugin.ToggleConfigUI();
        }

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        var goatImage = XIVSocketPlugin.TextureProvider.GetFromFile(goatImagePath).GetWrapOrDefault();
        if (goatImage != null)
        {
            ImGuiHelpers.ScaledIndent(55f);
            ImGui.Image(goatImage.ImGuiHandle, new Vector2(goatImage.Width, goatImage.Height));
            ImGuiHelpers.ScaledIndent(-55f);
        }
        else
        {
            ImGui.Text("Image not found.");
        }
    }
}
