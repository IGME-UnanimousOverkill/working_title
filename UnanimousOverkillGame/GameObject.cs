#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UnanimousOverkillGame
{
    /// <summary>
    /// All objects within the game inherit from this class.
    /// </summary>
    abstract class GameObject
    {
        //Fields
        Texture2D texture;
        protected Rectangle rectangle;

        //Properties
        public int X
        { 
            get { return rectangle.X; }
            set { rectangle.X = value; }
        }
        public int Y
        {
            get { return rectangle.Y; }
            set { rectangle.Y = value; }
        }
        public Rectangle Rect { get { return rectangle; } }
        public Texture2D Texture
        {
            get { return texture; }
        }

        /// <summary>
        /// Instantiates a game object with a rectangle based upon its x, y, width and height.
        /// Does NOT require a texture.
        /// </summary>
        protected GameObject(int x, int y, int width, int height, Texture2D texture = null)
        {
            this.rectangle = new Rectangle(x, y, width, height);
            this.texture = texture;
        }

        /// <summary>
        /// Draws the texture, if there is one.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            { spriteBatch.Draw(texture, rectangle, Color.White); }
        }
    }
}
