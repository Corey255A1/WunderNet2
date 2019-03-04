using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using WunderNetLayer;
namespace WunderNet
{
    public delegate void WunderPacketClientReceivedCallback(ClientHandler client, WunderPacket packet);
    public class ClientHandler
    {
        public string ClientInfo
        {
            get { return _client.Client.RemoteEndPoint.ToString(); }
        }
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
        public async void Send(WunderPacket p)
        {
            await _processor.WriteData(p.GetBytes());
        }
        public void Disconnect()
        {
            _client.Close();
        }
        private void PacketReceived(WunderPacket wp)
        {
            WunderPacketReceived?.Invoke(this, wp);
        }
    }
}
