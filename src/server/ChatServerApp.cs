using XDChatServer.Services;
using Microsoft.Extensions.Logging;
using XDChatLib;
using System.Net.Sockets;
using System.Net;

namespace XDChatServer;

public class ChatServerApp : IChatServerApp
{
    private readonly ILogger<ChatServerApp> _logger;
    private readonly IServerConfigurationService _configurationService;
    private readonly TcpListener _listener;
    private readonly IPEndPoint _iPEndPoint;
    private List<ChatClient> _clients = new();

    public ChatServerApp(IServerConfigurationService configurationService, ILogger<ChatServerApp> logger)
    {
        _logger = logger;
        _configurationService = configurationService;
        _iPEndPoint = _configurationService.ConfigureEndPoint();
        _listener = new TcpListener(_iPEndPoint);
    }

    public void Run()
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} >> Server listening on {_iPEndPoint.Address}::{_iPEndPoint.Port}.");
            _listener.Start();
            Console.WriteLine(">> Waiting for a connection... ");
            while (true)
            {
                var tcpClient = _listener.AcceptTcpClient();
                var client = new ChatClient(tcpClient);
                _clients.Add(client);
                var usernames = string.Join(", ", _clients.Select(client => client.Username).ToArray());
                BroadcastMessage($"XDChat: user '{client.Username}' connected at {DateTime.Now}.\nCurrent chat users are: {usernames}");
                Task.Run(() => ProcessClient(client));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(0, e, "Error.");
            _listener.Stop();
        }
    }

    private void ProcessClient(ChatClient client)
    {
        while (true)
        {
            try
            {
                var message = client.PacketReader.ReadMessage();
                BroadcastMessage(client.FormatMessage(message));
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                BroadcastMessage($"XDChat: user '{client.Username}' disconnected at {DateTime.Now}.");
                
                client.TcpClient.Close();
                _clients.Remove(client);
                break;
            }
        }
    }

    private void BroadcastMessage(string message)
    {
        foreach (var client in _clients)
        {
            try
            {
                var builder = new PacketBuilder();
                builder.WriteMessage(message);
                client.TcpClient.Client.Send(builder.PacketBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        Console.WriteLine($">> {message} $");
    }
}

