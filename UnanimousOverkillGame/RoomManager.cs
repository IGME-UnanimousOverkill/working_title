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
        private Room head;
        private Room current;
        // This will be replaced with tile sets.
        private Texture2D placeholderTexture;
        public Texture2D boundsTexture;
        public const string ROOM_DIR = "Content/Rooms/";
        private static Random rand = new Random();

        public RoomManager()
        {
            head = RandomRoom(null);
            current = head;
        }

        /// <summary>
        /// Changes the current room.
        /// </summary>
        public void ChangeRoom(Room room)
        {
            current = room;
            // Placeholder tile assignment because no tilesets yet.
            room.SetTileTexture(placeholderTexture, boundsTexture);
            room.SpawnRoom();
        }

        /// <summary>
        /// Update Method in case rooms need updating.
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            current.Update(time);
        }

        /// <summary>
        /// Returns a list of collidable objects in the current room.
        /// </summary>
        public List<PhysicsEntity> getColliders()
        {
            return current.GetColliders();
        }

        /// <summary>
        /// Loads content, such as tile sets and other room graphics.
        /// </summary>
        public void LoadContent(GraphicsDevice graphics)
        {
            System.IO.Stream tileStream = TitleContainer.OpenStream("Content/placeholder.png");
            System.IO.Stream boundStream = TitleContainer.OpenStream("Content/boundsTest.png");
            placeholderTexture = Texture2D.FromStream(graphics, tileStream);
            boundsTexture = Texture2D.FromStream(graphics, boundStream);
            tileStream.Close();
            boundStream.Close();

            current.SetTileTexture(placeholderTexture, boundsTexture);

            current.SpawnRoom();
        }

        /// <summary>
        /// Draws the current room.
        /// </summary>
        public void Draw(SpriteBatch batch)
        {
            current.Draw(batch);
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
    }
}
