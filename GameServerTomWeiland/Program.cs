using System;
using System.Net;
using System.Net.Sockets;

namespace GameServerTomWeiland
{
   class Program
   {
      static void Main(string[] args)
      {
         Server.Start(20, 26950);
         Console.ReadKey();
      }
   }
}
