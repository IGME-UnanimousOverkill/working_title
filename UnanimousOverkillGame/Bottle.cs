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

//Worked on by Gavin Keirstead
namespace UnanimousOverkillGame
{
    class Bottle : Pickup
    {
        int count;
        bool bounce;
        Player player;
        private bool draw;
        public Bottle(int x, int y, int width, int height, Texture2D texture, Player p)
            : base(x, y, width, height, texture)
        {
            draw = true;
            count = 0;
            bounce = false;
            player = p;
        }
        public override void OnCollide(PhysicsEntity other)
        {
            if (other is Player)
            {
                draw = false;
                isCollidable = false;
                player.Intox+=5;
                X = -20;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            if(draw)
                spriteBatch.Draw(texture, new Vector2(x, y+count/5), new Rectangle(0, 0, 25, 25), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (count < -50)
                bounce = true;
            if (count > 50)
                bounce  = false;
            if (bounce)
            {
                count++;
            }
            else
                count--;
        }
    }
}
