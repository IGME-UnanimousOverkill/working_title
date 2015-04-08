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

        public Enemy(int x, int y, int width, int height, Texture2D texture)
            : base(x, y, width, height, texture)
        {

        }

        public virtual void Update(GameTime time)
        {
            
        }
    }
}
