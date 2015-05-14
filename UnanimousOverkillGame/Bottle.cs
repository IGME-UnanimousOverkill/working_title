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

//Worked on by Gavin Keirstead
namespace UnanimousOverkillGame
{
    class Bottle : Pickup
    {
        int count;//used to make the bottle hover
        bool bounce;//used to make the bottle hover
        Player player;//reference to the player
        private bool drawing;//if the bottle is supposed to be drawn
        private bool thrown;//if the bottle has been thrown
        public bool Drawing { get { return drawing; } set { drawing = value; } }//property for the drawing field
        public bool Thrown { get { return thrown; } set { thrown = value; } }//property for the drawing field
        private RoomManager rm;//room manager reference

        /// <summary>
        /// instantiates a bottle object
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="width">width of the bottle</param>
        /// <param name="height">height of the bottle</param>
        /// <param name="texture">texture for the bottle</param>
        /// <param name="p">the player</param>
        /// <param name="rm">the room manager</param>
        public Bottle(int x, int y, int width, int height, Texture2D texture, Player p, RoomManager rm)
            : base(x, y, width, height, texture)
        {
            drawing = true;
            count = 0;
            bounce = false;
            player = p;
            this.rm = rm;
            MaxXV = 7;
        }
        /// <summary>
        /// when this bottle collides with some other physics entity
        /// </summary>
        /// <param name="other">the physics entity it has collided with</param>
        public override void OnCollide(PhysicsEntity other)
        {
            
            if (other is Player)//if the other object is the player, give the bottle to the player and no longer draw it
            {
                drawing = false;
                isCollidable = false;
                player.bottlesOnHand++;
                player.Color = Color.Green;
                rm.Current.Colliders.Remove(this);
                rm.Current.Enemies.Remove(this);
                EntityIsRemoved();
            }
            if(thrown)//if it has been thrown determine what it has collided with and deal with it accordingly
            {
                if (!(other is EffectBox) && !(other is Button) && !(other is Player)&&!(other is Fan)&&!(other is Bottle)&&!(other is Spikes) && (other.IsCollidable))
                {
                    drawing = false;
                    isCollidable = false;
                    thrown = false;
                    rm.Current.Colliders.Remove(this);
                    rm.Current.Enemies.Remove(this);
                }
                    if (!(other is ForegroundTile)&&!(other is Spikes) &&!(other is PhaseBlock))
                    {
                        if (other is PhysicsEntity && !(other is Player))
                        {
                            if (other is Enemy && !(other is Door))
                                (other as Enemy).GetHit();
                            other.acceleration = Vector2.Zero;
                            other.velocity = new Vector2(this.velocity.X,0);

                            other.AddForce(new Vector2((this.X < other.X) ? 5000 : -5000, 0));
                        }
                    }
                
            }
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
        {
            if(Thrown)
            {
                if(colliderArray[2] || colliderArray[3] || colliderArray[1])
                {
                    drawing = false;
                    isCollidable = false;
                    X = -20000;
                    thrown = false;
                }
            }
            if (drawing)
            {
                device.Textures[1] = normal;
                if(!thrown)
                spriteBatch.Draw(texture, new Vector2(x, y + count / 5), new Rectangle(0, 0, 33, 51), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, 33, 51), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            if(y>1500)
            {
                drawing = false;
                isCollidable = false;
                rm.Current.Colliders.Remove(this);
                rm.Current.Enemies.Remove(this);
            }
            

            if (count < -50)
                bounce = true;
            if (count > 0)
                bounce  = false;
            if (bounce)
            {
                count++;
            }
            else
                count--;
        }
    }
}
