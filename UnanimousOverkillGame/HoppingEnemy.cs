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
        private Player player;
        private const int hopDistanceAggro = 40;
        private const int hopDistnacePassive = 20;
        Random rand = new Random();

        private const int ENEMY_WIDTH = 114;
        private const int ENEMY_HEIGHT = 88;

        bool jumped;
        GameTime gameTime;
        double lastAttackTime;
        int lastAttackTime2;


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
            enemyState = EnemyState.FaceLeft;
            player = p;
            targetingPlayer = false;
            count = 0;
            activateGravity = false;
            this.acceleration = new Vector2(2.0f, 2.0f);
           
        }
        public override void OnCollide(PhysicsEntity other)
        {
            if (other is Player)
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastAttackTime > 1.5)
                {
                    AttackPlayer();
                    lastAttackTime = gameTime.TotalGameTime.TotalSeconds;
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            distanceToPlayer = Math.Abs((player.X +player.Rect.Width)/2 - (this.X + rectangle.Width)/2);
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
            if (distanceToPlayer > 10 )
                Move();

            if (colliderArray[3] || colliderArray[1])
            {
                if (Math.Abs((player.X + player.Rect.Width) / 2 - (rectangle.Width + X) / 2) > (player.Rect.Width / 2 + rectangle.Width / 2 + 5))
                        enemyState = (colliderArray[3]) ? EnemyState.FaceRight : EnemyState.FaceLeft;
                //if (enemyState == EnemyState.FaceLeft)
                //    AddForce(new Vector2(-6, 0));
                //else
                //    AddForce(new Vector2(6, 0));
                //X += (colliderArray[3] && !(colliderArray[1] && colliderArray[3])) ? 5 : -5;
                jumpcount = 0;
            }

            int check = rand.Next(-70, 90);
            if (count >= (90 + check))
            {
                count = 0;
                if (targetingPlayer)
                    FacePlayer();
                if (jumped == false)
                {
                    if (jumpcount > 6)
                    {
                        check = rand.Next(0, 2);
                        if (Math.Abs((player.X + player.Rect.Width) / 2 - (rectangle.Width + X) / 2) > (player.Rect.Width / 2 + rectangle.Width / 2 + 5))
                        if (check == 1  )
                        {
                            enemyState = EnemyState.FaceLeft;
                        }
                        else if (check == 0 )
                        {
                            enemyState = EnemyState.FaceRight;
                        }
                        //velocity = (new Vector2((enemyState == EnemyState.FaceRight) ? -20 : 20, 0));
                        //AddForce(new Vector2((enemyState == EnemyState.FaceRight)?-20:20, 0));

                        jumpcount = 0;
                    }
                    if (Math.Abs((player.X + player.Rect.Width )/2 - (rectangle.Width + X)/2) > (player.Rect.Width/2 + rectangle.Width/2 + 5))
                        AddForce(new Vector2(0, -350));
                    activateGravity = true;
                    jumped = true;

                    jumpcount++;
                }
            }

            Updates(gameTime);
            count++;
            lastAttackTime2++;
        }

        public void AttackPlayer()
        {
            player.Health -= 5;
            player.AddForce(new Vector2((X < player.X) ? 200 : -200, 0));
            player.velocity = (new Vector2((X < player.X) ? 1000 : -1000, 0));
        }

        public void FacePlayer()
        {
            if (player.X < this.X)
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

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            enemyLoc = new Vector2(x, y);

            switch (enemyState)
            {

                case EnemyState.FaceRight:
                    {
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
                                        1,
                                        SpriteEffects.FlipHorizontally,
                                        0);
                        break;
                    }
                case EnemyState.FaceLeft:
                    {
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
                                        1,
                                        SpriteEffects.None,
                                        0);
                        break;
                    }
            }
        }

    }
}
