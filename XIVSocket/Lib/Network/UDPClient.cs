using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XIVSocket.App.Network;

public class UDPClient : IDisposable
{
    private int incomingPort;// Port to listen for incoming messages
    private string incomingHost;// Port to listen for incoming messages
    private string outgoingHost;  // Destination IP
    private int outgoingPort;  // Destination port

    private Task recvTask;
    private CancellationTokenSource cnclTokenSrc;

    public UDPClient(int outgoingPort)
    {
        this.outgoingPort = outgoingPort;
        cnclTokenSrc = new CancellationTokenSource();
    }
    public void SendMessageAsync(string message, bool isEcho = false)
    {
        var port = isEcho ? incomingPort : outgoingPort;
        using (var udpClient = new UdpClient(outgoingPort))
        {
            var data = Encoding.UTF8.GetBytes(message);
            var _ = udpClient.SendAsync(data, data.Length, outgoingHost, port);
        }
    }

    public void SendBytesAsync(byte[] data)
    {
        using (var udpClient = new UdpClient(outgoingPort)) {
            udpClient.SendAsync(data, data.Length, outgoingHost, outgoingPort);
        }
    }

    public async void Dispose()
    {
        XIVSocketPlugin.PluginLogger.Debug("Socket closing ... ");
        try
        {
            cnclTokenSrc.Cancel();
            await recvTask;
            XIVSocketPlugin.PluginLogger.Debug("Socket closed");
        }
        catch (Exception ex)
        {
            XIVSocketPlugin.PluginLogger.Error("Socket failed to close: " + ex.Message);
        }
        finally
        {
            cnclTokenSrc.Dispose();
        }
    }

    private async Task ReceiveMessagesAsync(CancellationToken token)
    {
        using (var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, incomingPort)))
        {
            XIVSocketPlugin.PluginLogger.Debug($"Socket open at: {IPAddress.Any.ToString()}:{incomingPort} and waiting for messages...");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var receivedResult = await udpClient.ReceiveAsync(cnclTokenSrc.Token);
                    var receivedMessage = Encoding.UTF8.GetString(receivedResult.Buffer);

                    var friendlyMessage = receivedMessage.Replace(" - respond", "");
                    XIVSocketPlugin.PluginLogger.Debug($"Recv from [{receivedResult.RemoteEndPoint}]: {friendlyMessage}");
                    XIVSocketPlugin.ChatGui.Print($"[XIVSocket] {friendlyMessage}");


                    if (receivedMessage.Contains("respond"))
                    {
                        SendMessageAsync(receivedMessage);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                XIVSocketPlugin.PluginLogger.Debug("Socket closed gracefully.");
            }
            catch (Exception ex) when (token.IsCancellationRequested)
            {
                XIVSocketPlugin.PluginLogger.Debug($"Operation canceled: {ex.Message}");
            }
            catch (Exception ex)
            {
                XIVSocketPlugin.PluginLogger.Error($"Unexpected error: {ex.Message}");
            }
        }
    }

}
