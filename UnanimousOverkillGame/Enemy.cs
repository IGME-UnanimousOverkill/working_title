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

//Worked on by: Jeannette Forbes

namespace UnanimousOverkillGame
{
    abstract class Enemy : PhysicsEntity
    {
        protected enum MoveState { moveUp, moveRight, moveDown, moveLeft,
                                 stillUp, stillRight, stillDown, stillLeft}

        //Fields
        protected MoveState moveState;
        private int hitcounter;

        public Enemy(int x, int y, int width, int height, Texture2D texture, Texture2D normal)
            : base(x, y, width, height, texture, null)
        {
            hitcounter = 0;
        }

        public virtual void Update(GameTime time)
        {
            
        }

        public void GetHit()
        {
            hitcounter++;
            if(hitcounter>=3)
            {
                Die();
            }
        }
        public void Die()
        {
            RoomManager.GetRoomManager.Current.Enemies.Remove(this);
            RoomManager.GetRoomManager.Current.Colliders.Remove(this);
        }
    }
}
