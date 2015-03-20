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

//Worked on by: Jeannette Forbes

namespace UnanimousOverkillGame
{
    class WallCrawler : Enemy
    {
        //Fields
        private bool[] moving; //up, right, down, left
        private Random r;
        
        public WallCrawler(int x, int y, int width, int height, Texture2D texture)
            : base(x, y, width, height, texture)
        {
            moving = new bool[4] { false, false, false, true };
            r = new Random();
        }

        /// <summary>
        /// Updates and moves the enemy.
        /// </summary>
        public void Update()
        {
            
        }

        /// <summary>
        /// UNFINISHED!!!  Checks the states as part of the Update method.  Keeping this separate for now.
        /// </summary>
        private void CheckStates()
        {
            if (moving[0]) //Moving up
            {
                if(colliderArray[0]){
                    moving[0] = false;
                    moving[1] = true;
                }
            }
            else if(moving[1]){ //right
                if (colliderArray[1])
                {
                    moving[1] = false;
                    moving[2] = true;
                }
            }
            else if(moving[2]){ //down
                if (colliderArray[2])
                {
                    moving[2] = false;
                    moving[3] = true;
                }
            }
            else if(moving[3]){ //left
                if (colliderArray[3])
                {
                    moving[3] = false;
                    if(!colliderArray[0]){moving[0] = true;}
                }
            }

            switch (this.moveState)
            {
                
            }
        }
    }
}
