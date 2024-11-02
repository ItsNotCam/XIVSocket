using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.EventSystem.Events;

namespace XIVSocket.App.EventSystem.Listeners
{
    internal class PlayerMoveListener : IEventListener<PlayerMoveEvent>
    {
        Plugin plugin;

        public PlayerMoveListener(Plugin plugin) {
            this.plugin = plugin;
        }

        public void OnEvent(PlayerMoveEvent e)
        {
            string message = $"Player moved: {e.oldPosition.ToString()} -> {e.newPosition.ToString()}";
            plugin.Logger.Log(message, Logging.Logger.LogLevel.VERBOSE);
        }
    }
}
