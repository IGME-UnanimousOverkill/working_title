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
        int count;
        bool bounce;
        Player player;
        private bool drawing;
        private bool thrown;
        public bool Drawing { get { return drawing; } set { drawing = value; } }
        public bool Thrown { get { return thrown; } set { thrown = value; } }
        private RoomManager rm;

        public Bottle(int x, int y, int width, int height, Texture2D texture, Player p, RoomManager rm)
            : base(x, y, width, height, texture)
        {
            drawing = true;
            count = 0;
            bounce = false;
            player = p;
            this.rm = rm;
        }
        public override void OnCollide(PhysicsEntity other)
        {
            
            if (other is Player)
            {
                drawing = false;
                isCollidable = false;
                player.Intox+=5;
                player.bottlesOnHand++;
                X = -20000;
                player.Holding = true;
                player.Color = Color.Green;
            }
            if(thrown)
            {
                if (!(other is EffectBox) && !(other is Button) && !(other is Player)&&!(other is Fan)&&!(other is Bottle)&&!(other is Spikes))
                {
                    drawing = false;
                    isCollidable = false;
                    X = -20000;
                    thrown = false;
                    rm.Current.Colliders.Remove(this);
                    rm.Current.Enemies.Remove(this);
                }
                    if (!(other is BackgroundTile) && !(other is ForegroundTile))
                    {
                        if (other is PhysicsEntity && !(other is Player))
                        {
                            if (other is Enemy && !(other is Door)&&!(other is Spikes))
                                (other as Enemy).GetHit();
                            other.acceleration = Vector2.Zero;
                            other.velocity = new Vector2(this.velocity.X,0);//new Vector2(((this.X <= other.X) ? other.MaxXV + other.velocity.X : other.velocity.X-other.MaxXV), other.velocity.Y);

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
