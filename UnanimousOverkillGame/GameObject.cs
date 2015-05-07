#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

//Worked on by: Jeannette Forbes
//Worked on by Gavin Keirstead
namespace UnanimousOverkillGame
{
    /// <summary>
    /// All objects within the game inherit from this class.
    /// </summary>
    abstract class GameObject
    {
        //Fields
        protected Texture2D texture;
        protected Texture2D normal;
        protected Rectangle rectangle;


        protected int prevX;
        protected int prevY;

        private bool positionLocked;

        protected bool isCollidable;

        public bool PositionLocked { get { return positionLocked; } set { positionLocked = value; } }

        //Properties

        public int PrevX { get { return prevX; } }
        public int PrevY { get { return prevY; } }
        public int X
        { 
            get { return rectangle.X; }
            set { if (!positionLocked) { prevX = rectangle.X; rectangle.X = value; onPositionChange(); } }
        }
        public int Y
        {
            get { return rectangle.Y; }
            set { if (!positionLocked) { prevY = rectangle.Y; rectangle.Y = value; onPositionChange(); } }
        }
        public Rectangle Rect { get { return rectangle; } }
        public Texture2D Texture
        {
            get { return texture; }
        }

        public Boolean IsCollidable { get { return isCollidable; } }


        protected virtual void onPositionChange()
        {

        }

        /// <summary>
        /// Instantiates a game object with a rectangle based upon its x, y, width and height.
        /// Does NOT require a texture.
        /// </summary>
        protected GameObject(int x, int y, int width, int height, Texture2D texture = null, Texture2D normal = null)
        {
            this.rectangle = new Rectangle(x, y, width, height);
            this.normal = normal;
            this.texture = texture;
            isCollidable = true;
        }

        protected GameObject(int x, int y, int width, int height, Boolean Collidable, Texture2D texture = null, Texture2D normal = null)
        {
            this.rectangle = new Rectangle(x, y, width, height);
            this.normal = normal;
            this.texture = texture;
            isCollidable = Collidable;
        }

        public void positionChangedManually()
        {
            prevX = X;
            prevY = Y;
        }

        public virtual void AddInformation(List<String> infoLines, GameObject[,] objects)
        {
            //
        }

        /// <summary>
        /// Draws the texture, if there is one.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
        {
            device.Textures[1] = normal;
            if (texture != null)
            { spriteBatch.Draw(texture, new Rectangle(x, y, rectangle.Width, rectangle.Height), Color.White); }
        }
    }
}
