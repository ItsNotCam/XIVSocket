using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.EventSystem.Pollers;
using XIVSocket.App.Models;

namespace XIVSocket.App.EventSystem
{
    public class EventPoller
    {
        private readonly EventManager eventManager;

        public LocationPoller LocationPoller { get; }

        public EventPoller(EventManager eventManager)
        {
            this.eventManager = eventManager;
            LocationPoller = new LocationPoller(eventManager);
        }

        public void Start()
        {
            Plugin.Framework.Update += tick;
        }

        public void Stop() {
            Plugin.Framework.Update -= tick;
        }

        public void Dispose() {
            Stop();
        }

        private void tick(IFramework framework) {
            LocationPoller.poll(framework);
        }
    }
}
