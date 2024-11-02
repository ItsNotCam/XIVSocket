using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.EventSystem.Events;

namespace XIVSocket.App.EventSystem.Listeners
{
    internal class PlayerChangeSubAreaListener : IEventListener<PlayerChangeSubAreaEvent>
    {
        Plugin plugin;

        public PlayerChangeSubAreaListener(Plugin plugin) {
            this.plugin = plugin;
        }

        public void OnEvent(PlayerChangeSubAreaEvent e)
        {
            string message = $"Player changed sub area: {e.oldSubArea} -> {e.newSubArea}";
            plugin.NetworkManager.SendMessage(message);

            plugin.Logger.Log(message, Logging.Logger.LogLevel.INFO);
        }
    }
}
