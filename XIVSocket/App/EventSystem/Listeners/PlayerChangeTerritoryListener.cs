using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.EventSystem.Events;

namespace XIVSocket.App.EventSystem.Listeners
{
    internal class PlayerChangeTerritoryListener : IEventListener<PlayerChangeTerritoryEvent>
    {
        Plugin plugin;

        public PlayerChangeTerritoryListener(Plugin plugin) {
            this.plugin = plugin;
        }

        public void OnEvent(PlayerChangeTerritoryEvent e)
        {
            string message = $"Player changed territory: {e.oldTerritory} -> {e.newTerritory}";
            plugin.NetworkManager.SendMessage(message);

            plugin.Logger.Log(message, Logging.Logger.LogLevel.INFO);
        }
    }
}
