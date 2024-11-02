using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.App.EventSystem.Events
{
    public class PlayerChangeSubAreaEvent : GameEvent
    {
        public string oldSubArea;
        public string newSubArea;

        public PlayerChangeSubAreaEvent(string oldSubArea, string newSubArea) {
            this.oldSubArea = oldSubArea;
            this.newSubArea = newSubArea;
        }
    }
}
