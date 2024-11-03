using System;
using System.Threading.Tasks;

namespace XIVSocket.App.Network
{
    public class NetworkManager : IDisposable
    {
        internal UDPSocket udpSock { get; } = null!;

        public NetworkManager()
        {
            udpSock = new UDPSocket(27000, "127.0.0.1", 27001, "127.0.0.1");
        }

        public void StartSocket()
        {
            if (udpSock == null)
            {
                Plugin.PluginLogger.Error("Socket cannot start - it has not been initialized");
                return;
            }

            if (udpSock.isRunning())
            {
                Plugin.PluginLogger.Error("Socket already running.");
            }
            else
            {
                Plugin.PluginLogger.Debug("Starting Socket");
                try
                {
                    udpSock.Start();
                    Plugin.PluginLogger.Debug("Started Socket");
                }
                catch (Exception e)
                {
                    Plugin.PluginLogger.Error($"Could not start socket {e.Message}");
                }
                
            }
        }

        public void StopSocket()
        {
            if (udpSock != null && udpSock.isRunning())
            {
                udpSock.Dispose();
            }
            else
            {
                Plugin.PluginLogger.Info("Socket already stopped.");
            }
        }

        public bool SocketRunning()
        {
            return udpSock != null && udpSock.isRunning();
        }

        public void SendUdpMessage(string message)
        {
            _ = udpSock.SendMessageAsync(message);
        }

        public void Dispose()
        {
            StopSocket();
        }
    }
}
