using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.App.EventSystem.Events
{
    public class PlayerChangeTerritoryEvent : GameEvent
    {
        public string oldTerritory;
        public string newTerritory;

        public PlayerChangeTerritoryEvent(string oldTerritory, string newTerritory) { 
            this.newTerritory = newTerritory;
            this.oldTerritory = oldTerritory;
        }
    }
}
