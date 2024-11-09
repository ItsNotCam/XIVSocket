using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;

using System.Linq;

using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace XIVSocket.Gui.Windows;

public class MainWindow : Window, IDisposable
{
    private string GoatImagePath;
    private Plugin Plugin;

    private string[] playerAttributeNames;
    private int[] playerAttributes;

    private Lumina.Excel.ExcelSheet<Item> Items;
    private Lumina.Excel.ExcelSheet<Recipe> Recipes;
    private Dictionary<uint, List<Recipe>> RecipesByResult;

    private string itemInfo;
    private string itemName;

    private string searchItem;


    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string goatImagePath)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollWithMouse)
    {
        searchItem = "";

        Items = Plugin.DataManager.GetExcelSheet<Item>()!;

        RecipesByResult = new Dictionary<uint, List<Recipe>>();
        var recipes = Plugin.DataManager.GetExcelSheet<Recipe>()!;
        recipes.ToList().ForEach(recipe =>
        {
            var rowId = recipe.ItemResult.Row;
            if (RecipesByResult.ContainsKey(rowId))
            {
                RecipesByResult[rowId].Add(recipe);
            }
            else
            {
                RecipesByResult.Add(rowId, new List<Recipe>() { recipe });
            }
        });


        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };


        GoatImagePath = goatImagePath;
        Plugin = plugin;

        unsafe
        {
            var playerStatePtr = PlayerState.Instance();
            playerAttributes = playerStatePtr->Attributes.ToArray();
        }

        playerAttributeNames = [
            "??", "Strength", "Dexterity", "Vitality", "Intelligence", "Mind"
        ];
    }

    public void Dispose() { }

    void GetRecipeInfo(uint itemId, uint depth)
    {
        if (depth > 10)
        {
            itemInfo += "\nMaximum item depth exceeded";
            return;
        }

        List<Recipe> recipes = null;
        if (!RecipesByResult.TryGetValue(itemId, out recipes))
        {
            return;
        }

        var ingredients = recipes.First().UnkData5.Where(x => x.AmountIngredient > 0).ToList();
        foreach (var ingredient in ingredients)
        {
            var ingredientId = (uint)ingredient.ItemIngredient;
            int amount = ingredient.AmountIngredient;
            var row = Items.GetRow(ingredientId);
            if (row == null)
            {
                continue;
            }

            string curItemName = row.Name;
            for (var i = 0; i < depth; i++)
            {
                itemInfo += "\t";
            }
            itemInfo += amount + " " + curItemName + "\n";
            GetRecipeInfo(ingredientId, depth + 1);
        }
    }

    void GetItemInfo()
    {
        Plugin.ChatGui.Print("Searching for " + searchItem);
        var searchItemObj = Items.FirstOrDefault(i => i.Name.ToString().Equals(searchItem), null);
        if (searchItemObj == null)
        {
            Plugin.ChatGui.Print("No such item exists");
        }
        else
        {
            Plugin.ChatGui.Print("found " + searchItemObj.Name);

            itemInfo = "";
            itemName = searchItemObj.Name;
            GetRecipeInfo(searchItemObj.RowId, 0);
        }
    }

    public override void Draw()
    {
        var netMgr = Plugin.NetworkManager;
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
            Plugin.GameManager.GetLocation().region.name,
            Plugin.GameManager.GetLocation().territory.name,
            Plugin.GameManager.GetLocation().area.name,
            Plugin.GameManager.GetLocation().subArea.name
        ];
        ImGui.Text(string.Join(", ", txt.ToList()));
        ImGui.Spacing();
        ImGui.Text(Plugin.GameManager.GetLocation().ToString());

        if (ImGui.Button("Show Settings"))
        {
            Plugin.ToggleConfigUI();
        }

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        var goatImage = Plugin.TextureProvider.GetFromFile(GoatImagePath).GetWrapOrDefault();
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

        ImGui.Spacing();

        for (var i = 1; i < playerAttributeNames.Length; i++)
        {
            ImGui.Text(playerAttributeNames[i] + ": " + playerAttributes[i]);
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();

        ImGui.Text("Search for an item");
        ImGui.InputText("", ref searchItem, 100);

        if (ImGui.Button("Get the item"))
        {
            GetItemInfo();
        }

        if (itemName != null)
        {
            ImGui.Text(itemName);
        }
        else
        {
            ImGui.Text("No Item Name");
        }

        if (itemInfo != null)
        {
            ImGui.Text(itemInfo);
        }
        else
        {
            ImGui.Text("No Item Info");
        }
    }
}
