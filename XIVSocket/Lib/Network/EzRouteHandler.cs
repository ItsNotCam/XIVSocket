using System;
using XIVEvents.Models;
using XIVSocket.Lib.Network.Socket;

namespace XIVSocket.Lib.Network;

internal class EzRouteHandler
{
    private XIVSocketPlugin plugin;

    public EzRouteHandler(XIVSocketPlugin plugin) {
        this.plugin = plugin;
    }

    public string? Handle(EzFlag flag, string payload)
    {
        switch (flag)
        {
            case EzFlag.JOB_MAIN:
                var mainJobStr = HandleGetJobMain();
                return mainJobStr;
            case EzFlag.TIME:
                var gameTime = HandleGetGameTime();
                return gameTime;
        }

        return null;
    }
    private string? HandleGetJobMain()
    {
        JobModel? job = null;
        try {
            job = plugin.XIVStateManager.getCurrentJob();
        } catch (Exception e) {
            XIVSocketPlugin.PluginLogger.Warning($"Error getting main job - {e.Message}");
        }

        if (job == null) {
            XIVSocketPlugin.PluginLogger.Warning("Main job is null");
        }

        string? result = null;
        try {
            result = Newtonsoft.Json.JsonConvert.SerializeObject(job).ToLower();
            XIVSocketPlugin.PluginLogger.Info($"Sending job data: {result}");
        } catch (Exception e) {
            XIVSocketPlugin.PluginLogger.Warning($"Error serializing job - {e.Message}");
        }

        return result;
    }
    
    private string? HandleGetGameTime() => plugin.XIVStateManager.GetGameTime();
}
