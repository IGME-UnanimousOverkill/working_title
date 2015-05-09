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

        private static RoomManager roomManager;

        public static RoomManager GetRoomManager { get { return roomManager; } }

        public Player player;
        private CollisionManager collisionManager;
        private Room head;

        SpriteFont font;

        private Room current;
        // This will be replaced with tile sets.
        public Texture2D tileSet;
        public Texture2D backTileSet;
        public Texture2D boundsTexture;
        public Texture2D doorTexture;
        public Texture2D bottleTexture;
        public Texture2D hopEnemyTexture;
        public Texture2D oozeEnemyTexture;
        public Texture2D spikesTexture;
        public int greatestID;

        public const string ROOM_DIR = "Content/Rooms/";
        public const string SPECIAL_DIR = "Content/SpecialRooms/";
        public ContentManager content;
        private static Random rand = new Random();
        private int currentID = 0;
        private Vector2 cameraLocation;
        private int screenWidth;
        private int screenHeight;

        private int currentDepth = 1;

        public static float MINIMAP_SCALE = 0.3f;

        public RoomManager(Player play, CollisionManager manager, SpriteFont font, ContentManager content, GraphicsDevice graphics)
        {
            this.font = font;
            player = play;
            collisionManager = manager;
            this.content = content;
            head = current;
            var screen = graphics.Viewport.Bounds;
            screenWidth = screen.Width;
            screenHeight = screen.Height;
            greatestID = 0;
            roomManager = this;
        }

        /// <summary>
        /// Changes the current room.
        /// </summary>
        public void ChangeRoom(Room room)
        {
            room.SetTileTexture(tileSet, boundsTexture, backTileSet, doorTexture, bottleTexture, hopEnemyTexture, oozeEnemyTexture, spikesTexture);
            collisionManager.ClearCollisions();
            room.SpawnRoom(player, current);
            UpdateCollisionManager(room);
            current = room;
        }

        public void UpdateCollisionManager(Room room)
        {
            collisionManager.UpdateObjects(GetColliders(room));
            collisionManager.UpdateEntities(GetEntities(room));

        }

        public int MakeID()
        {
            currentID += 1;
            if (currentID > greatestID)
                greatestID = currentID;
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
            System.IO.Stream backStream = TitleContainer.OpenStream("Content/backgroundTiles.png");
            System.IO.Stream boundStream = TitleContainer.OpenStream("Content/boundsTest.png");
            System.IO.Stream doorStream = TitleContainer.OpenStream("Content/door.png");
            System.IO.Stream bottleStream = TitleContainer.OpenStream("Content/bottle.png");
            System.IO.Stream hopEnemyStream = TitleContainer.OpenStream("Content/hopEnemy.png");
            System.IO.Stream oozeEnemyStream = TitleContainer.OpenStream("Content/Ooze.png");
            System.IO.Stream spikesStream = TitleContainer.OpenStream("Content/spikes.png");

            tileSet = Texture2D.FromStream(graphics, tileStream);
            backTileSet = Texture2D.FromStream(graphics, backStream);
            boundsTexture = Texture2D.FromStream(graphics, boundStream);
            doorTexture = Texture2D.FromStream(graphics, doorStream);
            bottleTexture = Texture2D.FromStream(graphics, bottleStream);
            hopEnemyTexture = Texture2D.FromStream(graphics, hopEnemyStream);
            oozeEnemyTexture = Texture2D.FromStream(graphics, oozeEnemyStream);
            spikesTexture = Texture2D.FromStream(graphics, spikesStream);

            tileStream.Close();
            backStream.Close();
            boundStream.Close();
            doorStream.Close();
            bottleStream.Close();
            hopEnemyStream.Close();
            oozeEnemyStream.Close();
            spikesStream.Close();

            ChangeRoom(RandomRoom(null));
        }

        /// <summary>
        /// Draws the current room.
        /// </summary>
        public void Draw(GraphicsDevice device, SpriteBatch batch)
        {
            current.Draw(device, batch);
        }

        /// <summary>
        /// Draws the current room's collision bounds, for debugging.
        /// </summary>
        public void BoundsDraw(SpriteBatch batch)
        {
            current.BoundsDraw(batch);
            Vector2 drawLoc = WorldToMinimap((int)(player.X * MINIMAP_SCALE), (int)(player.Y * MINIMAP_SCALE));
            player.DrawBounds(batch, boundsTexture, (int)drawLoc.X, (int)drawLoc.Y);
        }

        /// <summary>
        /// Creates a random room from a random file and returns it.
        /// </summary>
        public Room RandomRoom(Room previous)
        {
            if (previous != null)
            {
                currentDepth = previous.depth + 1;
            }

            Room room = new Room(this, previous, font, currentDepth);

            if (File.Exists(SPECIAL_DIR + "Room" + currentDepth + ".txt"))
            {
                room.LoadRoom(SPECIAL_DIR + "Room" + currentDepth + ".txt");
                return room;
            }

            string[] files = Directory.GetFiles(ROOM_DIR);
            room.LoadRoom(files[rand.Next(files.Length)]);

            if (current != null && current.depth >= 10)
            {
                Game1.RumbleMode = true;
            }

            return room;
        }

        public void RespawnRoom()
        {
            collisionManager.ClearCollisions();
            current.RespawnRoom();
            UpdateCollisionManager(current);
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

        public Vector2 WorldToMinimap(int worldX, int worldY)
        {
            int top = (int)(cameraLocation.Y * MINIMAP_SCALE);
            int left = (int)(cameraLocation.X * MINIMAP_SCALE);

            int screenX = (int)worldX + ((screenWidth / 5) * 4) - left;
            int screenY = (int)worldY + ((screenHeight / 5) * 4) - top;

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
