// Worked on by: Sean Coffey
//Worked on by: Jeannette Forbes
//worked on by Gavin Keirstead

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace UnanimousOverkillGame
{

    //GameStates - Menu, Paused, Game
    enum GameState
    {
        Menu, Paused, Game
    }

    //player state enum to determine if the player is walking which direction for the animation will add more states later
    //fyi the animation doesn't fully work, it always walks where it shouldn't and i haven't looked into it yet
    enum PlayerState
    {
        FaceLeft,
        WalkLeft,
        FaceRight,
        WalkRight,
        Jumping,
        Falling,
        Dead
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public static bool RumbleMode = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RoomManager roomManager;
        CollisionManager collisionManager;
        KeyboardState kbState;
        KeyboardState prevkbState;
        SpriteFont font;
        Rectangle healthBox;
        Rectangle health;
        Rectangle intoxBox;
        //Handling switching between GameStates
        GameState gameState;
        Keys currentKey;
        Keys prevKey;
        int prevKeyCount; //makes sure pause menu isn't skipped

        Boolean enableShaders = true;

        //player
        static Player player;

        //Shader stuff
        Effect lightingEffect;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            Window.IsBorderless = true;
            Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
            graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            graphics.PreferredBackBufferHeight = screen.Bounds.Height;
            graphics.ApplyChanges();

            gameState = GameState.Menu;
            prevKeyCount = 0;
            health = new Rectangle(GraphicsDevice.Viewport.Width-200, 150,100 , 25);
            intoxBox = new Rectangle(GraphicsDevice.Viewport.Width - 200, 200, 0, 25);
            healthBox = health;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //loads the texture for the sprite sheet for the player, just using the one from the practice exercise, cause it was easier
            System.IO.Stream imageStream = TitleContainer.OpenStream("Content/player_spritesheet.png");
            Texture2D spriteSheet = Texture2D.FromStream(GraphicsDevice, imageStream);
            player = new Player(100, 228, 34, 55, 64, 70, spriteSheet, Content.Load<Texture2D>("Normals/BlankNormal.png"));

            imageStream.Close();
            collisionManager = new CollisionManager(player);
            player.CollisionManagerGet(collisionManager);

            //Loads the spriteFont
            font = Content.Load<SpriteFont>("TimesNewRoman12");

            roomManager = new RoomManager(player, collisionManager, font, Content);
            roomManager.LoadContent(GraphicsDevice);
            player.RoomManagerGet(roomManager);
            if(enableShaders)
                lightingEffect = LoadEffect("Content/Test.mgfx");
            

        }

        private Effect LoadEffect(String file)
        {
            System.IO.BinaryReader reader = new System.IO.BinaryReader(TitleContainer.OpenStream(file));
            Effect effect = new Effect(GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
            reader.Close();
            return effect;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            prevKey = currentKey;
            switch (gameState)
            {
                case GameState.Menu:

                    prevKeyCount--;
                    if (prevKeyCount < 0)
                    {
                        prevKey = Keys.Divide;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentKey = Keys.Enter;
                        if (prevKey != currentKey)
                        {
                            Initialize();
                            LoadContent();
                            gameState = GameState.Game;
                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        currentKey = Keys.Escape;
                        if (prevKey != currentKey)
                        {
                            Exit();
                        }
                    }

                    break;
                case GameState.Game:

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        currentKey = Keys.Escape;
                        if (prevKey != currentKey)
                        {
                            prevKeyCount = 10; //the amount of time that has to pass before Paused --> Menu
                            gameState = GameState.Paused;
                        }
                    }

                    //calls the player update method to get the logic for movement
                    player.Update(gameTime);
                    roomManager.Update(gameTime);
                    
                    health.Width = player.Health * 2;
                    intoxBox.Width = (int)(player.Intox * 1.5);

                    if(player.Y > 1500)
                    {
                        player.PState = PlayerState.Dead;
                    }
                    if(player.PState == PlayerState.Dead)
                    {
                        gameState = GameState.Menu;
                    }

                    kbState = Keyboard.GetState();

                    if (kbState.IsKeyDown(Keys.Up)) { player.Y -= 10; player.velocity.Y = 0; }
                    if (kbState.IsKeyDown(Keys.Down)) { player.Y += 10; player.velocity.Y = 0; }

                    if (prevkbState.IsKeyDown(Keys.OemPeriod) && kbState.IsKeyUp(Keys.OemPeriod))
                    {
                        RumbleMode = !RumbleMode;
                    }

                    collisionManager.DetectCollisions();
                    collisionManager.HandleCollisions();

                    prevkbState = kbState;

                    base.Update(gameTime);

                    break;
                case GameState.Paused:

                    prevKeyCount--;
                    if (prevKeyCount < 0)
                    {
                        prevKey = Keys.Divide;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        currentKey = Keys.Escape;
                        if (prevKey != currentKey)
                        {
                            prevKeyCount = 10;
                            gameState = GameState.Menu;
                        }
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentKey = Keys.Enter;
                        if (prevKey != currentKey)
                        {
                            gameState = GameState.Game;
                        }
                    }

                    break;
            }
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            
            switch (gameState)
            {
                case GameState.Menu:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "MENU", new Vector2(250, 120), Color.White, 0, new Vector2(0, 0), 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "Press ENTER to start", new Vector2(250, 170), Color.White);
                    break;
                case GameState.Game:

                     spriteBatch.Begin(0, null, null, null, null, lightingEffect);

                     if (enableShaders)
                     {
                         // Set params
                         EffectParameter lightPos = lightingEffect.Parameters["lightPos"];
                         EffectParameter lightColor = lightingEffect.Parameters["lightColor"];

                         lightPos.SetValue(new Vector3(roomManager.WorldToScreen(player.X, player.Y).X+140, roomManager.WorldToScreen(player.X, player.Y).Y+150, 120));
                         lightColor.SetValue(Color.White.ToVector4());
                     }
                    roomManager.Draw(GraphicsDevice, spriteBatch);

                    kbState = Keyboard.GetState();
                    // Hold down space to should tile physics boundaries.
                    roomManager.BoundsDraw(spriteBatch);

                    spriteBatch.DrawString(font, "Bottles In Inventory:" + player.bottlesOnHand
                        , new Vector2(GraphicsDevice.Viewport.Width - 200, 230), Color.Yellow);

                    spriteBatch.Draw(roomManager.tileSet, healthBox, Color.White);
                    spriteBatch.Draw(roomManager.boundsTexture, health, Color.White);
                    spriteBatch.Draw(roomManager.boundsTexture, intoxBox, Color.White);
                    break;
                case GameState.Paused:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "PAUSED", new Vector2(250, 120), Color.White, 0, new Vector2(0, 0), 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "Press ENTER to continue", new Vector2(250, 170), Color.White);
                    spriteBatch.DrawString(font, "Press ESC to go to Menu", new Vector2(250, 210), Color.White);
                    player.Health = 25;                    
                    roomManager.BoundsDraw(spriteBatch);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

