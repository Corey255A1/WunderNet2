using System.Collections.Generic;
using System.Net.Sockets;
using WunderNetLayer;
namespace WunderClient
{
    public class WunderTCPClient
    {
        TcpClient _client;
        StreamProcessor _processor;
        WunderLayer _decoder;
        string _ipAddress;
  
        int _port;
        private Dictionary<string, WunderPacketReceivedCallback> PacketCallbacks = new Dictionary<string, WunderPacketReceivedCallback>();
        public WunderTCPClient(string xmlPath, string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _decoder = new WunderLayer(xmlPath);
        }
        private void PacketReceived(WunderPacket packet)
        {
            if (packet != null && PacketCallbacks.ContainsKey(packet.Name))
            {
                PacketCallbacks[packet.Name]?.Invoke(packet);
            }
        }

        public void Connect()
        {
            _client = new TcpClient(_ipAddress, _port);
            
            _processor = new StreamProcessor(_decoder, _client.Client.RemoteEndPoint, _client.GetStream(), 1024);
            _processor.PacketReceived += PacketReceived;
            _processor.BeginReadData();
        }

        public void Disconnect()
        {
            _client.Close();
        }

        public void AddDataCallback(string packetname, WunderPacketReceivedCallback callback)
        {
            if (!PacketCallbacks.ContainsKey(packetname))
            {
                PacketCallbacks.Add(packetname, null);
            }
            PacketCallbacks[packetname] += callback;
        }

        public WunderPacket GetNewPacket(string packetname)
        {
            return _decoder.GetNewPacket(packetname);
        }

        public async void Send(WunderPacket p)
        {
            await _processor.WriteData(p.GetBytes());
        }
    }
}
