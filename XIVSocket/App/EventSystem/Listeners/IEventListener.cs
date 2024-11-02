using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.EventSystem.Events;

namespace XIVSocket.App.EventSystem.Listeners
{
    public interface IEventListener<T> where T : GameEvent
    {
        void OnEvent(T e);
    }
}
