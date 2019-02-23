using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using WunderNetLayer;
namespace WunderNet
{
    public delegate void WunderPacketClientReceivedCallback(EndPoint id, WunderPacket packet);
    public class ClientHandler
    {
        TcpClient _client;
        StreamProcessor _processor;
        public WunderPacketClientReceivedCallback WunderPacketReceived;
        public ClientHandler(WunderLayer packetDecoder, TcpClient c)
        {
            _client = c;
            
            _processor = new StreamProcessor(packetDecoder, _client.Client.RemoteEndPoint, _client.GetStream(), 1024);
            _processor.PacketReceived += PacketReceived;
            _processor.BeginReadData();
        }
        public async void WriteData(WunderPacket p)
        {
            await _processor.WriteData(p.GetBytes());
        }
        public void PacketReceived(WunderPacket wp)
        {
            WunderPacketReceived?.Invoke(_client.Client.RemoteEndPoint, wp);
        }
    }
}
