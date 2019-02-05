using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateNetworking.Networking.Packets;

namespace UltimateNetworking.Interfaces.Servers
{
    public interface IBasicServer
    {
        void Start(int bufferSize, int port);

        void ProcessRequest(BasicPacket basicPacket);

    }
}
