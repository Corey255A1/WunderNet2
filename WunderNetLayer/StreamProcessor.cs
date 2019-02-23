using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

namespace WunderNetLayer
{
    public delegate void WunderPacketReceivedCallback(WunderPacket packet);
    public class StreamProcessor
    {
        NetworkStream _stream;
        WunderLayer _decoder;
        EndPoint _endpointID;
        private int BUFFERSIZE;
        private byte[] buffer;

        public event WunderPacketReceivedCallback PacketReceived;

        

        public StreamProcessor(WunderLayer decoder, EndPoint endpoint, NetworkStream stream, int buffersize)
        {
            BUFFERSIZE = buffersize;
            _endpointID = endpoint;
            _stream = stream;
            _decoder = decoder;
            buffer = new byte[BUFFERSIZE];
        }
        public async void BeginReadData()
        {
            try
            {
                int bytesread = await _stream.ReadAsync(buffer, 0, BUFFERSIZE);
                while (bytesread > 0)
                {
                    int offset = 0; //Not Using this currently
                    var packet = _decoder.GetFromBytes(buffer, ref offset);
                    if (packet != null) PacketReceived?.Invoke(packet);
                    bytesread = await _stream.ReadAsync(buffer, 0, BUFFERSIZE);
                }
            }
            catch
            {
                Console.WriteLine("Client Read Aborted");
            }
            Console.WriteLine("DONE");
        }

        public async System.Threading.Tasks.Task<bool> WriteData(byte[] data)
        {
            try
            { 
                await _stream.WriteAsync(data);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
