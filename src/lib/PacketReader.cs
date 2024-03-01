using System.Text;

namespace XDChatLib;

public class PacketReader : BinaryReader
{
    private readonly NetworkStream _networkStream;
    
    public PacketReader(NetworkStream networkStream) : base(networkStream)
    {
        _networkStream = networkStream;
    }
    
    public string ReadMessage()
    {
        var buffer = new byte[ReadInt32()];
        _ = _networkStream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer);
    }
}