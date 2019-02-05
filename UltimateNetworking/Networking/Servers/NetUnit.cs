using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltimateNetworking.Networking.Packets;
using UniversalLogger;

namespace UltimateNetworking.Networking.Servers
{
    public class NetUnit
    {
        private ULogger logger = new ULogger();
        private Socket mySocket;
        private ManualResetEvent allDone;
        private int bufferSize;

        public NetUnit(int bufferSize = 1024, int port = 11000)
        {
            try
            {
                allDone = new ManualResetEvent(false);
                this.bufferSize = bufferSize;

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

                mySocket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                mySocket.Bind(localEndPoint);
                mySocket.Listen(200);

                logger.Info("Server started!");

                new Thread(Listen).Start();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void Listen()
        {
            try
            {
                while (true)
                {
                    allDone.Reset();

                    logger.Info("Waiting for connection...");

                    mySocket.BeginAccept(new AsyncCallback(AcceptCallback), mySocket);// listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket ishikSocket = mySocket.EndAccept(ar);
            BasicPacket packet = new BasicPacket(ishikSocket, bufferSize);

            ishikSocket.BeginReceive(packet.Buffer, 0, bufferSize,
                0, new AsyncCallback(ReadCallback), packet);

        }

        private void ReadCallback(IAsyncResult ar)
        {
            BasicPacket packet = ar.AsyncState as BasicPacket;
            Socket ishikSocket = packet.WorkingSocket;

            int bytesRead = ishikSocket.EndReceive(ar);
            logger.Info("Bytes readed: " + bytesRead);
        }

        public void Send(Socket ishikSocket, byte[] data)
        {
            ishikSocket.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), ishikSocket);
        }

        public void Send(string ip, int port, byte[] data)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                
                Socket ishikScoket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                ishikScoket.Connect(remoteEP);

                Send(ishikScoket, data);
            }
            catch (Exception excception)
            {
                logger.Error(excception);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket ishikScoket = ar.AsyncState as Socket;

                int bytesSended = ishikScoket.EndSend(ar);
                logger.Info("Bytes sended: " + bytesSended);

                ishikScoket.Shutdown(SocketShutdown.Both);
                ishikScoket.Close();
            }
            catch (Exception excception)
            {
                logger.Error(excception);
            }
        }

    }
}
