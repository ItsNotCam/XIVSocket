using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.EventSystem.Events;

namespace XIVSocket.App.EventSystem.Listeners
{
    internal class PlayerChangeRegionListener : IEventListener<PlayerChangeRegionEvent>
    {
        Plugin plugin;

        public PlayerChangeRegionListener(Plugin plugin) {
            this.plugin = plugin;
        }

        public void OnEvent(PlayerChangeRegionEvent e)
        {
            string message = $"Player changed region: {e.oldRegion} -> {e.newRegion}";
            plugin.NetworkManager.SendMessage(message);

            plugin.Logger.Log(message, Logging.Logger.LogLevel.INFO);
        }
    }
}
