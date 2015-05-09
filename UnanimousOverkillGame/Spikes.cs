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
using System.Diagnostics;
//worked on by: Gavin Keirstead
namespace UnanimousOverkillGame
{
    class Spikes : Enemy
    {
        private Player player;//reference to player

        private const int ENEMY_WIDTH = 50;//enemy width
        private const int ENEMY_HEIGHT = 50;//enemy height

        GameTime gameTime;
        static double lastAttackTime;//time since spikes attacked last

        int count;//frame counter

        Vector2 enemyLoc;//location of spikes
        public Spikes(int x, int y, Texture2D texture, Texture2D normal, Player p)
            : base(x, y, (int)(ENEMY_WIDTH), (int)(ENEMY_HEIGHT), texture, normal)
        {
            this.MaxXV = 0;
            enemyLoc = new Vector2(x, y);
            player = p;
            count = 0;
            activateGravity = false;           
        }
        public override void OnCollide(PhysicsEntity other)
        {
            if (other is Player)
            {
                //not instadeath
                if (gameTime.TotalGameTime.TotalSeconds - lastAttackTime > 3)
                {

                    AttackPlayer();
                    lastAttackTime = gameTime.TotalGameTime.TotalSeconds;
                }
                //instadeath
                //player.PState = PlayerState.Dead;
            }
        }
        public override void Update(GameTime gameTime)
        {
            activateGravity = false;//no gravity, will not fall
            this.gameTime = gameTime;//gametime reference
            Updates(gameTime);
        }
        public void AttackPlayer()
        {
            player.Health -= 10;//hurts player
            player.Color = Color.Red;//hit indicator
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
        {
            enemyLoc = new Vector2(x, y);



            spriteBatch.Draw(texture,
                                        enemyLoc,
                                        new Rectangle(
                                            0,
                                            0,
                                            ENEMY_WIDTH,
                                            ENEMY_HEIGHT),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        1f,
                                        SpriteEffects.None,
                                        0);
                    
            
        }

    }
}

    

