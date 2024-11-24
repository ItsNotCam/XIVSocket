using System;
using XIVEvents.Models;
using XIVSocket.Lib.Network;
using XIVSocket.Lib.Network.Socket;

namespace XIVSocket.App.Network
{
    public class NetworkManager : IDisposable
    {
        private UDPClient udpClient { get; } = null!;
        private TCPServer tcpServer { get; } = null!;

        private EzRouteHandler ezRouteHandler { get; } = null;
        private EzWsServer ezWsServer { get; } = null!;

        private XIVSocketPlugin plugin { get; }

        public NetworkManager(XIVSocketPlugin plugin)
        {
            //udpClient = new UDPClient(27001);
            //tcpServer = new TCPServer(58008);
            this.plugin = plugin;
            ezWsServer = new EzWsServer(50085, this);
            ezRouteHandler = new EzRouteHandler(this.plugin);
        }

        public bool SocketRunning() => this.ezWsServer.IsRunning();

        public void SendUdpMessage(string message)
        {
            if (udpClient == null) {
                return;
            }

            udpClient.SendMessageAsync(message);
        }

        public void SendMovementMessage(LocationModel location) {
            if(udpClient == null) {
                return;
            } 

            float x = location.position.X;
            float y = location.position.Y;

            byte[] data = new byte[9];
            data[0] = 0x01;

            BitConverter.GetBytes(x).CopyTo(data, 1);
            BitConverter.GetBytes(y).CopyTo(data, 5);
            //udpClient.SendBytesAsync(data);
        }

		public string? HandleRequest(EzFlag flag, string payload) => ezRouteHandler.Handle(flag, payload);

        public void Dispose()
        {
            if(udpClient != null) {
                udpClient.Dispose();
            }

            if(tcpServer != null) {
                tcpServer.Dispose();
            }

            if (ezWsServer != null) {
                ezWsServer.Dispose();
            }
        }
    }
}
