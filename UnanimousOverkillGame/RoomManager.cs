using System;
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
        private Texture2D boundsTexture;
        public const string ROOM_DIR = "Content/Rooms/";

        public RoomManager(string firstRoom)
        {
            head = new Room();
            head.LoadRoom(firstRoom);
            current = head;
        }

        public void Update(GameTime time)
        {

        }

        public List<PhysicsEntity> getColliders()
        {
            return current.GetColliders();
        }

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

        public void Draw(SpriteBatch batch)
        {
            current.Draw(batch);
        }

        public void BoundsDraw(SpriteBatch batch)
        {
            current.BoundsDraw(batch);
        }
    }
}
