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

        private EzWsServer ezWsServer { get; } = null!;

        private Plugin plugin;

        public NetworkManager(Plugin plugin)
        {
            //udpClient = new UDPClient(27001);
            //tcpServer = new TCPServer(58008);
            this.plugin = plugin;

            ezWsServer = new EzWsServer(50085, this);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //if(tcpServer != null && tcpServer.isRunning) {
            //    tcpServer.Dispose();
            //    ezWsServer.Dispose();
            //}
            if(ezWsServer != null) {
                ezWsServer.Dispose();
            }
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

		public string HandleRequest(uint packetId, uint packetFlag, string payload)
		{
            EzFlag flag = (EzFlag)packetFlag;
            switch (flag)
			{
				case EzFlag.JOB_MAIN:
                    JobModel job = plugin.XIVStateManager.getMainJob();
                    return Newtonsoft.Json.JsonConvert.SerializeObject(job);
            }

			return null;
		}

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
