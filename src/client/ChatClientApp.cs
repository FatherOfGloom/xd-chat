using System.Net;
using System.Net.Sockets;
using System.Text;
using XDChatLib;

namespace XDChatClient;

public class ChatClientApp
{
    private readonly TcpClient _tcpClient = new();
    private readonly IPEndPoint _serverEndPoint;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private PacketReader _packetReader = null!;
    private StringBuilder _chatHistory = new();

    public ChatClientApp()
    {
        _serverEndPoint = GetLocalEndPoint();
    }

    public void Run()
    {
        Console.WriteLine("Client started.");
        var username = ReadUserInput("Please enter your username.",
            "Invalid username. It cannot be empty. Try again.");
        var cancellationToken = _cancellationTokenSource.Token;

        try
        {
            ConnectToChat(username);
            ReadPackets(cancellationToken);
            string input;
            do
            {
                input = ReadUserInput();
                SendMessage(input);
            } while (input != ":q");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _cancellationTokenSource.Cancel();
            _tcpClient.Close();
        }
    }

    private void ConnectToChat(string username)
    {
        if (!_tcpClient.Connected)
        {
            _tcpClient.Connect(_serverEndPoint);
            _packetReader = new PacketReader(_tcpClient.GetStream());
            var connectPacket = new PacketBuilder();
            connectPacket.WriteMessage(username);
            _tcpClient.Client.Send(connectPacket.PacketBytes);
        }
    }

    private void ReadPackets(CancellationToken token)
    {
        Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                var message = _packetReader.ReadMessage();
                Console.WriteLine(message);
            }
        });
    }

    private void SendMessage(string message)
    {
        var packet = new PacketBuilder();
        packet.WriteMessage(message);
        _tcpClient.Client.Send(packet.PacketBytes);
    }

    private IPEndPoint GetLocalEndPoint()
        => new IPEndPoint(Dns
            .GetHostEntry(Dns.GetHostName())
            .AddressList
            .First(ip => ip.AddressFamily == AddressFamily.InterNetwork), 1500);

    private string ReadUserInput(string? message = null, string? errorMessage = null)
    {
        string? userInput;
        while (true)
        {
            if (!string.IsNullOrEmpty(message)) Console.WriteLine(message);
            userInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(userInput))
            {
                break;
            }
            if (!string.IsNullOrEmpty(errorMessage)) Console.WriteLine(errorMessage);
        }
        return userInput;
    }
}