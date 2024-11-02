using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.App.EventSystem.Events
{
    public class PlayerChangeAreaEvent : GameEvent
    {
        public string oldArea;
        public string newArea;

        public PlayerChangeAreaEvent(string oldArea, string newArea) { 
            this.newArea = newArea;
            this.oldArea = oldArea;
        }
    }
}
