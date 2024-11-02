namespace XIVSocket.Network
{
    public class NetworkManager
    {
        internal UDPSocket sock { get; private set; } = null!;

        public NetworkManager() {
            sock = new UDPSocket(27000, "127.0.0.1", 27001, "127.0.0.1");
        }

        public void StartSocket()
        {
            if (sock == null) {
                Plugin.PluginLogger.Error("Socket cannot start - it has not been initialized");
                return;
            }

            if (sock.isRunning()) {
                Plugin.PluginLogger.Error("Socket already running.");
            } else {
                sock.Start();
            }
        }

        public void StopSocket()
        {
            if (sock != null && sock.isRunning()) {
                sock.Dispose();
            } else {
                Plugin.PluginLogger.Info("Socket already stopped.");
            }
        }

        public bool SocketRunning() {
            return sock != null && sock.isRunning();
        }
    }
}
