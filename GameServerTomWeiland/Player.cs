using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServerTomWeiland
{
   class Player
   {
      public int id;
      public string username;

      public Vector3 position;
      public Quaternion rotation;

      public Player(int playerId, string playerUsername, Vector3 spawnPosition)
      {
         id = playerId;
         username = playerUsername;
         position = spawnPosition;
         rotation = Quaternion.Identity;
      }
   }
}
