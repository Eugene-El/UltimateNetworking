using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UltimateNetworking.Networking.Packets
{
    public class BasicPacket
    {
        public BasicPacket(Socket socket, int bufferSize)
        {
            WorkingSocket = socket;
            Buffer = new byte[bufferSize];
            MemOry = new MemoryStream();
        }
        public BasicPacket() { }

        public Socket WorkingSocket { get; set; }
        public byte[] Buffer { get; set; }
        public MemoryStream MemOry { get; set; }
    }
}
