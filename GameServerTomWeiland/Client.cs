using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServerTomWeiland
{
   class Client
   {
      public static int dataBufferSize = 4096;

      public int id;
      public TCP tcp;
      public UDP udp;

      public Player player;

      public Client(int clientId)
      {
         id = clientId;
         tcp = new TCP(id);
         udp = new UDP(id);
      }

      public void SendIntoGame(string playerName)
      {
         player = new Player(id, playerName, Vector3.Zero);

         // sends ALL THE OTHER already existing players to this player.
         foreach(Client client in Server.clients.Values) {
            if (client.player != null) {
               if (client.id != id) {
                  ServerSend.SpawnPlayer(id, client.player);
               }
            }
         }

         //sends THIS PLAYER'S info to all other players, and himself.
         foreach (Client client in Server.clients.Values) {
            if (client.player != null) {
               ServerSend.SpawnPlayer(client.id, player);
            }
         }
      }

      public void Disconnect()
      {
         Console.WriteLine($"{tcp.Socket.Client.RemoteEndPoint} has disconnected.");

         player = null;

         tcp.Disconnect();
         udp.Disconnect();
      }

      public class TCP
      {
         public TcpClient Socket;

         private readonly int id;
         private NetworkStream stream;
         private byte[] receiveBuffer;

         private Packet receivedData;

         public TCP(int id)
         {
            this.id = id;
         }

         public void Connect(TcpClient socket)
         {
            Socket = socket;
            Socket.ReceiveBufferSize = dataBufferSize;
            Socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();
            receiveBuffer = new byte[dataBufferSize];

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, "This is a message from the server.");
         }

         private void ReceiveCallback(IAsyncResult result)
         {
            try {

               int byteLength = stream.EndRead(result);

               if(byteLength <= 0) {
                  Server.clients[id].Disconnect();
                  return;
               }

               byte[] data = new byte[byteLength];
               Array.Copy(receiveBuffer, data, byteLength);

               receivedData.Reset(HandleData(data));
               stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            } catch(Exception ex) {

               Console.WriteLine($"Error receiving TCP data {ex.Message}");
               Server.clients[id].Disconnect();
            }
         }

         private bool HandleData(byte[] data)
         {
            int packetLength = 0;

            receivedData.SetBytes(data);

            if(receivedData.UnreadLength() >= 4) {
               packetLength = receivedData.ReadInt();

               if(packetLength <= 0) {
                  return true;
               }
            }

            while(packetLength > 0 && packetLength <= receivedData.UnreadLength()) {
               byte[] packetBytes = receivedData.ReadBytes(packetLength);
               ThreadManager.ExecuteOnMainThread(() => {
                  using(Packet packet = new Packet(packetBytes)) {

                     int packetId = packet.ReadInt();
                     Server.packetHandlers[packetId](id, packet);
                  }
               });

               packetLength = 0;
               if(receivedData.UnreadLength() >= 4) {
                  packetLength = receivedData.ReadInt();

                  if(packetLength <= 0) {
                     return true;
                  }
               }
            }

            if(packetLength <= 1) {
               return true;
            }

            return false;
         }

         public void SendData(Packet packet)
         {
            try {
               if (Socket != null) {
                  stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);

               }

            } catch(Exception ex) {

               Console.WriteLine($"Error sending data to player {id} via TCP: {ex.Message}.");
            }
         }

         public void Disconnect()
         {
            Socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            Socket = null;
         }

      }

      public class UDP
      {
         public IPEndPoint endPoint;

         private int clientId;

         public UDP(int id)
         {
            clientId = id;
         }

         public void Connect(IPEndPoint endPoint)
         {
            this.endPoint = endPoint;
            //ServerSend.UDPTest(clientId);
         }

         public void Disconnect()
         {
            endPoint = null;
         }
         
         public void SendData(Packet packet)
         {
            Server.SendUDPData(endPoint, packet);
         }

         public void HandleData(Packet packet)
         {
            int packetLength = packet.ReadInt();
            byte[] data = packet.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() => {
               using(Packet packet = new Packet(data)) {
                  int packetId = packet.ReadInt();
                  Server.packetHandlers[packetId](clientId, packet);
               }
            });
         }

      }
   }
}
