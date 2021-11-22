using System;
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

      private static void SendTCPDataToAllExcept(int _exceptClient, Packet _packet)
      {
         _packet.WriteLength();
         for(int i = 1; i <= Server.MaxPlayers; i++) {
            if(i != _exceptClient) {
               Server.clients[i].tcp.SendData(_packet);
            }
         }
      }

      private static void SendUDPDataToAll(Packet packet)
      {
         packet.WriteLength();
         for(int i = 1; i < Server.MaxPlayers; i++) {
            Server.clients[i].udp.SendData(packet);
         }
      }

      private static void SendUDPDataToAllExcept(int _exceptClient, Packet _packet)
      {
         _packet.WriteLength();
         for(int i = 1; i <= Server.MaxPlayers; i++) {
            if(i != _exceptClient) {
               Server.clients[i].udp.SendData(_packet);
            }
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

      public static void SpawnPlayer(int toClient, Player player)
      {
         using (Packet packet = new Packet((int)ServerPackets.spawnPlayer)) {
            packet.Write(player.id);
            packet.Write(player.username);
            packet.Write(player.position);
            packet.Write(player.rotation);

            SendTCPData(toClient, packet);
         }

      }

      public static void PlayerPosition(Player player)
      {
         using (Packet packet = new Packet((int)ServerPackets.playerPosition)) {
            packet.Write(player.id);
            packet.Write(player.position);

            SendUDPDataToAll(packet);
         }
      }

      public static void PlayerRotation(Player player)
      {
         using(Packet packet = new Packet((int)ServerPackets.playerRotation)) {
            packet.Write(player.id);
            packet.Write(player.rotation);

            SendUDPDataToAllExcept(player.id, packet);
         }
      }

      //public static void UDPTest(int toClient)
      //{
      //   using (Packet packet = new Packet((int)ServerPackets.udpTest)) {
      //      packet.Write("This is the UDP test.");

      //      SendUDPData(toClient, packet);
      //   }
      //}
   }
}
