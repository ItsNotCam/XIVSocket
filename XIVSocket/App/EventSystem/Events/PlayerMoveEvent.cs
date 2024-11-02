using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.App.EventSystem.Events
{
    public class PlayerMoveEvent : GameEvent
    {
        public Vector3 oldPosition;
        public Vector3 newPosition;

        public PlayerMoveEvent(Vector3 oldPosition, Vector3 newPosition) { 
            this.oldPosition = oldPosition;
            this.newPosition = newPosition;
        }
    }
}
