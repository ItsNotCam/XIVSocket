using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XIVEvents.Models;
using XIVSocket.Lib.Network.EzProto;

namespace XIVSocket.App.Network
{
    public class NetworkManager : IDisposable
    {
        internal UDPSocket udpSock { get; } = null!;

        public NetworkManager()
        {
            udpSock = new UDPSocket(27000, "127.0.0.1", 27001, "127.0.0.1");
        }

        //public void RunEcho()
        //{
        //    // createa a new task that runs an echo to port 27001 every 1 second
        //    Task.Run(async () =>
        //    {
        //        while (true)
        //        {
        //            SendUdpMessage("Echo");
        //            await Task.Delay(1000);
        //        }
        //    });
        //}


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
            udpSock.SendMessageAsync(message);
        }

        public void SendMovementMessage(LocationModel location) {
            float x = location.position.X;
            //float y = location.position.Y;
            float z = location.position.Z;

            //byte[] data = new byte[13];
            byte[] data = new byte[9];
            data[0] = 0x01;

            BitConverter.GetBytes(x).CopyTo(data, 1);
            //BitConverter.GetBytes(y).CopyTo(data, 5););
            BitConverter.GetBytes(z).CopyTo(data, 5);
            //BitConverter.GetBytes(z).CopyTo(data, 9);

            udpSock.SendBytesAsync(data);
        }

        public void Dispose()
        {
            StopSocket();
        }
    }
}
