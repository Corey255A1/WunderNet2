
/**
 * Corey Wunderlich - What is a good Server Design?
 * What makes a good server?
 * - Fast
 * - Low Overhead
 * - Multiple Connections
 * - Reliability
 * What happens when someone connects?
 * -
 * 
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using WunderNetLayer;
namespace WunderNet
{
    public delegate void NewConnectionEvent(ClientHandler ch);
    public class WunderServer
    {
        public event NewConnectionEvent NewConnection;
        TcpListener _tcpServer;
        bool _running = false;
        ConcurrentBag<ClientHandler> _clients = new ConcurrentBag<ClientHandler>();
        WunderLayer _decoder;
        private Dictionary<string, WunderPacketClientReceivedCallback> PacketCallbacks = new Dictionary<string, WunderPacketClientReceivedCallback>();
        public WunderServer(string xmlpath)
        {
            _tcpServer = new TcpListener(new IPEndPoint(IPAddress.Any, 1234));
            _decoder = new WunderLayer(xmlpath);
        }

        public async void AcceptConnections()
        {
            _running = true;
            _tcpServer.Start();
            while (_running)
            {
                var client = await _tcpServer.AcceptTcpClientAsync();
                Console.WriteLine("SERVER:"+client.Client.RemoteEndPoint.ToString());
                var ch = new ClientHandler(_decoder, client);
                ch.WunderPacketReceived += WunderPacketClientReceived;
                _clients.Add(ch);
                NewConnection?.Invoke(ch);
            }
            _tcpServer.Stop();
        }

        public void WunderPacketClientReceived(EndPoint id, WunderPacket packet)
        {
            if (packet != null && PacketCallbacks.ContainsKey(packet.Name))
            {
                PacketCallbacks[packet.Name]?.Invoke(id, packet);
            }
        }

        public void AddDataCallback(string packetname, WunderPacketClientReceivedCallback callback)
        {
            if (!PacketCallbacks.ContainsKey(packetname))
            {
                PacketCallbacks.Add(packetname, null);
            }
            PacketCallbacks[packetname] += callback;
        }


    }
}
