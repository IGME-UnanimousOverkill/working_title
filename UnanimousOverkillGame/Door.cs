﻿// Worked on by: Sean Coffey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace UnanimousOverkillGame
{
    class Door : Enemy
    {
        public Room destination;
        private RoomManager roomManager;

        public Door(int x, int y, int width, int height, Texture2D texture, Texture2D normal, Room dest, RoomManager manager) : base(x, y, width, height, texture, normal)
        {
            destination = dest;
            roomManager = manager;
        }

        public void UseDoor()
        {
            roomManager.ChangeRoom(destination);
        }

        /// <summary>
        /// On Collision with player, change rooms.
        /// </summary>
        public override void OnCollide(PhysicsEntity other)
        {
            if (other is Player)
            {
                (other as Player).DoorInRange = this;
            }
        }

    }
}
