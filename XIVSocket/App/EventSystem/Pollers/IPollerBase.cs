using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.Logging;

namespace XIVSocket.App.EventSystem.Pollers
{
    public interface IPollerBase
    {
        public void poll(IFramework framework) { }
    }
}
