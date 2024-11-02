using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.EventSystem.Events;

namespace XIVSocket.App.EventSystem.Listeners
{
    internal class PlayerChangeAreaListener : IEventListener<PlayerChangeAreaEvent>
    {
        Plugin plugin;

        public PlayerChangeAreaListener(Plugin plugin) {
            this.plugin = plugin;
        }

        public void OnEvent(PlayerChangeAreaEvent e)
        {
            string message = $"Player changed areas: {e.oldArea} -> {e.newArea}";
            plugin.NetworkManager.SendMessage(message);

            plugin.Logger.Log(message, Logging.Logger.LogLevel.INFO);
        }
    }
}
