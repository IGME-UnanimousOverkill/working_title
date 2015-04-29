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
        private EnemyState enemyState;
        private Player player;
        Random rand = new Random();
        private const int ENEMY_WIDTH = 114;
        private const int ENEMY_HEIGHT = 88;
        private float scale;
        GameTime gameTime;
        Vector2 enemyLoc;
        int counter;
         public Ooze(int x, int y, float scale, Texture2D texture, Texture2D normal, Player p)
            : base(x, y-10, (int)(ENEMY_WIDTH * scale), (int)(ENEMY_HEIGHT * scale), texture, normal)
        {
            player = p;
            enemyLoc = new Vector2(x, y-10);
            enemyState = EnemyState.FaceLeft;
            this.scale = scale;
            activateGravity = true;
            this.drag = false;
        }
        public override void OnCollide(PhysicsEntity other)
        {
            if (other is Player)
            {
                if(player.Y + player.Rect.Height +3<Y+5)
                {
                    player.AddForce(new Vector2(0, -500));
                }
                //collide with player
                //if (player.Y + player.Rect.Height == Y + rectangle.Height)
                //{
                //    X = (player.X + player.Rect.Width <= X + 3) ? player.X + player.Rect.Width : player.X - rectangle.Width;
                //    velocity = Vector2.Zero;
                //    drag = true;
                //}
            }
            if(other is Enemy)
            {

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
            activateGravity = true;
            this.gameTime = time;
            //Move();
            if (colliderArray[3] || colliderArray[1])
            {
                velocity = new Vector2((enemyState == EnemyState.FaceLeft) ? 2f : -2f, 0);
                enemyState = (enemyState == EnemyState.FaceLeft) ? EnemyState.FaceRight : EnemyState.FaceLeft;
            }
            if(velocity.X > 4)
            {
                velocity.X = 1;
            }
            if (velocity.X < -3)
            {
                velocity.X = -1;
            }
            if(counter >60)
            {
                drag = false;
                Move();
                counter = 0;
            }
            counter++;
            Updates(time);
        }
        public void Move()
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
