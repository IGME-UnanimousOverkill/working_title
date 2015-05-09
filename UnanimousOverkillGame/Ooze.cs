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
//enum EnemyState
//{
//    FaceLeft,
//    FaceRight,
//    Jumping
//}
namespace UnanimousOverkillGame
{
    class Ooze : Enemy
    {
        private EnemyState enemyState;//enemy state, not used
        private Player player;//reference to player
        Random rand = new Random();//random
        private const int ENEMY_WIDTH = 114;//enemy width
        private const int ENEMY_HEIGHT = 88;//enemy height
        private float scale;//scale of enemy
        GameTime gameTime;//gametime 
        Vector2 enemyLoc;//location of enemy
        int counter;//counter for move
        int collided;//how many times it has collided
         public Ooze(int x, int y, float scale, Texture2D texture, Texture2D normal, Player p)
            : base(x, y-10, (int)(ENEMY_WIDTH * scale), (int)(ENEMY_HEIGHT * scale), texture, normal)
        {
            collided = 0;
            player = p;
            enemyLoc = new Vector2(x, y-10);
            enemyState = EnemyState.FaceLeft;
            this.scale = scale;
            activateGravity = true;
            this.drag = false;
            this.MaxXV = 5;
        }
        public override void OnCollide(PhysicsEntity other)
        {
            if (other is Player)//if other is player(will only be on top or bottom) bounces player
            {
                if(player.Y + player.Rect.Height +3<Y+5)
                {
                    player.AddForce(new Vector2(0, -700));
                }
                //collide with player
                //if (player.Y + player.Rect.Height == Y + rectangle.Height)
                //{
                //    X = (player.X + player.Rect.Width <= X + 3) ? player.X + player.Rect.Width : player.X - rectangle.Width;
                //    velocity = Vector2.Zero;
                //    drag = true;
                //}
            }
            //{ 
               //enemyState = (enemyState == EnemyState.FaceLeft) ? EnemyState.FaceRight : EnemyState.FaceLeft;
            //}
            //else
            //{
            //    //if (player.Y + player.Rect.Height == Y + rectangle.Height)
            //    //    X = (player.X + player.Rect.Width <= X + 3) ? player.X + player.Rect.Width : player.X - rectangle.Width;
            //    //velocity = Vector2.Zero;
            //    //drag = true;
            //}
        }
        public override void Update(GameTime time)
        {
            this.drag = false;//no horizontal drag
            activateGravity = false;//no gravity
            if (colliderArray[2] == false)//if is not colliding on ground then gravity
                activateGravity = true;
            this.gameTime = time;//gametime reference
            //Move();
            if (colliderArray[3] || colliderArray[1])//if collided on left or right
            {
                collided++;//increase number of times collided
                if (collided >= 16)//if collided 16 times(if in succession, 1 seconds worth of collision)
                {
                    collided = 0;//reset counter
                    velocity = new Vector2((enemyState == EnemyState.FaceLeft) ? 2f : -2f, 0);//move a little in other direction
                    enemyState = (enemyState == EnemyState.FaceLeft) ? EnemyState.FaceRight : EnemyState.FaceLeft;//change direction
                }
            }
            if(velocity.X > 4)//slow down
            {
                velocity.X = 2;
            }
            if (velocity.X < -4)//slow down
            {
                velocity.X = -2;
            }
            if(counter >60)//pushes ooze a little more
            {
                drag = false;
                Move();
                counter = 0;
                collided = 0;
            }
            counter++;
            Updates(time);
        }
        public void Move()//adds force in direction its facing, will max out velocity
        {
            if (enemyState == EnemyState.FaceLeft)
            {
                AddForce(new Vector2(-100f, 0));
            }
            else
            {
                AddForce(new Vector2(100f, 0));
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
