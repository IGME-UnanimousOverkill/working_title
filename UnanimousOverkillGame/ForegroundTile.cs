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
        int tile;
        static Random rand = new Random();
        Color color;

        /// <summary>
        /// Instantiates a tile that physicsobjects can collide with. This will need to be editted to allow for pseudo-isometric view.
        /// </summary>
        public ForegroundTile(int x, int y, int width, int height, int isoWidth, int isoHeight, Texture2D texture, Texture2D bounds, int tileNum)
            : base(x, y, width, height, texture)
        {
            isoRectangle = new Rectangle(x - ((isoWidth - width) / 2), y - ((isoHeight - height) / 2), isoWidth, isoHeight);
            boundsTexture = bounds;
            tile = tileNum;
            color = new Color(120 + rand.Next(30), 120 + rand.Next(30), 120 + rand.Next(30));
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            if (texture != null)
            {
                //Color c = new Color(120 + rand.Next(30), 120 + rand.Next(30), 120 + rand.Next(30));
                spriteBatch.Draw(texture, new Rectangle(x - ((isoRectangle.Width - rectangle.Width) / 2), y - ((isoRectangle.Height - rectangle.Height) / 2), isoRectangle.Width, isoRectangle.Height), new Rectangle(tile * 100, 0, 100, 100), color); 
            }
        }

        /// <summary>
        /// Temporary debug method to see tile boundaries.
        /// </summary>
        public void DrawBounds(SpriteBatch spriteBatch)
        {
            if (boundsTexture != null)
            { spriteBatch.Draw(boundsTexture, rectangle, Color.White); }
        }
    }
}
