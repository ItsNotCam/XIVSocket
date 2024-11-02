using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.App.EventSystem.Events
{
    public class PlayerChangeRegionEvent : GameEvent
    {
        public string oldRegion;
        public string newRegion;

        public PlayerChangeRegionEvent(string oldRegion, string newRegion) {
            this.oldRegion = oldRegion;
            this.newRegion = newRegion;
        }
    }
}
