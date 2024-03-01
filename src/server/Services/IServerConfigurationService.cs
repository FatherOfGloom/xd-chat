using System.Net;

namespace XDChatServer.Services;

public interface IServerConfigurationService
{
    IPEndPoint ConfigureEndPoint();
}