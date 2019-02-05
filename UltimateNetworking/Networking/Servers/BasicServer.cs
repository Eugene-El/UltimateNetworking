using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltimateNetworking.Interfaces.Servers;
using UltimateNetworking.Networking.Packets;
using UniversalLogger;

namespace UltimateNetworking.Networking.Servers
{
    public class BasicServer : IBasicServer
    {
        private bool _serverIsRunning = false;

        private int _bufferSize;
        private ULogger _logger;
        private ManualResetEvent _allDone;

        public void Start(int bufferSize, int port = 11000)
        {
            try
            {
                _logger = new ULogger();
                _bufferSize = bufferSize;
                _allDone = new ManualResetEvent(false);
            
                
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
                
                Socket listener = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(200);

                _serverIsRunning = true;
                _logger.Info("Server started!");

                while (true)
                {
                    _allDone.Reset();

                    _logger.Info("Waiting for connection...");

                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    _allDone.WaitOne();
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                throw exception;
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _allDone.Set();

            Socket listener = ar.AsyncState as Socket;
            Socket handler = listener.EndAccept(ar);

            BasicPacket basicPacket = new BasicPacket()
            {
                WorkingSocket = handler
            };

            handler.BeginReceive(basicPacket.Buffer, 0, _bufferSize, 0, new AsyncCallback(ReadCallback), basicPacket);

        }

        private void ReadCallback(IAsyncResult ar)
        {
            BasicPacket basicPacket = ar.AsyncState as BasicPacket;
            Socket handler = basicPacket.WorkingSocket;

            int bytesRead = handler.EndReceive(ar);
        }

        public void ProcessRequest(BasicPacket basicPacket)
        {
            throw new NotImplementedException();
        }

    }
}
