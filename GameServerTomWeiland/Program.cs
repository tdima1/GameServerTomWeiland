using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameServerTomWeiland
{
   class Program
   {
      private static bool isRunning = false;

      static void Main(string[] args)
      {
         Console.Title = "Game Server";

         isRunning = true;

         Thread mainThread = new Thread(new ThreadStart(MainThread));
         mainThread.Start();

         Server.Start(20, 26950);


      }

      private static void MainThread()
      {
         Console.WriteLine($"Main thread started. Running at {Constants.TicksPerSecond} ticks per second");

         DateTime nextLoop = DateTime.Now;

         while(isRunning) {

            while (nextLoop < DateTime.Now) {
               GameLogic.Update();

               nextLoop = nextLoop.AddMilliseconds(Constants.MillisecondsPerTick);

               if (nextLoop > DateTime.Now) {
                  Thread.Sleep(nextLoop - DateTime.Now);
               }
            }

         }

      }
   }
}
