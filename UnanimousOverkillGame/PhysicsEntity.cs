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

namespace UnanimousOverkillGame
{
    /// <summary>
    /// All classes handling physics use this class.
    /// </summary>
    abstract class PhysicsEntity : GameObject
    {
        public bool[] colliderArray = new bool[4];//indicates the direction that the entity is colliding with something, [top, right, bottom, left]
        protected Vector2 velocity;//velocity vector
        protected Vector2 acceleration;//acceleration vector



        /// <summary>
        /// Instantiates a basic PhysicsEntity.  Nothing special here.
        /// </summary>
        public PhysicsEntity(int x, int y, int width, int height, Texture2D texture)
            : base(x, y, width, height, texture)
        {
            velocity = new Vector2();
            acceleration = new Vector2(0.0f,-1.0f);
        }

        public virtual void OnCollide(PhysicsEntity other)
        {
            // Manage collision with other entities
        }

        /// <summary>
        /// Entity rises a certain height into the air.
        /// Relies on Fall method to fall.
        /// </summary>
        public void Jump(Vector2 force)
        {

            Fall();
        }

        /// <summary>
        /// Causes the entity to be affected by gravity.
        /// </summary>
        public void Fall()
        {

        }
    }
}
