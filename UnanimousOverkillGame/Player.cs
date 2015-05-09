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
        public int maxHealth;//max health value
        private bool holding; //will show whether the player is holding an object or not, initilized to false
        private PlayerState pState; //will hold the movement state the player is currently in, inside of the Game1 file
        private PlayerState prevState;//previous state as of the previous frame

        private KeyboardState prevKeyboardState;//previous keyboard state as of the previous frame
        private Button buttonInRange;//if a button is in range of the player
        private Door doorInRange;//if a door is in range of a player
        //SpriteBatch spriteBatch;

        public int Health { get { return health; } set { health = value; } }//health property
        private SpriteBatch playerSpriteBatch;//the players spritebatch, acts just as a reference to the spritebatch given from the draw call
        Bottle b;//the bottle that can be thrown
        private Rectangle spriteRect; // Holds information about how the player should be drawn
        private Texture2D spriteSheet; //holds the texture for the player, preferably a sprite sheet for animation 
        private CollisionManager col;//a reference to the collision manager
        private int jumpHeight = 500;//the jump height of the player
        private int finalHeight = 0;//obsolete, was used for the previous jumping mechanics
        //movement animation stuff
        private int frame; // The current animation frame
        private double timeCounter; // The amount of time that has passed  
        private double fps; // The speed of the animation                  
        private double timePerFrame; // The amount of time (in fractional seconds) per frame                  
        private int intoxDecreaseCounter;//how long until the intoxication slowly decreases
        private bool jumped;//if the player has jumped, should reset when the player lands on some platform or the ground
        //spritesheet animation stuff
        const int WALK_FRAME_COUNT = 7; // The number of frames in the animation                 
        const int MARIO_RECT_Y_OFFSET = 70; // How far down in the image are the frames?                 
        const int MARIO_RECT_HEIGHT = 70; // The height of a single frame
        const int MARIO_RECT_X_OFFSET = 0;//the offset from the left of the spritesheet
        const int MARIO_RECT_WIDTH = 64; // The width of a single frame
        private SpriteBatch uispritebatch;
        public int deathCounter;//how many times the player has died
        private Color color;//the color to output for the player(used when the player interacts with an enemy or a bottle)
        public int bottlesOnHand;//how many bottles you are currently carrying
        public bool wallClimb;//if the player can wall climb activated after intox is >= 40
        public Button ButtonInRange { get { return buttonInRange; } set { if (value != null) buttonInRange = value; } }//property for the buttoninrange field
        public Door DoorInRange { get { return doorInRange; } set { if (value != null) doorInRange = value; } }//property for the doorinrange field
        public bool Holding { get { return holding; } set { holding = value; } }//property for the holding field
        private int holdingCounter;//how long until a bottle disappears from view after not being interacted with when holding
        RoomManager rm;//reference to the room manager
        public PlayerState PState { get { return pState; } set { pState = value; } }//property for the pstate field
        public PlayerState PrevState { get { return prevState; } set { prevState = value; } }//property for the prevstate field
        public int Intox { get { return intox; } set { intox = value; } }//property for the intox field
        public Color Color { set { color = value; } }//property for the color field
        /// <summary>
        /// initializes a player, sets the intoxication level to 0, health value to 50 and holding to false
        /// </summary>
        /// <param name="x">x position of rectangle</param>
        /// <param name="y">y position of rectangle</param>
        /// <param name="width">how wide the rectangle is</param>
        /// <param name="height">how tall the rectangle is</param>
        /// <param name="texture">the texture going into the rectangle for the player</param>
        public Player(int x, int y, int width, int height, int spriteWidth, int spriteHeight, Texture2D texture, Texture2D normal)
            : base(x, y, width, height, texture, normal)
        {
            spriteRect = new Rectangle(x - ((spriteWidth - width) / 2), y - (spriteHeight - height), spriteWidth, spriteHeight);
            buttonInRange = null;
            doorInRange = null;
            intox = 0;
            health = 100;
            maxHealth = 100;
            holding = false;
            fps = 10.0;
            timePerFrame = 1.0 / fps;
            this.X = x;
            this.Y = y;
            this.prevX = x;
            this.prevY = y;
            spriteSheet = texture; //takes the sprite sheet in here so you can "animate" in the draw method
            activateGravity = false;
            color = Color.White;
            prevKeyboardState = Keyboard.GetState();
            bottlesOnHand = 500;
            jumped = false;
            holdingCounter = 0;
            intoxDecreaseCounter = 0;
            deathCounter = 0;
            wallClimb = false;
        }
        public void getUISpriteBatch(SpriteBatch ui)
        {
            uispritebatch = ui;
        }
        //used to get a reference to the collsion manager
        public void CollisionManagerGet(CollisionManager col)
        {
            this.col = col;
        }
        //used to get a reference to the room manager
        public void RoomManagerGet(RoomManager col)
        {
            this.rm = col;
        }
        /// <summary>
        /// updates the game logic, so will change playerState, frame count and gametime, etc.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();//initializes the keyboard state for getting movement input and other commands/actions
            if (health <= 0)//resets health back to 0 if it goes below and kills the player
            {
                health = 0;
                pState = PlayerState.Dead;
            }
            if(health>maxHealth)//resets the health to maxhealth if it goes past
                health = maxHealth; 
            if (intox > 100)//if the intox goes past the max value, then resets it to it
                intox = 100;
            if (intox < 0)//keeps the intox from going below 0
                intox = 0;

            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeCounter >= timePerFrame)
            {
                frame += 1; // Adjust the frame

                if (frame > WALK_FRAME_COUNT) // Check the bounds
                    frame = 0;

                timeCounter -= timePerFrame; // Remove the time we "used"
            }
            if (!colliderArray[0] && !colliderArray[1] && !colliderArray[2] && !colliderArray[3] && pState != PlayerState.Jumping)//if the player is not colliding with anything nor jumping then it is falling
                pState = PlayerState.Falling;
            if (kbState.IsKeyDown(Keys.E) && buttonInRange != null && !prevKeyboardState.IsKeyDown(Keys.E))//if you press e while in range of a button it presses the button
                buttonInRange.PressButton();
            if (kbState.IsKeyDown(Keys.W) && doorInRange != null && !prevKeyboardState.IsKeyDown(Keys.W))//if you press w while in range of a door it opens the door and progresses to the next room
                doorInRange.UseDoor();

            MouseState mState = Mouse.GetState();//initializes the mousestate for input
            if (kbState.IsKeyDown(Keys.F) && !prevKeyboardState.IsKeyDown(Keys.F))//if you press f, and you have bottles, and you arent in the air, it makes it so you are now holding a bottle
                if (bottlesOnHand > 0)
                    if (pState != PlayerState.Falling)
                        if (holding == false)
                            holding = true;

            if (wallClimb)//if you can wall climb(intox >=40) then if you collide with something on the left or right, you no longer fall and can move up and down manually
                if (colliderArray[3] || colliderArray[1])
                {
                    activateGravity = false;
                    if (kbState.IsKeyDown(Keys.S))
                        activateGravity = true;
                    if (kbState.IsKeyDown(Keys.A)&&!prevKeyboardState.IsKeyDown(Keys.A))
                            pState = PlayerState.FaceLeft;
                    if (kbState.IsKeyDown(Keys.D) && !prevKeyboardState.IsKeyDown(Keys.D))
                        pState = PlayerState.FaceRight;
                    if (kbState.IsKeyDown(Keys.W) || kbState.IsKeyDown(Keys.Space))
                        AddForce(new Vector2(0, -20));
                }
                else
                    if (colliderArray[2] == false)
                        activateGravity = true;

            if (holding)//if you are holding a bottle, if you dont interact with it within 2 seconds it goes back into your "inventory". otherwise(you do interact with it) if you left click you throw it, if you right click you drink it
                if (holdingCounter >= 120)
                {
                    holdingCounter = 0;
                    holding = false;
                }
                else
                    if (mState.RightButton == ButtonState.Pressed)
                    {
                        holding = false;
                        intox += 5;
                        bottlesOnHand--;
                    }
                    else if (mState.LeftButton == ButtonState.Pressed)
                    {
                        holding = false;
                        if (pState == PlayerState.FaceLeft || pState == PlayerState.WalkLeft)
                        {
                            b = new Bottle((X - 60), Y - 10, Room.TILE_WIDTH, Room.TILE_HEIGHT, rm.bottleTexture, this, rm);
                            b.velocity = new Vector2(-20 + velocity.X, velocity.Y);
                            b.AddForce(new Vector2(-2000, -350));
                        }
                        else if (pState == PlayerState.FaceRight || pState == PlayerState.WalkRight)
                        {
                            b = new Bottle((X + Rect.Width + 3), Y - 10, Room.TILE_WIDTH, Room.TILE_HEIGHT, rm.bottleTexture, this, rm);
                            b.velocity = new Vector2(20 + velocity.X, velocity.Y);
                            b.AddForce(new Vector2(2000, -350));

                        }
                        b.Drawing = true;
                        b.Thrown = true;
                        b.drag = false;
                        rm.Current.Colliders.Add(b);
                        rm.Current.Enemies.Add(b);
                        rm.Current.Drawable.Add(b);
                        b.activateGravity = true;
                        bottlesOnHand--;
                    }

            if (intoxDecreaseCounter >= 1200)//after 20 seconds your intoxication level decreases by 5
            {
                intox -= 5;
                intoxDecreaseCounter = 0;
            }
            if (intox >= 40)//if intox is >= 40 you can wallclimb otherwise you cant
                wallClimb = true;
            else
                wallClimb = false;
            if (intox >= 70)//if intox >=70 you jump higher otherwise you jump at a normal height
                jumpHeight = 700;
            else
                jumpHeight = 500;

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
                            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)&&!colliderArray[1])
                            {
                                prevState = pState;
                                pState = PlayerState.WalkLeft;
                            }
                            if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)&&!colliderArray[3])
                            {
                                prevState = pState;
                                pState = PlayerState.WalkRight;
                            }
                        }
                        if (kbState.IsKeyDown(Keys.Space))
                        {
                            prevState = pState;
                            pState = PlayerState.Jumping;
                        }
                        break;
                    }
                case PlayerState.FaceRight:
                    {
                        if ((kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)) && (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)))
                        {
                        }
                        else
                        {
                            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)&&!colliderArray[1])
                            {
                                prevState = pState;
                                pState = PlayerState.WalkLeft;
                            }
                            if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)&&!colliderArray[3])
                            {
                                prevState = pState;
                                pState = PlayerState.WalkRight;
                            }
                        }
                        if (kbState.IsKeyDown(Keys.Space))
                        {
                            prevState = pState;
                            pState = PlayerState.Jumping;
                        }
                        break;
                    }
                case PlayerState.WalkRight:
                    {
                        if (!colliderArray[1])
                            AddForce(new Vector2(10, 0));
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
                        }
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
                        }
                        break;
                    }
                case PlayerState.Jumping:
                    {
                        if (!activateGravity && !wallClimb)
                            activateGravity = true;
                        if (colliderArray[2] == true)
                        {
                            pState = prevState;
                            jumped = false;
                            activateGravity = false;
                        }
                        if (!jumped)
                        {
                            AddForce(new Vector2(0, -jumpHeight));

                            activateGravity = true;
                            jumped = true;
                            pState = PlayerState.Falling;
                        }
                        if (colliderArray[0] || Y <= finalHeight)
                            pState = PlayerState.Falling;
                        if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
                            if (!colliderArray[3])
                                AddForce(new Vector2(-6, 0));
                        if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
                            if (!colliderArray[1])
                                AddForce(new Vector2(6, 0));
                        break;
                    }
                case PlayerState.Falling:
                    {
                        if (!activateGravity && !wallClimb)
                            activateGravity = true;
                        if (colliderArray[2] == true)
                        {
                            pState = prevState;
                            jumped = false;
                            activateGravity = false;
                        }
                        if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
                            if (!colliderArray[3])
                                AddForce(new Vector2(-8, 0));
                        if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
                            if (!colliderArray[1])
                                AddForce(new Vector2(8, 0));
                        break;
                    }
            }
            buttonInRange = null;
            doorInRange = null;
            prevKeyboardState = kbState;
            if (holding)
                holdingCounter++;
            if (intox > 0)
                intoxDecreaseCounter++;
            Updates(gameTime);
        }

        /// <summary>
        /// will draw the player with the given spritebatch, intended to be called from Game1's draw
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
        {
            playerSpriteBatch = spriteBatch;//gets the spritebatch reference
            spriteRect.X = x - ((spriteRect.Width - rectangle.Width) / 2);
            spriteRect.Y = y - (spriteRect.Height - rectangle.Height);
            //switch case for player state again to determine which way to turn the texture and to determine where in the spritesheet to take the texture from 
            if (holding)
            {
                if (pState == PlayerState.FaceRight || pState == PlayerState.WalkRight)
                    spriteBatch.Draw(rm.bottleTexture, new Rectangle((int)rm.WorldToScreen(this.Rect.X + this.Rect.Width, this.Rect.Y).X, (int)rm.WorldToScreen(this.Rect.X + this.Rect.Width, this.Rect.Y).Y, 24, 38), Color.White);
                else if (pState == PlayerState.FaceLeft || pState == PlayerState.WalkLeft)
                    spriteBatch.Draw(rm.bottleTexture, new Rectangle((int)rm.WorldToScreen(this.Rect.X - 24, this.Rect.Y).X, (int)rm.WorldToScreen(this.Rect.X - 24, this.Rect.Y).Y, 24, 38), Color.White);
                if (pState == PlayerState.Falling || pState == PlayerState.Jumping || pState == PlayerState.Dead)
                    holding = false;
            }
            if (color != Color.White)
            {
                switch (pState)
                {
                    case PlayerState.WalkRight:
                        {
                            if (!colliderArray[1])
                                uispritebatch.Draw(spriteSheet,
                                               spriteRect,
                                               new Rectangle(
                                                   frame * (MARIO_RECT_WIDTH + MARIO_RECT_X_OFFSET),
                                                   MARIO_RECT_Y_OFFSET,
                                                   MARIO_RECT_WIDTH,
                                                   MARIO_RECT_HEIGHT),
                                               color,
                                               0,
                                               Vector2.Zero,
                                               SpriteEffects.None,
                                               0);
                            else
                                uispritebatch.Draw(spriteSheet,
                                            spriteRect,
                                            new Rectangle(
                                                0,
                                                MARIO_RECT_Y_OFFSET,
                                                MARIO_RECT_WIDTH,
                                                MARIO_RECT_HEIGHT),
                                            color,
                                            0,
                                            Vector2.Zero,
                                            SpriteEffects.None,
                                            0);
                            break;
                        }
                    case PlayerState.WalkLeft:
                        {
                            if (!colliderArray[3])
                                uispritebatch.Draw(spriteSheet,
                                                 spriteRect,
                                                 new Rectangle(
                                                     frame * (MARIO_RECT_WIDTH + MARIO_RECT_X_OFFSET),
                                                     MARIO_RECT_Y_OFFSET,
                                                     MARIO_RECT_WIDTH,
                                                     MARIO_RECT_HEIGHT),
                                                 color,
                                                 0,
                                                 Vector2.Zero,
                                                 SpriteEffects.FlipHorizontally,
                                                 0);
                            else
                                uispritebatch.Draw(spriteSheet,
                                            spriteRect,
                                            new Rectangle(
                                                0,
                                                MARIO_RECT_Y_OFFSET,
                                                MARIO_RECT_WIDTH,
                                                MARIO_RECT_HEIGHT),
                                            color,
                                            0,
                                            Vector2.Zero,
                                            SpriteEffects.FlipHorizontally,
                                            0);
                            break;
                        }
                    case PlayerState.FaceRight:
                        {
                            uispritebatch.Draw(spriteSheet,
                                            spriteRect,
                                            new Rectangle(
                                                0,
                                                0,
                                                MARIO_RECT_WIDTH,
                                                MARIO_RECT_HEIGHT),
                                            color,
                                            0,
                                            Vector2.Zero,
                                            SpriteEffects.None,
                                            0);
                            break;
                        }
                    case PlayerState.FaceLeft:
                        {
                            uispritebatch.Draw(spriteSheet,
                                            spriteRect,
                                            new Rectangle(
                                                0,
                                                0,
                                                MARIO_RECT_WIDTH,
                                                MARIO_RECT_HEIGHT),
                                            color,
                                            0,
                                            Vector2.Zero,
                                            SpriteEffects.FlipHorizontally,
                                            0);
                            break;
                        }
                    case PlayerState.Jumping:
                        {
                            if (prevState == PlayerState.FaceLeft || prevState == PlayerState.WalkLeft)
                                uispritebatch.Draw(spriteSheet,
                                    spriteRect,
                                    new Rectangle(
                                        (1 * MARIO_RECT_WIDTH),
                                        0,
                                        MARIO_RECT_WIDTH,
                                        MARIO_RECT_HEIGHT),
                                        color,
                                        0,
                                        Vector2.Zero,
                                        SpriteEffects.FlipHorizontally,
                                        0);
                            else if (prevState == PlayerState.FaceRight || prevState == PlayerState.WalkRight)
                                uispritebatch.Draw(spriteSheet,
                                    spriteRect,
                                    new Rectangle(
                                        (1 * MARIO_RECT_WIDTH),
                                        0,
                                        MARIO_RECT_WIDTH,
                                        MARIO_RECT_HEIGHT),
                                        color,
                                        0,
                                        Vector2.Zero,
                                        SpriteEffects.None,
                                        0);
                            break;
                        }
                    case PlayerState.Falling:
                        {
                            if (prevState == PlayerState.FaceLeft || prevState == PlayerState.WalkLeft)
                                uispritebatch.Draw(spriteSheet,
                                    spriteRect,
                                    new Rectangle(
                                        (1 * MARIO_RECT_WIDTH),
                                        0,
                                        MARIO_RECT_WIDTH,
                                        MARIO_RECT_HEIGHT),
                                        color, 0,
                                        Vector2.Zero,
                                        SpriteEffects.
                                        FlipHorizontally,
                                        0);
                            else if (prevState == PlayerState.FaceRight || prevState == PlayerState.WalkRight)
                                uispritebatch.Draw(
                                    spriteSheet,
                                    spriteRect,
                                    new Rectangle(
                                        (1 * MARIO_RECT_WIDTH),
                                        0,
                                        MARIO_RECT_WIDTH,
                                        MARIO_RECT_HEIGHT),
                                        color,
                                        0,
                                        Vector2.Zero,
                                        SpriteEffects.None,
                                        0);
                            break;
                        }
                }
            }

            switch (pState)
            {
                case PlayerState.WalkRight:
                    {
                        if (!colliderArray[1])
                            spriteBatch.Draw(spriteSheet,
                                            spriteRect,
                                            new Rectangle(
                                                frame * (MARIO_RECT_WIDTH + MARIO_RECT_X_OFFSET),
                                                MARIO_RECT_Y_OFFSET,
                                                MARIO_RECT_WIDTH,
                                                MARIO_RECT_HEIGHT),
                                            color,
                                            0,
                                            Vector2.Zero,
                                            SpriteEffects.None,
                                            0);
                        else
                            spriteBatch.Draw(spriteSheet,
                                        spriteRect,
                                        new Rectangle(
                                            0,
                                            MARIO_RECT_Y_OFFSET,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        color,
                                        0,
                                        Vector2.Zero,
                                        SpriteEffects.None,
                                        0);
                        break;
                    }
                case PlayerState.WalkLeft:
                    {
                        if (!colliderArray[3])
                            spriteBatch.Draw(spriteSheet,
                                             spriteRect,
                                             new Rectangle(
                                                 frame * (MARIO_RECT_WIDTH + MARIO_RECT_X_OFFSET),
                                                 MARIO_RECT_Y_OFFSET,
                                                 MARIO_RECT_WIDTH,
                                                 MARIO_RECT_HEIGHT),
                                             color,
                                             0,
                                             Vector2.Zero,
                                             SpriteEffects.FlipHorizontally,
                                             0);
                        else
                            spriteBatch.Draw(spriteSheet,
                                        spriteRect,
                                        new Rectangle(
                                            0,
                                            MARIO_RECT_Y_OFFSET,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        color,
                                        0,
                                        Vector2.Zero,
                                        SpriteEffects.FlipHorizontally,
                                        0);
                        break;
                    }
                case PlayerState.FaceRight:
                    {
                        spriteBatch.Draw(spriteSheet,
                                        spriteRect,
                                        new Rectangle(
                                            0,
                                            0,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        color,
                                        0,
                                        Vector2.Zero,
                                        SpriteEffects.None,
                                        0);
                        break;
                    }
                case PlayerState.FaceLeft:
                    {
                        spriteBatch.Draw(spriteSheet,
                                        spriteRect,
                                        new Rectangle(
                                            0,
                                            0,
                                            MARIO_RECT_WIDTH,
                                            MARIO_RECT_HEIGHT),
                                        color,
                                        0,
                                        Vector2.Zero,
                                        SpriteEffects.FlipHorizontally,
                                        0);
                        break;
                    }
                case PlayerState.Jumping:
                    {
                        if (prevState == PlayerState.FaceLeft || prevState == PlayerState.WalkLeft)
                            spriteBatch.Draw(spriteSheet,
                                spriteRect,
                                new Rectangle(
                                    (1 * MARIO_RECT_WIDTH),
                                    0,
                                    MARIO_RECT_WIDTH,
                                    MARIO_RECT_HEIGHT),
                                    color,
                                    0,
                                    Vector2.Zero,
                                    SpriteEffects.FlipHorizontally,
                                    0);
                        else if (prevState == PlayerState.FaceRight || prevState == PlayerState.WalkRight)
                            spriteBatch.Draw(spriteSheet,
                                spriteRect,
                                new Rectangle(
                                    (1 * MARIO_RECT_WIDTH),
                                    0,
                                    MARIO_RECT_WIDTH,
                                    MARIO_RECT_HEIGHT),
                                    color,
                                    0,
                                    Vector2.Zero,
                                    SpriteEffects.None,
                                    0);
                        break;
                    }
                case PlayerState.Falling:
                    {
                        if (prevState == PlayerState.FaceLeft || prevState == PlayerState.WalkLeft)
                            spriteBatch.Draw(spriteSheet,
                                spriteRect,
                                new Rectangle(
                                    (1 * MARIO_RECT_WIDTH),
                                    0,
                                    MARIO_RECT_WIDTH,
                                    MARIO_RECT_HEIGHT),
                                    color, 0,
                                    Vector2.Zero,
                                    SpriteEffects.
                                    FlipHorizontally,
                                    0);
                        else if (prevState == PlayerState.FaceRight || prevState == PlayerState.WalkRight)
                            spriteBatch.Draw(
                                spriteSheet,
                                spriteRect,
                                new Rectangle(
                                    (1 * MARIO_RECT_WIDTH),
                                    0,
                                    MARIO_RECT_WIDTH,
                                    MARIO_RECT_HEIGHT),
                                    color,
                                    0,
                                    Vector2.Zero,
                                    SpriteEffects.None,
                                    0);
                        break;
                    }

            }
            color = Color.White;
        }
    }
}
