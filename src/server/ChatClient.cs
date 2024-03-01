using System.Net.Sockets;
using XDChatLib;

namespace XDChatServer;

public class ChatClient
{
    private readonly TcpClient _tcpClient;
    private readonly PacketReader _packetReader;

    public ChatClient(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        _packetReader = new PacketReader(tcpClient.GetStream());
        Username = _packetReader.ReadMessage();
    }   

    public string Username { get; set; }
    public PacketReader PacketReader => _packetReader;
    public TcpClient TcpClient => _tcpClient;
    
    public string FormatMessage(string message) => $"[{DateTime.Now}] {Username}: {message}";
}