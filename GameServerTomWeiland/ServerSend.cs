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

      private static void SendUDPData(int toClient, Packet packet)
      {
         packet.WriteLength();
         Server.clients[toClient].udp.SendData(packet);
      }

      private static void SendTCPDataToAll(Packet packet)
      {
         packet.WriteLength();
         for(int i = 1; i < Server.MaxPlayers; i++) {
            Server.clients[i].tcp.SendData(packet);
         }
      }

      private static void SendUDPDataToAll(Packet packet)
      {
         packet.WriteLength();
         for(int i = 1; i < Server.MaxPlayers; i++) {
            Server.clients[i].udp.SendData(packet);
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

      public static void UDPTest(int toClient)
      {
         using (Packet packet = new Packet((int)ServerPackets.udpTest)) {
            packet.Write("This is the UDP test.");

            SendUDPData(toClient, packet);
         }
      }
   }
}
