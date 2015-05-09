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
//worked on by: Gavin Keirstead
namespace UnanimousOverkillGame
{
    class HoppingEnemy : Enemy
    {
        private const double timeBetweenHops = 1.5;//the time between each hop
        private EnemyState enemyState;//holds the state in which the enemy is i.e. which way it is facing
        private Player player;//reference to the player
        private const int hopDistanceAggro = 40;//the distance it will hop when targeting the player
        private const int hopDistnacePassive = 20;//the distance it will hop when not targeting the player
        Random rand = new Random();

        private const int ENEMY_WIDTH = 114;//holds the width of the enemy
        private const int ENEMY_HEIGHT = 88;//holds the height of the enemy
        private float scale;//the scale value for the enemy

        bool jumped;//whether the enemy has jumped
        GameTime gameTime;
        double lastAttackTime;//the time it last attacked

        int jumpcount;//how many times it has jumped in the current direction
        public double distanceToPlayer;//the x distance to the player
        int count;//used to determine when to possibly change directions 

        Vector2 enemyLoc;//location of the enemy
        public bool targetingPlayer;//whether the enemy is targeting the player or not
        public HoppingEnemy(int x, int y, float scale, Texture2D texture, Texture2D normal, Player p)
            : base(x, y, (int)(ENEMY_WIDTH * scale), (int)(ENEMY_HEIGHT * scale), texture, normal)
        {
            this.MaxXV = 2;
            this.scale = scale;
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
                if (player.Y + player.Rect.Height == Y + rectangle.Height)
                    X = (player.X  <= X) ? player.X + player.Rect.Width : player.X - rectangle.Width;
                if (gameTime.TotalGameTime.TotalSeconds - lastAttackTime > 1.5)//if it has been 1.5 seconds since the last attack, attack
                {
                    AttackPlayer();
                    lastAttackTime = gameTime.TotalGameTime.TotalSeconds;
                }
            }
            if (other is Enemy)
            { 

            }
        }
        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            distanceToPlayer = Math.Abs((player.X + player.Rect.Width) / 2 - (this.X + rectangle.Width) / 2);
            if (distanceToPlayer < 150)//if player is within 150 units horizontally target the player
            {
                targetingPlayer = true;
                FacePlayer();
            }
            else
                targetingPlayer = false;

            if (colliderArray[2])//disable gravity when on ground and allow for another jump
            {
                activateGravity = false;
                jumped = false;
            }
            if (distanceToPlayer > 10)//if with 10 units of player dont move otherwise move
                Move();

            if (colliderArray[3] || colliderArray[1])//if colliding on left or right, and not in contact with player turn around and reset jumps
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
            if(!colliderArray[2])//if not on ground activate gravity
            {
                activateGravity = true;
            }
            int check = rand.Next(-70, 90);
            if (count >= (90 + check))//if it has been 20 to 180 frames(1/3 second to 3 seconds
            {
                count = 0;//reset the count for frame checking
                if (targetingPlayer)//face the player if you are targeting the player
                    FacePlayer();
                if (jumped == false)//if the enemy has not jumped
                {
                    if (jumpcount > 6)//if the enemy has jumped more than 6 times, turn left or right randomly and reset jumps in that direction
                    {
                        check = rand.Next(0, 2);
                        if (Math.Abs((player.X + player.Rect.Width) / 2 - (rectangle.Width + X) / 2) > (player.Rect.Width / 2 + rectangle.Width / 2 + 5))
                            if (check == 1)
                            {
                                enemyState = EnemyState.FaceLeft;
                            }
                            else if (check == 0)
                            {
                                enemyState = EnemyState.FaceRight;
                            }
                        //velocity = (new Vector2((enemyState == EnemyState.FaceRight) ? -20 : 20, 0));
                        //AddForce(new Vector2((enemyState == EnemyState.FaceRight)?-20:20, 0));

                        jumpcount = 0;
                    }
                    if (Math.Abs((player.X + player.Rect.Width) / 2 - (rectangle.Width + X) / 2) > (player.Rect.Width / 2 + rectangle.Width / 2 + 5))//jump if not in contact with player
                        AddForce(new Vector2(0, -350));
                    activateGravity = true;//fall
                    jumped = true;//enemy has jumped

                    jumpcount++;//increment jump count
                }
            }

            Updates(gameTime);
            count++;
        }

        public void AttackPlayer()
        {
            player.Health -= 10;//decrease player health
            player.AddForce(new Vector2((X < player.X) ? 200 : -200, 0));//push player away
            player.velocity = (new Vector2((X < player.X) ? 1000 : -1000, 0));//emphasis on push
            player.Color = Color.Red;//hit indicator
        }

        public void FacePlayer()//faces player
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

        public void Move()//moves in the direction the enemy is facing
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

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
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
                                        scale,
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
                                        scale,
                                        SpriteEffects.None,
                                        0);
                        break;
                    }
            }
        }

    }
}
