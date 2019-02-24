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
        private NetworkStream _stream;
        private WunderLayer _decoder;
        private EndPoint _endpointID;
        private int BUFFERSIZE;
        private byte[] _buffer;
        private int _dataoffset;
        public event WunderPacketReceivedCallback PacketReceived;        

        public StreamProcessor(WunderLayer decoder, EndPoint endpoint, NetworkStream stream, int buffersize)
        {
            BUFFERSIZE = buffersize;
            _endpointID = endpoint;
            _stream = stream;
            _decoder = decoder;
            _buffer = new byte[BUFFERSIZE];
        }
        public async void BeginReadData()
        {
            try
            {
                int bytesread = await _stream.ReadAsync(_buffer, 0, BUFFERSIZE);
                while (bytesread > 0)
                {
                    int offset = 0;
                    do
                    {
                        var packet = _decoder.GetFromBytes(_buffer, ref offset);
                        if (packet != null)
                        {
                            PacketReceived?.Invoke(packet);                            
                        }
                        else if(offset < bytesread)
                        {
                            _dataoffset = bytesread - offset;
                            Array.Copy(_buffer, offset, _buffer, 0, _dataoffset);
                            break;
                        }
                    } while (offset < bytesread);
                    bytesread = await _stream.ReadAsync(_buffer, _dataoffset, BUFFERSIZE-_dataoffset);
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
