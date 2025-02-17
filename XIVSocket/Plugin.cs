using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System.IO;
using XIVSocket.Gui.Windows;
using XIVSocket.App.Network;
using XIVSocket.App.Logging;
using XIVEvents;
using XIVSocket.Lib.Listeners;
using XIVEvents.Models;

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

    public NetworkManager NetworkManager { get; }
    public XIVEventManager GameManager { get; }
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

        /* Events */
        GameManager = new XIVEventManager();
        GameManager.RegisterListeners(new PlayerMoveListener(this));
        GameManager.Listen();

        /**********/
        /* FINITO */
        /***********/
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

        NetworkManager.Dispose();
        GameManager.Dispose();
    }

    private void OnCommand(string command, string args) => ToggleMainUI();
  

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
