﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

//Worked on by: Jeannette Forbes
//Worked on by Gaving Keirstead

namespace UnanimousOverkillGame
{
    class Player : PhysicsEntity
    {
        //fields
        private int intox; //will represent the amount of goo drunk, initialized to 0
        private int health; //will show the amount of health the player currently has, initialized to 50 for now
        private bool holding; //will show whether the player is holding an object or not, initilized to false
        private PlayerState pState; //will hold the movement state the player is currently in, inside of the Game1 file
        private PlayerState prevState;
        //SpriteBatch spriteBatch;

        private SpriteBatch playerSpriteBatch;

        private Vector2 playerLoc; //holds the players position in the form
        private Texture2D spriteSheet; //holds the texture for the player, preferably a sprite sheet for animation 
        private CollisionManager col;
        private const int jumpHeight = 110;
        private int finalHeight = 0;
        //movement animation stuff
        private int frame; // The current animation frame
        private double timeCounter; // The amount of time that has passed  
        private double fps; // The speed of the animation                  
        private double timePerFrame; // The amount of time (in fractional seconds) per frame                  

        private Vector2 scale;

        private bool jumped;

        //spritesheet animation stuff
        const int WALK_FRAME_COUNT = 3; // The number of frames in the animation                 
        const int MARIO_RECT_Y_OFFSET = 116; // How far down in the image are the frames?                 
        const int MARIO_RECT_HEIGHT = 72; // The height of a single frame
        const int MARIO_RECT_WIDTH = 44; // The width of a single frame


        public PlayerState PState { get { return pState; } set { pState = value; } }
        public PlayerState PrevState { get { return prevState; } set {  prevState = value; } }

        /// <summary>
        /// initializes a player, sets the intoxication level to 0, health value to 50 and holding to false
        /// </summary>
        /// <param name="x">x position of rectangle</param>
        /// <param name="y">y position of rectangle</param>
        /// <param name="width">how wide the rectangle is</param>
        /// <param name="height">how tall the rectangle is</param>
        /// <param name="texture">the texture going into the rectangle for the player</param>
        public Player(int x, int y, int width, int height, Texture2D texture)
            : base(x, y, width, height, texture)
        {
            intox = 0;
            health = 50;
            holding = false;
            fps = 10.0;
            timePerFrame = 1.0 / fps;
            playerLoc = new Vector2(x, y); //initializes player position in form to 400,400 *** will change later
            this.X = x;
            this.Y = y;
            this.prevX = x;
            this.prevY = y;
            spriteSheet = texture; //takes the sprite sheet in here so you can "animate" in the draw method
            activateGravity = false;

            jumped = false;
            scale = new Vector2(((float)width / (float)MARIO_RECT_WIDTH), ((float)height / (float)MARIO_RECT_HEIGHT));//the scale of the image to fit given width/height
        }

        public void CollisionManagerGet(CollisionManager col)
        {
            this.col = col;
        }

        /// <summary>
        /// updates the game logic, so will change playerState, frame count and gametime, etc.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

