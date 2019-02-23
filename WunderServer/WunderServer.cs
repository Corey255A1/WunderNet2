
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
        public WunderServer(string xmlpath)
        {
            _tcpServer = new TcpListener(new IPEndPoint(IPAddress.Any, 1234));

        }

        public async void AcceptConnections()
        {
            _running = true;
            _tcpServer.Start();
            while (_running)
            {
                var client = await _tcpServer.AcceptTcpClientAsync();
                Console.WriteLine("SERVER:"+client.Client.RemoteEndPoint.ToString());
                var ch = new ClientHandler(client);
                _clients.Add(ch);
                NewConnection?.Invoke(ch);
            }
            _tcpServer.Stop();
        }


    }
}
