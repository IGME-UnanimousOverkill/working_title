using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Text;

namespace UnanimousOverkillGame
{
    class PhaseBlock : ForegroundTile, IsClickableObject
    {
        private bool phasedIn;

        void IsClickableObject.onClick()
        {
            phasedIn = !phasedIn;
            isCollidable = phasedIn;
        }


        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            if(phasedIn)
                base.Draw(spriteBatch, x, y);
        }

        public PhaseBlock(int x, int y, int width, int height, int isoWidth, int isoHeight, Texture2D texture, Texture2D bounds, int tileNum, bool phasedIn)
            : base(x, y, width, height, isoWidth, isoHeight, texture, bounds, tileNum)
        {
            this.phasedIn = phasedIn;
            
            isCollidable = phasedIn;
        }
            

    }
}
