using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using XIVSocket.Windows;
using System.IO;
using XIVSocket.Network;
using Dalamud.Game.Network.Structures.InfoProxy;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using XIVSocket.Aetherite.Managers;
using XIVSocket.Dispatchers.Listeners;


/*
 * 
 * public int GetCurrentExperience()
{
    var playerState = PlayerState.Instance();
    if (playerState->IsLoaded == 0)
        return 0;

    var classJobId = playerState->CurrentClassJobId;
    var classJobRow = dataManager.GetExcelSheet<ClassJob>()!.GetRow(classJobId);
    var currentExperience = playerState->ClassJobExperience[classJobRow.ExpArrayIndex];
    return currentExperience;
}
 * yeah, you can use the IFramework service and listen to the Update event. it runs every frame
 * 
 * you can also get the experience from the exp bar via AgentHUD.Instance()->ExpCurrentExperience
also has ExpNeededExperience and ExpRestedExperience, as well as ExpClassJobId

PlayerState.Instance()->ClassJobExperience
 */



namespace XIVSocket;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static IPluginLog PluginLogger { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;


    private const string CommandName = "/xivsocket";

    public Configuration Configuration { get; init; }

    /* Window */
    public readonly WindowSystem WindowSystem = new("XIVSocket");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    /* Listeners */
    //private CharacterListener InventoryListener { get; init; }

    /* Network */
    public NetworkManager NetworkManager { get; private set; } = null;

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        string goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "OhGod.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        //InventoryListener = new CharacterListener(this);//, framework, dataManager, clientState);
        //InventoryListener.Start();

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        AetheryteManager.Load();
        NetworkManager.StartSocket();

        // always open on start
        if(Configuration.OpenOnLaunch) {
            ToggleMainUI();
        }
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);

        StopSocket();
        //InventoryListener.Dispose();
    }

    private void OnCommand(string command, string args) {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();

    public void GetClosestAetherite() {
        uint worldId = ClientState.LocalPlayer!.CurrentWorld.Id;
        World world = DataManager.GetExcelSheet<World>()!.GetRow(worldId)!;

        uint mapId = ClientState.MapId;
        Lumina.Excel.GeneratedSheets.Map map = DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Map>()!.GetRow(mapId)!;
        uint regionId = map.PlaceNameRegion.Row;
        uint placeId = map.PlaceName.Row;

        PlaceName place = DataManager.GetExcelSheet<PlaceName>()!.GetRow(regionId)!;
        PlaceName place2 = DataManager.GetExcelSheet<PlaceName>()!.GetRow(placeId)!;

        AetheryteManager.TryFindAetheryteByMapName(
            place2.Name, true, out TeleportInfo info
        );

        AetheryteManager.AvailableAetherytes.ForEach(ad => {
            uint aetheryteId = ad.AetheryteId;
            Aetheryte a = DataManager.GetExcelSheet<Aetheryte>().GetRow(aetheryteId);
            string nodeName = a.PlaceName.Value.NameNoArticle;
            string message = $"{world.Name} | {place.Name} @ {place2.Name} | closest: {nodeName}";

            sock.SendMessageAsync(message);
            PluginLogger.Debug(message);
        });
    }
}
