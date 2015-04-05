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

        private const int SPEED = 5;

        //Fields
        private bool[] moving; //up, right, down, left
        private MoveState cState;
        private CollisionManager collisionManager;

        //Properties
        public bool[] Moving
        {
            get { return moving; }
        }

        public WallCrawler(int x, int y, int width, int height, Texture2D texture, CollisionManager collisionManager)
            : base(x, y, width, height, texture)
        {
            moving = new bool[4] { true, false, false, false };
            this.collisionManager = collisionManager;
        }

        /// <summary>
        /// Updates the enemy's move
        /// </summary>
        public void Update(GameTime gameTime)
        {
            Move();
            CheckWalls();
        }

        /// <summary>
        /// Checks to see if the wallcrawler is still touching a wall.  If not, reacts accordingly.
        /// </summary>
        private void Move()
        {
            if (moving[0]) //Moving up
            {
                if (colliderArray[0])
                {
                    moving[0] = false;
                    Y += 5;
                    moving[1] = true;
                }
                Y = Y - SPEED;
            }
            else if (moving[1])
            { //right
                if (colliderArray[1])
                {
                    moving[1] = false;
                    X -= 5;
                    moving[2] = true;
                }
                X = X + SPEED;
            }
            else if (moving[2])
            { //down
                if (colliderArray[2])
                {
                    moving[2] = false;
                    Y -= 5;
                    moving[3] = true;
                }
                Y = Y + SPEED;
            }
            else if (moving[3])
            { //left
                if (colliderArray[3])
                {
                    moving[3] = false;
                    X += 5;
                    moving[0] = true;
                }
                X = X - SPEED;
            }
        }

        private void CheckWalls()
        {
            Rectangle tempRect = new Rectangle(0, 0, 0, 0);

            if (moving[0])
                tempRect = new Rectangle(X, Y - 6, Rect.Width, Rect.Height);
            else if (moving[1])
                tempRect = new Rectangle(X + 6, Y, Rect.Width, Rect.Height);
            else if (moving[2])
                tempRect = new Rectangle(X, Y + 6, Rect.Width, Rect.Height);
            else if (moving[3])
                tempRect = new Rectangle(X - 6, Y, Rect.Width, Rect.Height);

            //check the tempRect against other tiles, to see if WallCrawler is still hugging a wall
            for (int i = 0; i < collisionManager.Objects.Count; i++)
            {
                if ((collisionManager.Objects[i] is Tile) && tempRect.Width > 0)
                {
                    if (!tempRect.Intersects(collisionManager.Objects[i].Rect))
                    {
                        moving[0] = false;
                        moving[1] = false;
                        moving[2] = false;
                        moving[3] = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle(X, Y, Rect.Width, Rect.Height), Color.White);
        }
    }
}
