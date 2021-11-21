using System;
using System.Collections.Generic;
using System.Text;

namespace GameServerTomWeiland
{
   // packet reader service?
   class ServerHandle
   {
      public static void WelcomeReceived(int fromClient, Packet packet)
      {
         int clientId = packet.ReadInt();
         string username = packet.ReadString();

         Console.WriteLine($"{Server.clients[fromClient].tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient} with username {username}.");

         if (fromClient != clientId) {
            Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientId})");
         }

         //TODO: send player into game

      }
   }
}
