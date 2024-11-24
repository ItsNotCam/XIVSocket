using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using XIVSocket.Lib.Network.Socket;

namespace XIVSocket.Lib.Network
{
    public class TCPServer : IDisposable
    {
        public Int32 port { get; }
        public bool isRunning { get; private set; }

        private TcpListener server;
        private TcpClient client;
        private NetworkStream ns;
        private CancellationTokenSource cancellationToken;

        public TCPServer(Int32 port)
        {
            XIVSocketPlugin.PluginLogger.Info("Starting TCP Server ... on port " + port);
            this.port = port;
            server = new TcpListener(IPAddress.Any, port);
            cancellationToken = new CancellationTokenSource();
            startServer();
        }

        private async void startServer()
        {
            // continuously try to start the server
            while (!this.isRunning)
            {
                try {
                    server.Start();
                    XIVSocketPlugin.PluginLogger.Info($"Server started on port {port}");
                    this.isRunning = true;
                } catch (Exception e) {
                    XIVSocketPlugin.PluginLogger.Error($"Could not start server on port {port} - {e.Message}\nTrying again in 5 seconds ... ");
                    await Task.Delay(5000);
                    this.isRunning = false;
                }
            }

            try {
                handleConnections();
            } catch (Exception e) {
                XIVSocketPlugin.PluginLogger.Error($"Error handling connections - {e.Message}");
            }
        }

        private async void handleConnections()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                client = await server.AcceptTcpClientAsync();

                NetworkStream stream = client.GetStream(); //networkstream is used to send/receive messages
                while (client.Connected)  //while the client is connected, we look for incoming messages
                {
                    byte[] msg = new byte[1024];  //the messages arrive as byte array
                    try {
                        stream.Read(msg, 0, msg.Length); //the same networkstream reads the message sent by the client

                        // find the ez flag
                        if (msg[0] != 0x1D) {
                            XIVSocketPlugin.PluginLogger.Error("Invalid message received");
                            continue;
                        }

                        EzDeserializedPacket packet = EzSerDe.Deserialize(msg);
                        string message = Encoding.UTF8.GetString(packet.payload);
                        uint id = packet.id;
                        uint flag = packet.flag;

                        //stream.Dispose();
                        //stream = client.GetStream();
                        //stream.Write(msg, 0, msg.Length);


                        //Plugin.PluginLogger.Info($"Received message of length {length} with id {id} -> {message}");

                    } catch (Exception e) {
                        XIVSocketPlugin.PluginLogger.Error($"Error reading message - {e.Message}");
                        break;
                    }
                }

                stream.Dispose();
                client.Close();
            }
        }


        public void sendMessage(byte[] message) {
            if(ns == null) {
                XIVSocketPlugin.PluginLogger.Error("Network stream is null");
            } else if (!ns.CanWrite) {
                XIVSocketPlugin.PluginLogger.Error("Network stream cannot write");
            } else {
                try { 
                    ns.WriteAsync(message, 0, message.Length); 
                } catch (Exception e) {
                    XIVSocketPlugin.PluginLogger.Error($"Error sending message - {e.Message}");
                }
            }
        }

        public void Dispose()
        {
            try {
                if(cancellationToken != null) {
                    cancellationToken.Cancel();
                }

                if (server != null) {
                    server.Stop();
                    server.Dispose();
                }

                if (client != null)  {
                    client.Close();
                }
            } catch (Exception e) {
                XIVSocketPlugin.PluginLogger.Error($"Error disposing server - {e.Message}");
            }
        }

    }
}
