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
    //player state enum to determine if the player is walking which direction for the animation will add more states later
    //fyi the animation doesn't fully work, it always walks where it shouldn't and i haven't looked into it yet
    enum PlayerState
    {
        FaceLeft,
        WalkLeft,
        FaceRight,
        WalkRight,
        Jumping,
        Falling
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RoomManager roomManager;
        CollisionManager collisionManager;
        KeyboardState kbState;
        SpriteFont font;

        //player
        Player player;
        
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
            System.IO.Stream imageStream = TitleContainer.OpenStream("Content/Mario.png");
            Texture2D spriteSheet = Texture2D.FromStream(GraphicsDevice, imageStream);
            player = new Player(100, 228, 44, 70, spriteSheet);
            imageStream.Close();

            collisionManager = new CollisionManager(player);

            roomManager = new RoomManager(player, collisionManager);
            roomManager.LoadContent(GraphicsDevice);

            //Loads the spriteFont
            font = Content.Load<SpriteFont>("TimesNewRoman12");

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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //calls the player update method to get the logic for movement
            player.Update(gameTime);
            roomManager.Update(gameTime);

            // TODO: Add your update logic here
            kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Up)) { player.Y -= 5; }
            if (kbState.IsKeyDown(Keys.Down)) { player.Y += 5; }

            collisionManager.DetectCollisions();
            collisionManager.HandleCollisions();

            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            roomManager.Draw(spriteBatch);

            kbState = Keyboard.GetState();
            // Hold down space to should tile physics boundaries.
            if (kbState.IsKeyDown(Keys.Space))
            {
                roomManager.BoundsDraw(spriteBatch);
                player.DrawBounds(spriteBatch,roomManager.boundsTexture);//temporary, for testing
            }

            //calls the player draw method to actually draw the player to the screen
            player.Draw(spriteBatch);

            spriteBatch.DrawString(font, "Top: "+/*player.colliderArray[0]*/kbState.IsKeyDown(Keys.Space)+"\nRight: "+player.colliderArray[1]+"\nBottom: "+player.colliderArray[2]+"\nLeft: "+player.colliderArray[3], new Vector2(20, 400), Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
