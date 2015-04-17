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
//worked on by Gavin Keirstead

namespace UnanimousOverkillGame
{
    class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y, int width, int height, Texture2D texture)
            : base(x, y, width, height, texture, null)
        {

        }
        
        public override void Update(GameTime time)
        {
            //how close is the dudeski?
                //>if the player is close enough, chase 
                //>otherwise just take a stroll
            //Have I run into anything
                //>if it isn't the dude then just collide with it(turn around?)
                //>otherwise(it is the dude) hurt him, badly
        }
    }
}
