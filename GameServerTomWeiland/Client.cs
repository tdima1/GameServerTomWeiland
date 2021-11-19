using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameServerTomWeiland
{
   class Client
   {
      public static int dataBufferSize = 4096;

      public int id;
      public TCP tcp;

      public Client(int clientId)
      {
         id = clientId;
         tcp = new TCP(id);
      }

      public class TCP
      {
         public TcpClient Socket;

         private readonly int id;
         private NetworkStream stream;
         private byte[] receiveBuffer;

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

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, "This is a message from the server.");
         }

         private void ReceiveCallback(IAsyncResult result)
         {
            try {

               int byteLength = stream.EndRead(result);

               if(byteLength <= 0) {
                  return;
               }

               byte[] data = new byte[byteLength];
               Array.Copy(receiveBuffer, data, byteLength);

               stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            } catch(Exception ex) {

               Console.WriteLine($"Error receiving TCP data {ex.Message}");
            }
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
      }
   }
}
