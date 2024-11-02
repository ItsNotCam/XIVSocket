using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System.IO;
using XIVSocket.Gui.Windows;
using XIVSocket.App.Network;
//using XIVSocket.App.EventSystem;
using XIVSocket.App.Logging;
//using XIVSocket.App.EventSystem.Listeners;
//using XIVSocket.App.EventSystem.Events;
using XIVEvents;
using XIVSocket.Lib.Listeners;
using XIVEvents.Models;

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

    // ok

    /* Network */
    public NetworkManager NetworkManager { get; }

    /* Events */
    public EventManager EventManager { get; }
    public EventPoller EventPoller { get; }

    /* State */
    public LocationModel location;

    public Logger Logger { get; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Services>();

        /* Configuration */
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        string goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "OhGod.png");

        /* Register Windows */
        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry ofw this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;


        /*******************/
        /*     MY SHIT    */
        /*******************/

        /* Network */
        NetworkManager = new NetworkManager();
        NetworkManager.StartSocket();

        /* Logging */
        //Logger = new Logger(NetworkManager, Configuration.TransmitLogsToSocket);

        /* Events */
        EventManager = new EventManager();
        EventManager.RegisterListeners(new PlayerMoveListener(this));

        EventPoller = new EventPoller(EventManager);
        EventPoller.Start();

        /**********/
        /* FINITO */
        /***********/

        /* State */
        location = new LocationModel();

        if (Configuration.OpenOnLaunch) {
            ToggleMainUI();
        }
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);

        if(NetworkManager != null) {
            NetworkManager.Dispose();
        }
        //EventManager.Dispose();
    }

    private void OnCommand(string command, string args) {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();

    //public void GetClosestAetherite() {
    //    uint worldId = ClientState.LocalPlayer!.CurrentWorld.Id;
    //    World world = DataManager.GetExcelSheet<World>()!.GetRow(worldId)!;

    //    uint mapId = ClientState.MapId;
    //    Lumina.Excel.GeneratedSheets.Map map = DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Map>()!.GetRow(mapId)!;
    //    uint regionId = map.PlaceNameRegion.Row;
    //    uint placeId = map.PlaceName.Row;

    //    PlaceName place = DataManager.GetExcelSheet<PlaceName>()!.GetRow(regionId)!;
    //    PlaceName place2 = DataManager.GetExcelSheet<PlaceName>()!.GetRow(placeId)!;

    //    AetheryteManager.TryFindAetheryteByMapName(
    //        place2.Name, true, out TeleportInfo info
    //    );

    //    AetheryteManager.AvailableAetherytes.ForEach(ad => {
    //        uint aetheryteId = ad.AetheryteId;
    //        Aetheryte a = DataManager.GetExcelSheet<Aetheryte>().GetRow(aetheryteId);
    //        string nodeName = a.PlaceName.Value.NameNoArticle;
    //        string message = $"{world.Name} | {place.Name} @ {place2.Name} | closest: {nodeName}";
            
    //        NetworkManager.SendMessage(message);
    //        PluginLogger.Debug(message);
    //    });
    //}
}
