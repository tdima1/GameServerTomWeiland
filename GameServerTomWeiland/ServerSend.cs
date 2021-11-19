﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameServerTomWeiland
{
   class ServerSend
   {
      private static void SendTCPData(int toClient, Packet packet)
      {
         packet.WriteLength();
         Server.clients[toClient].tcp.SendData(packet);
      }

      private static void SendTCPDataToAll(Packet packet)
      {
         packet.WriteLength();
         for(int i = 1; i < Server.MaxPlayers; i++) {
            Server.clients[i].tcp.SendData(packet);
         }
      }

      public static void Welcome(int toClient, string message)
      {
         using (Packet packet = new Packet((int)ServerPackets.welcome)) {
            packet.Write(message);
            packet.Write(toClient);

            SendTCPData(toClient, packet);
         }
      }
   }
}
