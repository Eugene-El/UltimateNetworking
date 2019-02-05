using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateNetworking.Networking.Servers;

namespace TestUserA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("----------------- TEST USER A -----------------");

            Console.WriteLine("Enter to start...");
            Console.ReadLine();

            NetUnit netUnit = new NetUnit(port: 11022);
            Console.WriteLine("Started!");

            Console.ReadLine();
        }
    }
}