            KeyboardState kbState = Keyboard.GetState();

            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeCounter >= timePerFrame)
            {
                frame += 1; // Adjust the frame

                if (frame > WALK_FRAME_COUNT) // Check the bounds
                    frame = 1; // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame; // Remove the time we "used"
            }
            if(!colliderArray[0] && !colliderArray[1] && !colliderArray[2] && !colliderArray[3] && pState != PlayerState.Jumping){
                pState = PlayerState.Falling;

            }
            //switch case for the player state to determine if the player is facing/walking a certain way and then changing to the next state when a key is pressed, or lifted up.
            switch (pState)
            {
                case PlayerState.FaceLeft:
                    {

                        if ((kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)) && (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)))
                        {

                        }
                        else
                        {
                            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
                            {
                                prevState = pState;
                                pState = PlayerState.WalkLeft;
                            }
                            if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
                            {
                                prevState = pState;
                                pState = PlayerState.WalkRight;
                            }
                        }
                        if (kbState.IsKeyDown(Keys.Space))
                        {
                            prevState = pState;
                            pState = PlayerState.Jumping;
                            //finalHeight = Y - jumpHeight;

                        }
                        //if (colliderArray[2] == false&&)
                        //{
                        //    prevState = pState;
                        //    pState = PlayerState.Falling;
                        //}
                        break;
                    }
                case PlayerState.FaceRight:
                    {
                        if ((kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)) && (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)))
                        {

                        }
                        else
                        {
                            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
                            {
                                prevState = pState;
                                pState = PlayerState.WalkLeft;
                            }
                            if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
                            {
                                prevState = pState;
                                pState = PlayerState.WalkRight;
                            }
                        }
                        if (kbState.IsKeyDown(Keys.Space))
                        {
                            prevState = pState;
                            pState = PlayerState.Jumping;
                            //finalHeight = Y - jumpHeight;

                        }
                        //if (colliderArray[2] == false)
                        //{
                        //    prevState = pState;
                        //    pState = PlayerState.Falling;
                        //}
                        break;
                    }
                case PlayerState.WalkRight:
                    {
                        if (!colliderArray[1])
                        {
                            AddForce(new Vector2(10, 0));
                        }

                        if ((kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)) && (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)))
                        {
                            prevState = pState;
                            pState = PlayerState.FaceRight;
                        }
                        else
                        {
                            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
                            {
                                prevState = pState;
                                pState = PlayerState.WalkLeft;
                            }
                            if (kbState.IsKeyUp(Keys.D) && kbState.IsKeyUp(Keys.Right))
                            {
                                prevState = pState;
                                pState = PlayerState.FaceRight;
                            }
                        }
                        if (!colliderArray[2])
                        {
                            prevState = pState;
                            pState = PlayerState.Falling;
                        }
                        else if (kbState.IsKeyDown(Keys.Space))
                        {
                            prevState = pState;
                            pState = PlayerState.Jumping;
                            //finalHeight = Y - jumpHeight;

                        }
                        //if (colliderArray[2] == false)
                        //{
                        //    prevState = pState;
                        //    pState = PlayerState.Falling;
                        //}
                        break;
                    }
                case PlayerState.WalkLeft:
                    {
                        if (!colliderArray[3])
                        {
                            AddForce(new Vector2(-10, 0));
                        }
                        if ((kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)) && (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)))
                        {
                            prevState = pState;
                            pState = PlayerState.FaceLeft;
                        }
                        else
                        {
                            if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
                            {
                                prevState = pState;
                                pState = PlayerState.WalkRight;
                            }
                            if (kbState.IsKeyUp(Keys.A) && kbState.IsKeyUp(Keys.Left))
                            {
                                prevState = pState;
                                pState = PlayerState.FaceLeft;
                            }
                        }
                        if (!colliderArray[2])
                        {
                            prevState = pState;
                            pState = PlayerState.Falling;
                        }
                        else if (kbState.IsKeyDown(Keys.Space))
                        {
                            prevState = pState;
                            pState = PlayerState.Jumping;
                            //finalHeight = Y - jumpHeight;
                        }
                        //if (colliderArray[2] == false)
                        //{
                        //    prevState = pState;
                        //    pState = PlayerState.Falling;
                        //}


                        break;
                    }
                case PlayerState.Jumping:
                    {
                        if (!activateGravity)
                        {
                            activateGravity = true;
                        }
                        if (colliderArray[2] == true)
                        {
                            pState = prevState;
                            jumped = false;
                            activateGravity = false;
                        }
                        //Jump(finalHeight);
                        if (!jumped)
                        {
                            AddForce(new Vector2(0, -350));
                            activateGravity = true;
                            jumped = true;
                            pState = PlayerState.Falling;
                        }
                        if (colliderArray[0] || Y <= finalHeight)
                        {
                            pState = PlayerState.Falling;
                        }
                        if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
                        {
                            if (!colliderArray[3])
                            {
                                AddForce(new Vector2(-6, 0));
                            }
                        }
                        if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
                        {
                            if (!colliderArray[1])
                            {
                                AddForce(new Vector2(6, 0));
                            }
                        }
                        break;

                    }
                case PlayerState.Falling:
                    {
                        if (!activateGravity)
                        {
                            activateGravity = true;
                        }
                        //Fall();
                        if (colliderArray[2] == true)
                        {
                            pState = prevState;
                            jumped = false;
                            activateGravity = false;
                        }
                        if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
                        {
                            if (!colliderArray[3])
                            {
                                AddForce(new Vector2(-6, 0));
                            }
                        }
                        if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
                        {
                            if (!colliderArray[1])
                            {
                                AddForce(new Vector2(6, 0));
                            }
                        }
                        break;
                    }

            }
            Updates(gameTime);
        }
        public void DrawBounds(SpriteBatch spriteBatch, Texture2D bound)//temporary for testing
        {
            if (bound != null)
            { spriteBatch.Draw(bound, rectangle, Color.White); }
        }

        /// <summary>
        /// will draw the player with the given spritebatch, intended to be called from Game1's draw
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            playerSpriteBatch = spriteBatch;
            playerLoc.X = X;
            playerLoc.Y = Y;
            //switch case for player state again to determine which way to turn the texture and to determine where in the spritesheet to take the texture from 
            switch (pState)
            {
                case PlayerState.WalkRight:
                    {
                        if (!colliderArray[1])
                        spriteBatch.Draw(spriteSheet,
                                        playerLoc,
                                        new Rectangle(
                                            frame * MARIO_RECT_WIDTH,
                                            MARIO_RECT_Y_OFFSET,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        scale,
                                        SpriteEffects.None,
                                        0);
                        else
                            spriteBatch.Draw(spriteSheet,
                                        playerLoc,
                                        new Rectangle(
                                            0,
                                            MARIO_RECT_Y_OFFSET,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        scale,
                                        SpriteEffects.None,
                                        0);
                        break;
                    }
                case PlayerState.WalkLeft:
                    {
                        if (!colliderArray[3])
                            spriteBatch.Draw(spriteSheet,
                                             playerLoc,
                                             new Rectangle(
                                                 frame * MARIO_RECT_WIDTH,
                                                 MARIO_RECT_Y_OFFSET,
                                                 MARIO_RECT_WIDTH,
                                                 MARIO_RECT_HEIGHT),
                                             Color.White,
                                             0,
                                             Vector2.Zero,
                                             scale,
                                             SpriteEffects.FlipHorizontally,
                                             0);
                        else
                            spriteBatch.Draw(spriteSheet,
                                        playerLoc,
                                        new Rectangle(
                                            0,
                                            MARIO_RECT_Y_OFFSET,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        scale,
                                        SpriteEffects.FlipHorizontally,
                                        0);
                        break;
                    }
                case PlayerState.FaceRight:
                    {
                        spriteBatch.Draw(spriteSheet,
                                        playerLoc,
                                        new Rectangle(
                                            0,
                                            MARIO_RECT_Y_OFFSET,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        scale,
                                        SpriteEffects.None,
                                        0);
                        break;
                    }
                case PlayerState.FaceLeft:
                    {
                        spriteBatch.Draw(spriteSheet,
                                        playerLoc,
                                        new Rectangle(
                                            0,
                                            MARIO_RECT_Y_OFFSET,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        scale,
                                        SpriteEffects.FlipHorizontally,
                                        0);
                        break;
                    }
                case PlayerState.Jumping:
                    {
                        if (prevState == PlayerState.FaceLeft || prevState == PlayerState.WalkLeft)
                            spriteBatch.Draw(spriteSheet, playerLoc, new Rectangle((5 * MARIO_RECT_WIDTH) - 4, MARIO_RECT_Y_OFFSET, MARIO_RECT_WIDTH - 2, MARIO_RECT_HEIGHT), Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);
                        else if (prevState == PlayerState.FaceRight || prevState == PlayerState.WalkRight)
                            spriteBatch.Draw(spriteSheet, playerLoc, new Rectangle((5 * MARIO_RECT_WIDTH) - 4, MARIO_RECT_Y_OFFSET, MARIO_RECT_WIDTH - 2, MARIO_RECT_HEIGHT), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        break;
                    }
                case PlayerState.Falling:
                    {
                        if (prevState == PlayerState.FaceLeft || prevState == PlayerState.WalkLeft)
                            spriteBatch.Draw(spriteSheet, playerLoc, new Rectangle((4 * MARIO_RECT_WIDTH) - 4, MARIO_RECT_Y_OFFSET, MARIO_RECT_WIDTH - 2, MARIO_RECT_HEIGHT), Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);
                        else if (prevState == PlayerState.FaceRight || prevState == PlayerState.WalkRight)
                            spriteBatch.Draw(spriteSheet, playerLoc, new Rectangle((4 * MARIO_RECT_WIDTH) - 4, MARIO_RECT_Y_OFFSET, MARIO_RECT_WIDTH - 2, MARIO_RECT_HEIGHT), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        break;
                    }

            }
        }
    }
}
