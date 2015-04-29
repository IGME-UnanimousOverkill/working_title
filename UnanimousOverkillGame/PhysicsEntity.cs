// Worked on by: Sean Coffey
//Worked on by: Jeannette Forbes
//Worked on by Gavin Keirstead

#region Using Statements
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
#endregion

namespace UnanimousOverkillGame
{
    /// <summary>
    /// All classes handling physics use this class.
    /// </summary>
    abstract class PhysicsEntity : GameObject
    {
        public bool[] colliderArray = new bool[4];//indicates the direction that the entity is colliding with something, [top, right, bottom, left]
        public Vector2 velocity;//velocity vector
        public Vector2 acceleration;//acceleration vector

        private float maxXVelocity = 5;
        private float maxYVelocity = 1000;

        public bool drag = true;
        public float MaxXV { get { return maxXVelocity; } set { maxXVelocity = value; } }

        public bool activateGravity;
        /// <summary> 
        /// Instantiates a basic PhysicsEntity.  Nothing special here.
        /// </summary>
        public PhysicsEntity(int x, int y, int width, int height, Texture2D texture, Texture2D normal)
            : base(x, y, width, height, texture, normal)
        {
            velocity = new Vector2();
            acceleration = new Vector2(0.0f,0.0f);
            activateGravity = false;
        }

        public PhysicsEntity(int x, int y, int width, int height, Texture2D texture, Texture2D normal, bool collidable)
            : base(x, y, width, height,collidable, texture, normal)
        {
            velocity = new Vector2();
            acceleration = new Vector2(0.0f, 0.0f);
            activateGravity = false;
        }

        public virtual void OnCollide(PhysicsEntity other)
        {
            // Manage collision with other entities
        }


        public void AddForce(Vector2 forceVector)
        {
            acceleration.X += forceVector.X;
            acceleration.Y += forceVector.Y;
        }


        public void UpdateVelocity(GameTime gameTime)
        {

            if (activateGravity)
            {
                acceleration.Y += 19.8f;
            }


            velocity.X += acceleration.X * gameTime.ElapsedGameTime.Milliseconds / 1000;

            if (Math.Abs(velocity.X) > maxXVelocity)
            {
                velocity.X = (velocity.X > 0) ? maxXVelocity : -maxXVelocity;
            }

            velocity.Y += acceleration.Y *gameTime.ElapsedGameTime.Milliseconds / 1000;

            if (Math.Abs(velocity.Y) > maxYVelocity)
            {
                velocity.Y = (velocity.Y > 0) ? maxYVelocity : -maxYVelocity;
            }
            //horizontal drag
            if (Math.Abs(acceleration.X) == 0 && velocity.X != 0 && drag)
            {
                velocity.X = .5f * velocity.X;
                if (Math.Abs(velocity.X) < .5)
                    velocity.X = 0;
            }
            else
            {
                acceleration.X = 0;
            }
            //vertical drag, idk why
            if (Math.Abs(acceleration.Y) ==0 && velocity.Y != 0)
            {
                velocity.Y = .5f * velocity.Y;
                if (Math.Abs(velocity.Y) < .5)
                    velocity.Y = 0;
            }
            else
            {
                acceleration.Y = 0;
            }



        }

        public void UpdatePosition()
        {
            X = (int)(X + velocity.X);
            Y = (int)(Y + velocity.Y);
        }

        public void Updates(GameTime gameTime)
        {
            UpdateVelocity(gameTime);
            UpdatePosition();

        }

        public virtual void DrawBounds(SpriteBatch spriteBatch, Texture2D bound, int x, int y)//temporary for testing
        {
            if (bound != null)
            { spriteBatch.Draw(bound, new Rectangle(x, y, (int)(rectangle.Width * RoomManager.MINIMAP_SCALE), (int)(rectangle.Height * RoomManager.MINIMAP_SCALE)), Color.White); }
        }

        /// <summary>
        /// obsolete
        /// Entity rises a certain height into the air.
        /// Relies on Fall method to fall.
        /// </summary>
        public void Jump(int finishHeight)
        {
            if(colliderArray[0] == false && Y>finishHeight)
            {
                Y -= 5;
            }
        }

        /// <summary>
        /// obsolete
        /// Causes the entity to be affected by "gravity".
        /// </summary>
        public void Fall()
        {
            if (colliderArray[2] == false)
            {
                Y += 5;
            }
        }
    }
}
