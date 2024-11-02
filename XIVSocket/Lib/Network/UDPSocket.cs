using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XIVSocket.App.Network;

public class UDPSocket : IDisposable
{
    private int incomingPort;// Port to listen for incoming messages
    private string incomingHost;// Port to listen for incoming messages
    private string outgoingHost;  // Destination IP
    private int outgoingPort;  // Destination port

    private Task recvTask;
    private CancellationTokenSource cnclTokenSrc;

    public UDPSocket(int incomingPort, string incomingHost, int outgoingPort, string outgoingHost)
    {
        this.incomingPort = incomingPort;
        this.incomingHost = incomingHost;
        this.outgoingHost = outgoingHost;
        this.outgoingPort = outgoingPort;

        cnclTokenSrc = new CancellationTokenSource();
    }

    public async void Start()
    {
        try
        {
            recvTask = ReceiveMessagesAsync(cnclTokenSrc.Token);
            await recvTask;
        }
        catch (Exception ex)
        {
            Plugin.PluginLogger.Error("Error opening socket " + ex.Message);
        }
    }

    public bool isRunning()
    {
        return recvTask != null && !recvTask.IsCompleted && !recvTask.IsCanceled && !recvTask.IsFaulted;
    }

    private async Task ReceiveMessagesAsync(CancellationToken token)
    {
        using (var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, incomingPort)))
        {
            Plugin.PluginLogger.Debug($"Socket open at: {IPAddress.Any.ToString()}:{incomingPort} and waiting for messages...");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var receivedResult = await udpClient.ReceiveAsync(cnclTokenSrc.Token);
                    var receivedMessage = Encoding.UTF8.GetString(receivedResult.Buffer);

                    var friendlyMessage = receivedMessage.Replace(" - respond", "");
                    Plugin.PluginLogger.Debug($"Recv from [{receivedResult.RemoteEndPoint}]: {friendlyMessage}");
                    Plugin.ChatGui.Print($"[XIVSocket] {friendlyMessage}");


                    if (receivedMessage.Contains("respond"))
                    {
                        await SendMessageAsync(receivedMessage);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Plugin.PluginLogger.Debug("Socket closed gracefully.");
            }
            catch (Exception ex) when (token.IsCancellationRequested)
            {
                Plugin.PluginLogger.Debug($"Operation canceled: {ex.Message}");
            }
            catch (Exception ex)
            {
                Plugin.PluginLogger.Error($"Unexpected error: {ex.Message}");
            }
        }
    }


    public async Task SendMessageAsync(string message, bool isEcho = false)
    {
        var port = isEcho ? incomingPort : outgoingPort;
        using (var udpClient = new UdpClient(outgoingPort))
        {
            var data = Encoding.UTF8.GetBytes(message);
            await udpClient.SendAsync(data, data.Length, outgoingHost, port);
            Console.WriteLine($"Send: {message}");
        }
    }

    public async void Dispose()
    {
        Plugin.PluginLogger.Debug("Socket closing ... ");
        try
        {
            cnclTokenSrc.Cancel();
            await recvTask;
            Plugin.PluginLogger.Debug("Socket closed");
        }
        catch (Exception ex)
        {
            Plugin.PluginLogger.Error("Socket failed to close: " + ex.Message);
        }
        finally
        {
            cnclTokenSrc.Dispose();
        }
    }
}
