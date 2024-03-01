using System.Net;
using System.Net.Sockets;

namespace XDChatServer.Services;

public class ServerConfigurationServiceDefault : IServerConfigurationService
{
    public IPEndPoint ConfigureEndPoint()
    {
        IPAddress ip = Dns
            .GetHostEntry(Dns.GetHostName())
            .AddressList
            .First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        int port = 1500;
        return new IPEndPoint(ip, port);
    }
}