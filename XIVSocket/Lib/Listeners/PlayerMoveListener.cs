//using System.Threading;
using Windows.UI.ApplicationSettings;
using XIVEvents;
using XIVEvents.Events.Location;
using XIVEvents.Interfaces;
using XIVEvents.Models;

namespace XIVSocket.Lib.Listeners;

internal class PlayerMoveListener : IListener
{
    XIVSocketPlugin plugin;
    //private static Mutex mut = new Mutex();

    public PlayerMoveListener(XIVSocketPlugin plugin) { 
        this.plugin = plugin;
    }

    [EventListener]
    public void OnPlayerMove(PlayerMoveEvent e)
    {
        var message = $"M: {e.NewLocation.position.ToString()}";
        XIVSocketPlugin.PluginLogger.Verbose(message);
        //plugin.NetworkManager.SendUdpMessage(message);

        plugin.NetworkManager.SendMovementMessage(e.NewLocation);
    }

    [EventListener]
    public void OnPlayerChangeTerritory(PlayerChangeTerritoryEvent e)
    {
        PlaceModel oldTerritory = e.OldLocation.territory;
        PlaceModel newTerritory = e.NewLocation.territory;

        string message = "T: ";
        if (oldTerritory.name == null || oldTerritory.name.Length < 1) {
            message += $"Entered {newTerritory.name}";
        } else if (newTerritory.name == null || newTerritory.name.Length < 1) {
            message += $"Exited {oldTerritory.name}";
        } else {
            message += $"{oldTerritory.name} -> {newTerritory.name}";
        }

        XIVSocketPlugin.PluginLogger.Info(message);
        //plugin.NetworkManager.SendUdpMessage(message);

        //plugin.NetworkManager.SendMovementMessage(e.NewLocation);
    }

    [EventListener]
    public void OnPlayerChangeSubArea(PlayerChangeSubAreaEvent e)
    {
        PlaceModel oldSubArea = e.OldLocation.subArea;
        PlaceModel newSubArea = e.NewLocation.subArea;

        string message = "S: ";
        if (oldSubArea.name == null || oldSubArea.name.Length < 1) {
            message += $"Entered {newSubArea.name}";
        } else if (newSubArea.name == null || newSubArea.name.Length < 1) {
            message += $"Exited {oldSubArea.name}";
        } else {
            message += $"{oldSubArea.name} -> {newSubArea.name}";
        }

        XIVSocketPlugin.PluginLogger.Info(message);
        //plugin.NetworkManager.SendUdpMessage(message);

        //plugin.NetworkManager.SendMovementMessage(e.NewLocation);
    }

    [EventListener]
    public void OnPlayerChangeRegion(PlayerChangeRegionEvent e)
    {
        PlaceModel oldRegion = e.OldLocation.region;
        PlaceModel newRegion = e.NewLocation.region;

        string message = "R: ";
        if (oldRegion.name == null || oldRegion.name.Length < 1) {
            message += $"Entered {newRegion.name}";
        } else if (newRegion.name == null || newRegion.name.Length < 1) {
            message += $"Exited {oldRegion.name}";
        } else {
            message += $"{oldRegion.name} -> {newRegion.name}";
        }

        XIVSocketPlugin.PluginLogger.Info(message);
        //plugin.NetworkManager.SendUdpMessage(message);

        //plugin.NetworkManager.SendMovementMessage(e.NewLocation);
    }

    [EventListener]
    public void OnPlayerChangeArea(PlayerChangeAreaEvent e)
    {
        PlaceModel oldArea = e.OldLocation.area;
        PlaceModel newArea = e.NewLocation.area;

        string message = "A: ";
        if (oldArea.name == null || oldArea.name.Length < 1) {
            message += $"Entered {newArea.name}";
        } else if (newArea.name == null || newArea.name.Length < 1) {
            message += $"Exited {oldArea.name}";
        } else {
            message += $"{oldArea.name} -> {newArea.name}";
        }

        XIVSocketPlugin.PluginLogger.Info(message);
        //plugin.NetworkManager.SendUdpMessage(message);

        //plugin.NetworkManager.SendMovementMessage(e.NewLocation);
    }
}
