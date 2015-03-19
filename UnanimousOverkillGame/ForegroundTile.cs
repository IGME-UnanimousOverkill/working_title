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

        /// <summary>
        /// Instantiates a tile that physicsobjects can collide with. This will need to be editted to allow for pseudo-isometric view.
        /// </summary>
        public ForegroundTile(int x, int y, int width, int height, int isoWidth, int isoHeight, Texture2D texture, Texture2D bounds)
            : base(x, y, width, height, texture)
        {
            isoRectangle = new Rectangle(x - ((isoWidth - width) / 2), y - ((isoHeight - height) / 2), isoWidth, isoHeight);
            boundsTexture = bounds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            { spriteBatch.Draw(texture, isoRectangle, Color.White); }
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
