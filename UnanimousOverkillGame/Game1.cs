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
        public static bool developerMode = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch uiSpriteBatch;
        RoomManager roomManager;
        CollisionManager collisionManager;
        KeyboardState kbState;
        KeyboardState prevkbState;
        SpriteFont font;
        Rectangle healthBox;
        Rectangle health;
        Rectangle intoxBox;
        List<int> scores = new List<int>();
        bool alreadyRead;
        //Handling switching between GameStates
        GameState gameState;
        Keys currentKey;
        Keys prevKey;
        int prevKeyCount; //makes sure pause menu isn't skipped
        Boolean enableShaders = true;
        KeyboardState devprevkbState;

        bool Fullscreen = true;

        //player
        static Player player;

        //Shader stuff
        Effect lightingEffect;
        Effect postEffect;
        Effect bumpEffect;
        RenderTarget2D rt;
        RenderTarget2D rt2;
        Texture2D bumpMap;

        int blurAmt;

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
            if (Fullscreen)
            {
                var screen = System.Windows.Forms.Screen.PrimaryScreen;
                Window.IsBorderless = true;
                Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
                graphics.PreferredBackBufferWidth = screen.Bounds.Width;
                graphics.PreferredBackBufferHeight = screen.Bounds.Height;
                graphics.ApplyChanges();
            }

            gameState = GameState.Menu;
            prevKeyCount = 0;
            intoxBox = new Rectangle(GraphicsDevice.Viewport.Width - 200, 200, 0, 25);

            //SHADERS -- render targets
            rt = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            rt2 = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            blurAmt = 0;

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
            uiSpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //loads the texture for the sprite sheet for the player, just using the one from the practice exercise, cause it was easier
            System.IO.Stream imageStream = TitleContainer.OpenStream("Content/player_spritesheet.png");
            Texture2D spriteSheet = Texture2D.FromStream(GraphicsDevice, imageStream);
            player = new Player(100, 228, 34, 55, 64, 70, spriteSheet, Content.Load<Texture2D>("Normals/BlankNormal.png"));
            player.getUISpriteBatch(uiSpriteBatch);
            imageStream.Close();
            collisionManager = new CollisionManager(player);
            player.CollisionManagerGet(collisionManager);

            //Loads the spriteFont
            font = Content.Load<SpriteFont>("TimesNewRoman12");

            roomManager = new RoomManager(player, collisionManager, font, Content, GraphicsDevice);
            roomManager.LoadContent(GraphicsDevice);
            player.RoomManagerGet(roomManager);
            if (enableShaders)
            {
                lightingEffect = LoadEffect("Content/Test.mgfx");
                postEffect = LoadEffect("Content/PostProcess.mgfx");
                bumpEffect = LoadEffect("Content/BumpMapper.mgfx");
            }
            health = new Rectangle(player.X - (player.Rect.Width + 20 - player.Rect.Width) / 2, player.Y - 30, player.Rect.Width + 20, 5);
            healthBox = health;



            System.IO.StreamReader input = null;
            if (System.IO.File.Exists("Scores.txt"))
            {
                try
                {
                    input = new System.IO.StreamReader("Scores.txt");
                    string line = null;
                    while ((line = input.ReadLine()) != null && alreadyRead == false)
                    {
                        scores.Add(Int32.Parse(line));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    input.Close();
                }
            }


            //string line;
            //// Read the file and display it line by line.
            //System.IO.StreamReader file = new System.IO.StreamReader("Scores.txt");
            //while ((line = file.ReadLine()) != null && alreadyRead ==false)
            //{
            //    scores.Add(Int32.Parse(line));
            //}
            alreadyRead = true;
            //file.Close();
        }

        private Effect LoadEffect(String file)
        {
            System.IO.BinaryReader reader = new System.IO.BinaryReader(TitleContainer.OpenStream(file));
            Effect effect = null;
            try
            {
                effect = new Effect(GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
                Console.WriteLine("SUCCESSFULLY LOADED " + file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                enableShaders = false;
            }
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
                            System.IO.StreamWriter output = null;
                            try
                            {
                                output = new System.IO.StreamWriter("Scores.txt");
                                output.AutoFlush = true;
                                foreach (int i in scores)
                                {
                                    output.WriteLine(i);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            finally
                            {
                                output.Close();
                            }

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
                    kbState = Keyboard.GetState();
                    if (kbState.IsKeyDown(Keys.P) && !(prevkbState.IsKeyDown(Keys.P)))
                    {
                        player.Health = 0;
                    }
                    prevkbState = kbState;
                    health.X = (int)(roomManager.WorldToScreen((player.X - (healthBox.Width - player.Rect.Width) / 2), (player.Y - 20)).X);
                    health.Y = (int)(roomManager.WorldToScreen((player.X - (healthBox.Width - player.Rect.Width) / 2), (player.Y - 20)).Y);
                    healthBox.X = (int)(roomManager.WorldToScreen((player.X - (healthBox.Width - player.Rect.Width) / 2), (player.Y - 20)).X);
                    healthBox.Y = (int)(roomManager.WorldToScreen((player.X - (healthBox.Width - player.Rect.Width) / 2), (player.Y - 20)).Y);
                    //calls the player update method to get the logic for movement
                    player.Update(gameTime);
                    roomManager.Update(gameTime);
                    double value = ((double)(player.Health) / player.maxHealth);
                    health.Width = (int)(value * healthBox.Width);
                    intoxBox.Width = (int)(player.Intox * 1.5);

                    if (player.Y > 1500)
                    {
                        player.PState = PlayerState.Dead;
                    }
                    if (player.PState == PlayerState.Dead)
                    {
                        player.deathCounter++;
                        if (player.deathCounter <= 5)
                            roomManager.RespawnRoom();
                        else
                        {
                            gameState = GameState.Menu;

                            int depth = roomManager.Current.depth;
                            int index = scores.Count;
                            for (int i = scores.Count - 1; i >= 0; i--)
                            {
                                if (scores[i] < depth)
                                {
                                    index = i;
                                }
                            }
                            scores.Insert(index, depth);
                            if (scores.Count > 3)
                            {
                                scores.Remove(3);
                            }

                        }
                    }

                    kbState = Keyboard.GetState();

                    if (prevkbState.IsKeyDown(Keys.OemPeriod) && kbState.IsKeyUp(Keys.OemPeriod))
                    {
                        RumbleMode = !RumbleMode;
                    }

                    collisionManager.DetectCollisions();
                    collisionManager.HandleCollisions();
                    KeyboardState DevkbState = Keyboard.GetState();
                    if (DevkbState.IsKeyDown(Keys.OemTilde) && devprevkbState.IsKeyUp(Keys.OemTilde))
                    {
                        developerMode = (developerMode) ? false : true;
                        player.activateGravity = (developerMode) ? false : true;
                    }
                    if (developerMode)
                    {
                        if (DevkbState.IsKeyDown(Keys.Up) || DevkbState.IsKeyDown(Keys.W)) { player.Y -= 10; player.velocity.Y = 0; }
                        if (DevkbState.IsKeyDown(Keys.Down) || DevkbState.IsKeyDown(Keys.S)) { player.Y += 10; player.velocity.Y = 0; }
                        if (DevkbState.IsKeyDown(Keys.Left) || DevkbState.IsKeyDown(Keys.A)) { player.PState = PlayerState.FaceLeft; player.X -= 10; player.velocity.X = 0; }
                        if (DevkbState.IsKeyDown(Keys.Right) || DevkbState.IsKeyDown(Keys.D)) { player.PState = PlayerState.FaceRight; player.X += 10; player.velocity.X = 0; }

                    }
                    devprevkbState = kbState;

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
            GraphicsDevice.SetRenderTarget(rt);
            GraphicsDevice.Clear(Color.Black);


            switch (gameState)
            {
                case GameState.Menu:
                    GraphicsDevice.SetRenderTarget(null);

                    spriteBatch.Begin();
                    uiSpriteBatch.Begin();
                    spriteBatch.DrawString(font, "MENU", new Vector2(250, 120), Color.White, 0, new Vector2(0, 0), 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "Press ENTER to start", new Vector2(250, 170), Color.White);
                    spriteBatch.End();
                    try
                    {
                        uiSpriteBatch.DrawString(font, "High Scores:  \n1:  " + scores[0]
                            , new Vector2(GraphicsDevice.Viewport.Width - 400, 230), Color.Yellow);
                    }
                    catch (Exception e)
                    { }
                    try
                    {
                        uiSpriteBatch.DrawString(font, "2:  " + scores[1]
                            , new Vector2(GraphicsDevice.Viewport.Width - 400, 270), Color.Yellow);
                    }
                    catch (Exception e)
                    { }
                    try
                    {
                        uiSpriteBatch.DrawString(font, "3:  " + scores[2]
                            , new Vector2(GraphicsDevice.Viewport.Width - 400, 290), Color.Yellow);
                    }
                    catch (Exception e)
                    { }
                    break;
                case GameState.Game:

                    spriteBatch.Begin(0, null, null, null, null, lightingEffect);
                    uiSpriteBatch.Begin();

                    if (enableShaders)
                    {
                        // Set params
                        EffectParameter lightPos = lightingEffect.Parameters["lightPos"];
                        EffectParameter lightColor = lightingEffect.Parameters["lightColor"];

                        lightPos.SetValue(new Vector3(roomManager.WorldToScreen(player.X, player.Y).X, roomManager.WorldToScreen(player.X, player.Y).Y, 12));
                        lightColor.SetValue(Color.White.ToVector4());
                    }

                    roomManager.Draw(GraphicsDevice, spriteBatch);

                    kbState = Keyboard.GetState();
                    // Hold down space to should tile physics boundaries.
                    //roomManager.BoundsDraw(uiSpriteBatch);

                    uiSpriteBatch.DrawString(font, "Bottles In Inventory:" + player.bottlesOnHand
                        , new Vector2(GraphicsDevice.Viewport.Width - 200, 230), Color.Yellow);
                    if (developerMode)
                    {
                        uiSpriteBatch.DrawString(font, "You are a god Currently"
                            , new Vector2(10, 10), Color.Red);
                    }
                    uiSpriteBatch.DrawString(font, "Room:  " + roomManager.Current.ID
                        , new Vector2(300, 230), Color.White);
                    uiSpriteBatch.DrawString(font, "Bottles In Inventory:" + player.bottlesOnHand
                        , new Vector2(GraphicsDevice.Viewport.Width - 200, 230), Color.Yellow);
                    uiSpriteBatch.DrawString(font, "lives:" +(5- player.deathCounter)
                        , new Vector2(GraphicsDevice.Viewport.Width - 200, 15), Color.Red);
                    uiSpriteBatch.Draw(roomManager.spikesTexture, healthBox, Color.Red);
                    uiSpriteBatch.Draw(roomManager.spikesTexture, healthBox, new Rectangle(0, 0, roomManager.spikesTexture.Width, roomManager.spikesTexture.Height), Color.Red, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                    uiSpriteBatch.Draw(roomManager.spikesTexture, healthBox, new Rectangle(0, 0, roomManager.spikesTexture.Width, roomManager.spikesTexture.Height), Color.Red, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

                    uiSpriteBatch.Draw(roomManager.boundsTexture, health, Color.White);
                    uiSpriteBatch.Draw(roomManager.boundsTexture, intoxBox, Color.White);
                    uiSpriteBatch.DrawString(font, "intoxication: " + player.Intox, new Vector2(intoxBox.X - 74, intoxBox.Y + 2), Color.Red);
                    spriteBatch.End();

                    /*
                     *  SHADER STUFF
                     */
                    if (enableShaders)
                    {
                        GraphicsDevice.SetRenderTarget(rt2);
                        GraphicsDevice.Clear(Color.White);

                        spriteBatch.Begin(0, null, null, null, null, postEffect);

                        EffectParameter pixelSize = postEffect.Parameters["pixelSize"];
                        EffectParameter blurAmount = postEffect.Parameters["blurAmount"];
                        EffectParameter rampCount = postEffect.Parameters["rampCount"];

                        //Don't change this!
                        if (pixelSize != null) pixelSize.SetValue(new Vector2(1.0f / GraphicsDevice.Viewport.Width, 1.0f / GraphicsDevice.Viewport.Height));

                        //OK to fiddle with
                        if (blurAmount != null && player.Intox > 100) blurAmount.SetValue(blurAmt);
                        if (blurAmt > 0) blurAmt--;
                        if (rampCount != null) rampCount.SetValue(255);

                        spriteBatch.Draw(rt, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                        spriteBatch.End();

                        GraphicsDevice.SetRenderTarget(null);
                        GraphicsDevice.Clear(Color.Red);
                        GraphicsDevice.Textures[1] = bumpMap;

                        EffectParameter camPos = bumpEffect.Parameters["camPos"];
                        EffectParameter mouseOffset = bumpEffect.Parameters["mouseOffset"];

                        spriteBatch.Begin(0, null, null, null, null, bumpEffect);
                        spriteBatch.Draw(rt2, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                        spriteBatch.End();
                    }

                    spriteBatch.Begin();
                    GraphicsDevice.SetRenderTarget(null);
                    spriteBatch.Draw(rt, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.End();

                    break;
                case GameState.Paused:
                    GraphicsDevice.SetRenderTarget(null);

                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "PAUSED", new Vector2(250, 120), Color.White, 0, new Vector2(0, 0), 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "Press ENTER to continue", new Vector2(250, 170), Color.White);
                    spriteBatch.DrawString(font, "Press ESC to go to Menu", new Vector2(250, 210), Color.White);
                    roomManager.BoundsDraw(spriteBatch);
                    spriteBatch.End();
                    break;
            }

            uiSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

