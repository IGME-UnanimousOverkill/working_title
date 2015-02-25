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

        /// <summary>
        /// Instantiates a tile that physicsobjects can collide with. This will need to be editted to allow for pseudo-isometric view.
        /// </summary>
        public ForegroundTile(int x, int y, int width, int height, Texture2D texture)
            : base(x, y, width, height, texture)
        {

        }
    }
}
