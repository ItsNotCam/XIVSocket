using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using XIVSocket.App.Network;
using XIVSocket.Lib.Network.Codec;

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
            try {
                server.Start();
            } catch(HttpListenerException e) {
                XIVSocketPlugin.PluginLogger.Error($"Error starting server - {e.Message}");
                return;
            } catch(ObjectDisposedException e) {
                XIVSocketPlugin.PluginLogger.Error($"Error starting server - {e.Message}");
                return;
            } catch(Exception e) {
                XIVSocketPlugin.PluginLogger.Error($"Unknown error when starting server - {e.Message}");
                return;
            }

            XIVSocketPlugin.PluginLogger.Info($"Server started on port {port}");
            Listen();
        }

        private async void Listen()
        {
            while (!cancellationToken.Token.IsCancellationRequested)
            {
                HttpListenerContext context;
                try {
                    context = await server.GetContextAsync();
                } catch(Exception e) {
                    XIVSocketPlugin.PluginLogger.Error($"Error getting context - {e.Message}");
                    continue;
                }

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    XIVSocketPlugin.PluginLogger.Info("WebSocket connection established");
                    HandleConnectionLifecycle(wsContext.WebSocket);
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
            while (socket.State == WebSocketState.Open && !cancellationToken.Token.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                try {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken.Token);
                } catch(Exception e) {
                    if(e is WebSocketException WebSocketException) {
                        if(WebSocketException.ErrorCode == 10054) {
                            XIVSocketPlugin.PluginLogger.Info("Client disconnected");
                        } else {
                            XIVSocketPlugin.PluginLogger.Error($"Error receiving message - {WebSocketException.InnerException!.Message}");
                        }
                    } else {
                        XIVSocketPlugin.PluginLogger.Error($"Error receiving message - {e.Message}");
                    }
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Close) {
                    try {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        XIVSocketPlugin.PluginLogger.Info("WebSocket connection closed");
                    } catch(Exception e) {
                        XIVSocketPlugin.PluginLogger.Error($"Error closing socket - {e.Message}");
                    }
                } else {
                    var packet = EzSerDe.Deserialize(buffer);
                    var payload = EzCodec.Decode(packet.payload);
                    XIVSocketPlugin.PluginLogger.Info($"Received: {packet.id} {packet.flag} {payload}");

                    EzFlag flag = (EzFlag)packet.flag;
                    string? response = manager.HandleRequest(flag, payload);

                    if(response == null) {
                        flag = EzFlag.INVALID_ROUTE;
                        response = "Invalid Response";
                    }

                    byte[] message = EzSerDe.Serialize(
                        (int)flag,
                        EzCodec.Encode(response),
                        (int)packet.id
                    );
                }
            }

            if (socket != null && socket.State == WebSocketState.Open) { 
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                socket.Dispose();
                clients.Remove(socket);
            }
        }

        public async void Dispose()
        {
            try {
                cancellationToken.Cancel();
            } catch(Exception e) {
                XIVSocketPlugin.PluginLogger.Error($"Error cancelling token - {e.Message}");
            }

            //foreach (var client in clients) { 
            //    try {
            //        await client.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "", CancellationToken.None);
            //    } catch (Exception e) {
            //        XIVSocketPlugin.PluginLogger.Error($"Error closing client - {e.Message}");
            //    } finally {
            //        client.Dispose();
            //    }
            //}

            clients.Clear();

            try {
                server.Stop();
            } catch(ObjectDisposedException e) {
                XIVSocketPlugin.PluginLogger.Debug($"Server has been disposed");
            } finally {
                server.Close();
                XIVSocketPlugin.PluginLogger.Info("Server stopped");
            }
        }

        public bool IsRunning() => server.IsListening;
    }
}
