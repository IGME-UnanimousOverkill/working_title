// Worked on by: Sean Coffey

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
    class ForegroundTile : Enemy
    {
        Rectangle isoRectangle;
        Texture2D boundsTexture;
        Rectangle tileRectangle;
        static Random rand = new Random();
        Color color;

        bool falling = false;
        float fallTime = 1f;
        float curTime = 0f;

        /// <summary>
        /// Instantiates a tile that physicsobjects can collide with. This will need to be editted to allow for pseudo-isometric view.
        /// </summary>
        public ForegroundTile(int x, int y, int width, int height, int isoWidth, int isoHeight, Texture2D texture, Texture2D bounds, Texture2D normal, int tileNum)
            : base(x, y, width, height, texture, normal)
        {
            isoRectangle = new Rectangle(x - ((isoWidth - width) / 2), y - ((isoHeight - height) / 2), isoWidth, isoHeight);
            boundsTexture = bounds;
            if (tileNum > 4)
            {
                tileRectangle = new Rectangle((tileNum-5) * 100, 100, 100, 100);
            }
            else
            {
                tileRectangle = new Rectangle(tileNum * 100, 0, 100, 100);
            }
            //color = new Color(120 + rand.Next(30), 120 + rand.Next(30), 120 + rand.Next(30));
            color = Color.White;
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
        {
            device.Textures[1] = normal;
            if (texture != null)
            {
                //color = new Color(rand.Next(250), rand.Next(250), rand.Next(250));
                spriteBatch.Draw(texture, new Rectangle(x - ((isoRectangle.Width - rectangle.Width) / 2), y - ((isoRectangle.Height - rectangle.Height) / 2), isoRectangle.Width, isoRectangle.Height), tileRectangle, color); 
            }
        }

        public override void Update(GameTime time)
        {
            /*
            if (falling)
            {
                curTime += (float)time.ElapsedGameTime.Milliseconds / 1000f;
                if (curTime > fallTime/2 && curTime <= fallTime)
                {
                    rectangle.Location = new Point(rectangle.X + rand.Next(-2, 3), rectangle.Y);
                }
                if (curTime > fallTime)
                {
                    activateGravity = true;
                }
            }
            */
        }

        public override void OnCollide(PhysicsEntity other)
        {
            /*
            if (other is Player)
            {
                falling = true;
            }
            */
        }
    }
}
