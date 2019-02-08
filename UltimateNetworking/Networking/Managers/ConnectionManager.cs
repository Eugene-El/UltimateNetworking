using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniversalLogger;

namespace UltimateNetworking.Networking.Managers
{
    public class ConnectionManager
    {
        private List<Connection> connections = new List<Connection>();
        private ULogger logger = new ULogger();
        private TimeSpan frequency;
        private TimeSpan liveTime;

        /// <summary>
        /// Constructor, wich starts thread, that check sockets states
        /// </summary>
        /// <param name="frequency"> Standart - 30 sec </param>
        /// <param name="liveTime"> Standart - 30 min </param>
        public ConnectionManager(TimeSpan? frequency = null, TimeSpan? liveTime = null)
        {
            try
            {
                if (liveTime == null)
                    liveTime = TimeSpan.FromMinutes(10);
                this.liveTime = liveTime.Value;
                if (frequency == null)
                    frequency = TimeSpan.FromSeconds(30);
                this.frequency = frequency.Value;

                new Thread(CheckConnections).Start();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw exception;
            }
        }

        public Socket Connect(IPEndPoint endPoint)
        {
            try
            {
                lock (connections)
                {
                    var connection = connections.Find(c => c.EndPoint.Address.Equals(endPoint.Address));
                    if (connection == null)
                    {
                        Socket newSocket = new Socket(endPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        newSocket.Connect(endPoint);
                        connection = new Connection(newSocket, endPoint);
                        connections.Add(connection);
                    }
                    else
                    {
                        connection.UpdateLastUse();
                    }

                    return connection.Socket;
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw exception;
            }
        }

        public void UpdateLastUse(Socket socketToUpdate)
        {
            try
            {
                lock (connections)
                {
                    var conncetion = connections.Find(c => c.EndPoint.Equals(socketToUpdate.RemoteEndPoint));
                    if (conncetion != null)
                        conncetion.UpdateLastUse();
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw exception;
            }
        }

        private void CheckConnections()
        {
            try
            {
                do
                {
                    Thread.Sleep(frequency);
                    lock (connections)
                    {
                        for (int i = 0; i < connections.Count; i++)
                        {
                            if (connections[i].LastUseDateTime + liveTime < DateTime.Now)
                            {
                                connections[i].Socket.Disconnect(false);
                                connections[i].Socket.Dispose();
                                connections.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                } while (true);
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw exception;
            }
        }

        private class Connection
        {
            public Connection(Socket socket, IPEndPoint endPoint)
            {
                Socket = socket;
                EndPoint = endPoint;
                LastUseDateTime = DateTime.Now;
            }

            public Socket Socket { get; }
            public IPEndPoint EndPoint { get; }
            public DateTime LastUseDateTime { get; private set; }

            public void UpdateLastUse()
            {
                LastUseDateTime = DateTime.Now;
            }
        }
    }
}
