using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace XIVSocket.Gui.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("A Wonderful Configuration Window###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 90);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (Configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var openOnLaunch = Configuration.OpenOnLaunch;
        if (ImGui.Checkbox("Open on Launch", ref openOnLaunch))
        {
            Configuration.OpenOnLaunch = openOnLaunch;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            Configuration.Save();
        }

        // can't ref a property, so use a local copy
        var logToSocket = Configuration.TransmitLogsToSocket;
        if (ImGui.Checkbox("Transmit Logs to Socket", ref logToSocket))
        {
            Configuration.TransmitLogsToSocket = logToSocket;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            Configuration.Save();
        }
    }
}
