using System;
using XIVEvents.Models;
using XIVSocket.Lib.Network;

namespace XIVSocket.App.Network
{
    public class NetworkManager : IDisposable
    {
        internal UDPClient udpClient { get; } = null!;
        internal TCPServer tcpServer { get; } = null!;

        public NetworkManager()
        {
            //udpClient = new UDPClient(27001);
            tcpServer = new TCPServer(58008);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if(tcpServer != null && tcpServer.isRunning) {
                tcpServer.Dispose();
            }
        }

        public bool SocketRunning() => this.tcpServer.isRunning;

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
            udpClient.SendBytesAsync(data);
        }

        public void Dispose()
        {
            if(udpClient != null) {
                udpClient.Dispose();
            }

            if(tcpServer != null) {
                tcpServer.Dispose();
            }
        }
    }
}

/*
 * 

        public void StartSocket()
        {
            //if (tcpServer == null)
            //{
            //    Plugin.PluginLogger.Error("Socket cannot start - it has not been initialized");
            //    return;
            //}

            //if (tcpServer.isRunning())
            //{
            //    Plugin.PluginLogger.Error("Socket already running.");
            //}
            //else
            //{
            //    Plugin.PluginLogger.Debug("Starting Socket");
            //    try
            //    {
            //        udpClient.Start();
            //        Plugin.PluginLogger.Debug("Started Socket");
            //    }
            //    catch (Exception e)
            //    {
            //        Plugin.PluginLogger.Error($"Could not start socket {e.Message}");
            //    }
                
            //}
        }
*/
