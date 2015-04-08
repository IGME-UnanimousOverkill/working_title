// Worked on by: Sean Coffey

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace UnanimousOverkillGame
{
    class RoomManager
    {
        public Room Current
        {
            get
            {
                return current;
            }
        }
        
        public Player player;
        private CollisionManager collisionManager;
        private Room head;
        private Room current;
        // This will be replaced with tile sets.
        private Texture2D placeholderTexture;
        public Texture2D boundsTexture;
        public const string ROOM_DIR = "Content/Rooms/";
        private static Random rand = new Random();
        private int currentID = 0;
        private Vector2 cameraLocation;
        private int screenWidth;
        private int screenHeight;

        public RoomManager(Player play, CollisionManager manager)
        {
            player = play;
            collisionManager = manager;
            head = current;
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            screenWidth = screen.Bounds.Width;
            screenHeight = screen.Bounds.Height;
        }

        /// <summary>
        /// Changes the current room.
        /// </summary>
        public void ChangeRoom(Room room)
        {
            // Placeholder tile assignment because no tilesets yet.
            room.SetTileTexture(placeholderTexture, boundsTexture);
            collisionManager.ClearCollisions();
            room.SpawnRoom(player, current);
            collisionManager.UpdateObjects(GetColliders(room));
            collisionManager.UpdateEntities(GetEntities(room));

            current = room;
        }

        public int MakeID()
        {
            currentID += 1;
            return currentID;
        }

        /// <summary>
        /// Update Method in case rooms need updating.
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            current.Update(time);
            cameraLocation = new Vector2(player.X, player.Y);
        }

        /// <summary>
        /// Returns a list of collidable objects in the current room.
        /// </summary>
        public List<PhysicsEntity> GetColliders(Room room)
        {
            return room.GetColliders();
        }

        /// <summary>
        /// Returns a list of Entities in the current room.
        /// </summary>
        public List<PhysicsEntity> GetEntities(Room room)
        {
            return room.GetEntities();
        }

        /// <summary>
        /// Loads content, such as tile sets and other room graphics.
        /// </summary>
        public void LoadContent(GraphicsDevice graphics)
        {
            System.IO.Stream tileStream = TitleContainer.OpenStream("Content/gameTiles.png");
            System.IO.Stream boundStream = TitleContainer.OpenStream("Content/boundsTest.png");
            placeholderTexture = Texture2D.FromStream(graphics, tileStream);
            boundsTexture = Texture2D.FromStream(graphics, boundStream);
            tileStream.Close();
            boundStream.Close();

            ChangeRoom(RandomRoom(null));
        }

        /// <summary>
        /// Draws the current room.
        /// </summary>
        public void Draw(SpriteBatch batch)
        {
            current.Draw(batch);
            Vector2 drawLoc = WorldToScreen(player.X, player.Y);
            player.Draw(batch, (int)drawLoc.X, (int)drawLoc.Y);
        }

        /// <summary>
        /// Draws the current room's collision bounds, for debugging.
        /// </summary>
        public void BoundsDraw(SpriteBatch batch)
        {
            current.BoundsDraw(batch);
        }

        /// <summary>
        /// Creates a random room from a random file and returns it.
        /// </summary>
        public Room RandomRoom(Room previous)
        {
            Room room = new Room(this, previous);
            
            string[] files = Directory.GetFiles(ROOM_DIR);
            room.LoadRoom(files[rand.Next(files.Length)]);

            return room;
        }


        /// <summary>
        /// Methods for converting from World Coordinates to Screen Coordinates. Mainly for drawing with a scrolling screen.
        /// </summary>

        public Vector2 WorldToScreen(Vector2 worldLoc)
        {
            int top = (int)cameraLocation.Y - (screenHeight / 2);
            int left = (int)cameraLocation.X - (screenWidth / 2);

            int screenX = (int)worldLoc.X - left;
            int screenY = (int)worldLoc.Y - top;

            return new Vector2(screenX, screenY);
        }

        public Vector2 WorldToScreen(int worldX, int worldY)
        {
            int top = (int)cameraLocation.Y - (screenHeight / 2);
            int left = (int)cameraLocation.X - (screenWidth / 2);

            int screenX = (int)worldX - left;
            int screenY = (int)worldY - top;

            return new Vector2(screenX, screenY);
        }

        public Vector2 ScreenToWorld(Vector2 screenLoc)
        {
            int top = (int)cameraLocation.Y - (screenHeight / 2);
            int left = (int)cameraLocation.X - (screenWidth / 2);

            int worldX = left + (int)screenLoc.X;
            int worldY = top + (int)screenLoc.Y;

            return new Vector2(worldX, worldY);
        }

        public Vector2 ScreenToWorld(int screenX, int screenY)
        {
            int top = (int)cameraLocation.Y - (screenHeight / 2);
            int left = (int)cameraLocation.X - (screenWidth / 2);

            int worldX = left + (int)screenX;
            int worldY = top + (int)screenY;

            return new Vector2(worldX, worldY);
        }
    }
}
