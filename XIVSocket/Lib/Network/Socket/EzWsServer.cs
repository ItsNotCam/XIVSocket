using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using XIVSocket.App.Network;
using XIVSocket.Lib.Network.Codec;
using XIVSocket.Lib.Network.Socket;

namespace XIVSocket.Lib.Network.Socket
{
    internal class EzWsServer : IDisposable
    {
        private readonly int port;
        private HttpListener server;
        private CancellationTokenSource cancellationToken;
        private List<WebSocket> clients;
        private readonly NetworkManager manager;

        public EzWsServer(int port, NetworkManager manager)
        {
            this.port = port;
            this.manager = manager; 

            clients = new List<WebSocket>();
            cancellationToken = new CancellationTokenSource();

            server = new HttpListener();
            server.Prefixes.Add($"http://localhost:{port}/");
            Start();
        }

        private void Start()
        {
            server.Start();
            Plugin.PluginLogger.Info($"Server started on port {port}");
            Listen();
        }

        private async void Listen()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await server.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    Plugin.PluginLogger.Info("WebSocket connection established");

                    var socket = wsContext.WebSocket;
                    HandleConnectionLifecycle(socket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async void HandleConnectionLifecycle(WebSocket socket)
        {
            clients.Add(socket);

            var buffer = new byte[1024];
            while (socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken.Token);
                    Plugin.PluginLogger.Info("WebSocket connection closed");
                }
                else
                {
                    var packet = EzSerDe.Deserialize(buffer);
                    var payload = EzCodec.Decode(packet.payload);
                    Plugin.PluginLogger.Info($"Received: {packet.id} {packet.flag} {payload}");
                    var response = this.manager.HandleRequest(packet.id, packet.flag, payload);
                }
            }

            if (socket.State == WebSocketState.Open)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken.Token);
                clients.Remove(socket);
            }
        }

        public async void Dispose()
        {
            cancellationToken.Cancel();
            foreach (var client in clients)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }

            clients.Clear();
            server.Stop();
        }

        public bool IsRunning()
        {
            return true;
        }
    }
}
