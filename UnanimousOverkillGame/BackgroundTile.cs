#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

//Worked on by: Jeannette Forbes

namespace UnanimousOverkillGame
{


    /// <summary>
    /// Is this class really necessary?  We can make GameObject a concrete class and bypass the Tile class altogether.
    /// Or does it need bools/junk to interact with player/mobs?
    /// </summary>
    class BackgroundTile : GameObject
    {
        private Color color;
        private Rectangle tileRectangle;

        /// <summary>
        /// Instantiates a tile just like a GameObject.
        /// Sets all initial booleans to false.
        /// </summary>
        public BackgroundTile(int x, int y, int width, int height, Texture2D texture, int tileNum)
            : base(x, y, width, height, texture)
        {
            if (tileNum > 4)
            {
                tileRectangle = new Rectangle((tileNum - 5) * 75, 75, 75, 75);
            }
            else
            {
                tileRectangle = new Rectangle(tileNum * 75, 0, 75, 75);
            }
            //color = new Color(120 + rand.Next(30), 120 + rand.Next(30), 120 + rand.Next(30));
            color = Color.Gray;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            if (texture != null)
            {
                //color = new Color(rand.Next(250), rand.Next(250), rand.Next(250));
                spriteBatch.Draw(texture, new Rectangle(x, y, rectangle.Width, rectangle.Height), tileRectangle, color);
            }
        }
    }
}
