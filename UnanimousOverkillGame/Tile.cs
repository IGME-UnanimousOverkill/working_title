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
    class Tile : GameObject
    {
        //Fields
        bool climb;
        bool collide;

        //Properties
        bool Climb 
        {
            get { return climb; }
            set { climb = value; }
        }
        bool Collide
        {
            get { return collide; }
            set { collide = value; }
        }

        /// <summary>
        /// Instantiates a tile just like a GameObject.
        /// Sets all initial booleans to false.
        /// </summary>
        public Tile(int x, int y, int width, int height, Texture2D texture)
            : base(x, y, width, height, texture)
        {
            climb = false;
            collide = false;
        }

    }
}
