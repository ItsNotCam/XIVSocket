using Dalamud.Configuration;
using System;

namespace XIVSocket;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool OpenOnLaunch { get; set; } = false;
    public bool TransmitLogsToSocket { get; internal set; }

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
