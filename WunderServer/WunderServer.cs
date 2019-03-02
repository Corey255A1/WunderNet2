
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
    public class WunderTCPServer
    {
        public event NewConnectionEvent NewConnection;


        private TcpListener _tcpServer;
        private bool _running = false;
        private ConcurrentBag<ClientHandler> _clients = new ConcurrentBag<ClientHandler>();
        private WunderLayer _decoder;
        private Dictionary<string, WunderPacketClientReceivedCallback> PacketCallbacks = new Dictionary<string, WunderPacketClientReceivedCallback>();
        public WunderTCPServer(string xmlpath, IPAddress iPAddress, int port)
        {
            _tcpServer = new TcpListener(new IPEndPoint(iPAddress, port));
            _decoder = new WunderLayer(xmlpath);
            Console.WriteLine(_decoder.ToString());
        }

        public async void AcceptConnections()
        {
            try
            {
                _running = true;
                _tcpServer.Start();
                while (_running)
                {
                    var client = await _tcpServer.AcceptTcpClientAsync();
                    //Console.WriteLine("SERVER:" + client.Client.RemoteEndPoint.ToString());
                    var ch = new ClientHandler(_decoder, client);
                    ch.WunderPacketReceived += WunderPacketClientReceived;
                    _clients.Add(ch);
                    NewConnection?.Invoke(ch);
                }
                _tcpServer.Stop();
            }
            catch
            {
                Console.WriteLine("Connections Closed");
            }
        }

        public void Disconnect()
        {
            _running = false;
            _tcpServer.Stop();
            foreach(var client in _clients)
            {
                client.Disconnect();
            }
        }

        public WunderPacket GetNewPacket(string packetname)
        {
            return _decoder.GetNewPacket(packetname);
        }

        public void WunderPacketClientReceived(ClientHandler client, WunderPacket packet)
        {
            if (packet != null && PacketCallbacks.ContainsKey(packet.Name))
            {
                PacketCallbacks[packet.Name]?.Invoke(client, packet);
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
