namespace XDChatLib;

public class PacketBuilder
{
    private readonly MemoryStream _memoryStream = new();

    public void WriteMessage(string message)
    {
        _memoryStream.Write(BitConverter.GetBytes(message.Length));
        _memoryStream.Write(System.Text.Encoding.ASCII.GetBytes(message));
    }

    public byte[] PacketBytes => _memoryStream.ToArray();
}