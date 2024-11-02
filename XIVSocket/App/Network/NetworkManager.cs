using System;
using System.Threading.Tasks;

namespace XIVSocket.App.Network
{
    public class NetworkManager : IDisposable
    {
        internal UDPSocket sock { get; } = null!;

        public NetworkManager()
        {
            sock = new UDPSocket(27000, "127.0.0.1", 27001, "127.0.0.1");
        }

        public void StartSocket()
        {
            if (sock == null)
            {
                Plugin.PluginLogger.Error("Socket cannot start - it has not been initialized");
                return;
            }

            if (sock.isRunning())
            {
                Plugin.PluginLogger.Error("Socket already running.");
            }
            else
            {
                sock.Start();
            }
        }

        public void StopSocket()
        {
            if (sock != null && sock.isRunning())
            {
                sock.Dispose();
            }
            else
            {
                Plugin.PluginLogger.Info("Socket already stopped.");
            }
        }

        public bool SocketRunning()
        {
            return sock != null && sock.isRunning();
        }

        public void SendMessage(string message)
        {
            sock.SendMessageAsync(message).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Plugin.PluginLogger.Error($"Failed to send message {t.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Dispose()
        {
            StopSocket();
        }
    }
}
