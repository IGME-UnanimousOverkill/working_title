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

enum EnemyState
{
    FaceLeft,
    FaceRight,
    Jumping
}

namespace UnanimousOverkillGame
{
    class HoppingEnemy : Enemy
    {
        private const double timeBetweenHops = 1.5;
        private EnemyState enemyState;
        private Player p;
        private const int hopDistanceAggro = 40;
        private const int hopDistnacePassive = 20;
        Random rand = new Random();

        bool jumped;
        GameTime gameTime;

        int jumpcount;
        public double distanceToPlayer;
        int count;

        Vector2 enemyLoc;
        public bool targetingPlayer;
        public HoppingEnemy(int x, int y, int width, int height, Texture2D texture, Player p)
            : base(x, y, width, height, texture)
        {
            this.MaxXV = 2;
            enemyLoc = new Vector2(x, y);
            enemyState = EnemyState.FaceRight;
            this.p = p;
            targetingPlayer = false;
            count = 0;
            activateGravity = false;
            this.acceleration = new Vector2(2.0f, 2.0f);
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            distanceToPlayer = Math.Abs(p.X - this.X);
            if (distanceToPlayer < 150)
            {
                targetingPlayer = true;
                FacePlayer();
            }
            else
                targetingPlayer = false;

            if (colliderArray[2])
            {
                activateGravity = false;
                jumped = false;
            }
            if(distanceToPlayer>15)
            Move();

            if (colliderArray[3] || colliderArray[1])
            {
                enemyState = (enemyState == EnemyState.FaceLeft) ? EnemyState.FaceRight : EnemyState.FaceLeft;
                if (enemyState == EnemyState.FaceLeft)
                    AddForce(new Vector2(-2, 0));
                else
                    AddForce(new Vector2(2, 0));
                X += (colliderArray[3] && !(colliderArray[1] && colliderArray[3])) ? 5 : -5;
                jumpcount = 0;
            }

            int check = rand.Next(-70, 90);
            if (count >= (90 + check))
            {
                count = 0;
                if (targetingPlayer)
                    FacePlayer();

                

                if (distanceToPlayer > 15)
                {
                    if (jumped == false)
                    {
                        if (jumpcount > 3)
                        {
                            check = rand.Next(0, 2);
                            if (check == 1)
                            {
                                enemyState = EnemyState.FaceLeft;
                                AddForce(new Vector2(-2, 0));
                            }
                            else
                            {
                                enemyState = EnemyState.FaceRight;
                                AddForce(new Vector2(2, 0));
                            }
                            jumpcount = 0;
                        }
                        
                        AddForce(new Vector2(0, -350));
                        activateGravity = true;
                        jumped = true;
                        
                        jumpcount++;
                    }
                }


            }
            Updates(gameTime);
            count++;
        }

        public void FacePlayer()
        {
            if (p.X + p.Rect.Width < this.X)
            {
                enemyState = EnemyState.FaceLeft;
            }
            else
            {
                enemyState = EnemyState.FaceRight;
            }
        }

        public void Move()
        {
            if (targetingPlayer)
                if (enemyState == EnemyState.FaceLeft)
                {
                    AddForce(new Vector2(-2f, 0));
                }
                else
                {
                    AddForce(new Vector2(2f, 0));
                }
            else
                if (enemyState == EnemyState.FaceLeft)
                {
                    AddForce(new Vector2(-1f, 0));
                }
                else
                {
                    AddForce(new Vector2(1f, 0));
                }
        }

        public Vector2 WorldToScreen(int worldX, int worldY)
        {
            int top = (int)p.Y - (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2);
            int left = (int)p.X - (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2);

            int screenX = (int)worldX - left;
            int screenY = (int)worldY - top;

            return new Vector2(screenX, screenY);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            enemyLoc.X = x;
            enemyLoc.Y = y;

            Vector2 drawLoc = WorldToScreen((int)enemyLoc.X, (int)enemyLoc.Y);
            switch (enemyState)
            {

                case EnemyState.FaceRight:
                    {
                        spriteBatch.Draw(texture,
                                        drawLoc,
                                        new Rectangle(
                                            0,
                                            0,
                                            25,
                                            25),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        1,
                                        SpriteEffects.None,
                                        0);
                        break;
                    }
                case EnemyState.FaceLeft:
                    {
                        spriteBatch.Draw(texture,
                                        drawLoc,
                                        new Rectangle(
                                            0,
                                            0,
                                            25,
                                            25),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        1,
                                        SpriteEffects.FlipHorizontally,
                                        0);
                        break;
                    }
            }
        }

    }
}
