using System;
using System.Text;
using System.Net.Sockets;
namespace WunderNet
{
    public class ClientHandler
    {
        TcpClient _client;
        NetworkStream _stream;
        const int BUFFERSIZE = 1024;
        byte[] buffer = new byte[BUFFERSIZE];
        public ClientHandler(TcpClient c)
        {
            _client = c;
            _stream = _client.GetStream();
            ReadData();
        }
        private async void ReadData()
        {
            try
            {
                int bytesread = await _stream.ReadAsync(buffer, 0, BUFFERSIZE);
                while (bytesread > 0)
                {
                    Console.WriteLine("SERVER:"+Encoding.ASCII.GetString(buffer, 0, bytesread));
                    if (!_client.Connected) break;
                    await WriteData("WOOP");

                    bytesread = await _stream.ReadAsync(buffer, 0, BUFFERSIZE);
                }
            }
            catch
            {
                Console.WriteLine("Client Read Aborted");
            }
            Console.WriteLine("DONE");
        }
        public async System.Threading.Tasks.Task<bool> WriteData(string data)
        {
            if (_client.Connected)
            {
                await _stream.WriteAsync(Encoding.ASCII.GetBytes(data), 0, data.Length);
                return true;
            }
            return false;
        }
    }
}
