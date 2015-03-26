using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
// Worked on by: Sean Coffey

using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace UnanimousOverkillGame
{
    class Room
    {
        public Room PreviousRoom
        {
            get
            {
                return previousRoom;
            }
            set
            {
                previousRoom = value;
            }
        }

        // 2D array for level
        private char[,] level;
        private List<ForegroundTile> foreground;
        private List<PhysicsEntity> colliders;
        private Room previousRoom;
        private List<Room> nextRooms;
        // This will be replaced with tile sets.
        private Texture2D tileSet;
        private Texture2D boundsTexture;
        private RoomManager manager;
        private static Random rand = new Random();

        public const int TILE_WIDTH = 50;
        public const int TILE_HEIGHT = 50;

        /// <summary>
        /// Default constructor for RoomManager
        /// </summary>
        public Room(RoomManager roomManager)
        {
            manager = roomManager;
            foreground = new List<ForegroundTile>();
            colliders = new List<PhysicsEntity>();
            nextRooms = new List<Room>();
        }

        public Room(RoomManager roomManager, Room previous)
        {
            manager = roomManager;
            previousRoom = previous;
            foreground = new List<ForegroundTile>();
            colliders = new List<PhysicsEntity>();
            nextRooms = new List<Room>();
        }

        /// <summary>
        /// Sets the tile texture. This will be editted to include texture packs.
        /// </summary>
        public void SetTileTexture(Texture2D tileTexture, Texture2D bounds)
        {
            tileSet = tileTexture;
            boundsTexture = bounds;
        }

        /// <summary>
        /// Gets a list of collidable objects in the room.
        /// </summary>
        public List<PhysicsEntity> GetColliders()
        {
            return colliders;
        }

        /// <summary>
        /// Loads a room based off a text file.
        /// </summary>
        /// <param name="path">The path to the text file.</param>
        public void LoadRoom(string path)
        {
            
            StreamReader levelReader = new StreamReader(path);
            string readerLine = levelReader.ReadLine();
            List<string> levels = new List<string>();
            while (readerLine != "//")
            {
                levels.Add(readerLine);
                readerLine = levelReader.ReadLine();
            }
            levelReader.Close();
            level = new char[levels[0].Length, levels.Count];
            int x = 0;
            int y = 0;
            foreach (string line in levels)
            {
                foreach (char tile in line)
                {
                    level[x, y] = tile;
                    x++;
                }
                y++;
                x = 0;
            }
        }

        /// <summary>
        /// Creates the actual tile objects in the world.
        /// </summary>
        public void SpawnRoom(Player player, bool forward)
        {
            foreground.Clear();
            for (int y = level.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < level.GetLength(0); x++)
                {
                    switch(level[x,y])
                    {
                        case ('*'):
                            ForegroundTile tile = new ForegroundTile(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, (int)(TILE_WIDTH * 1.33), (int)(TILE_HEIGHT * 1.33), tileSet, boundsTexture, rand.Next(3));
                            foreground.Add(tile);
                            colliders.Add(tile);
                            break;
                            // Cases for entrances and exits.
                        case ('>'):
                            if (!forward)
                            {
                                player.X = (x - 1) * TILE_WIDTH;
                                player.Y = y * TILE_HEIGHT - (player.Rect.Height-TILE_HEIGHT-1);//made this a little more generic
                                player.positionChangedManually();
                            }
                            Room room = manager.RandomRoom(this);
                            Door nextDoor = new Door(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, null, room, manager);
                            nextRooms.Add(room);
                            colliders.Add(nextDoor);
                            break;
                        case ('<'):
                            if (forward)
                            {
                                player.X = (x + 1) * TILE_WIDTH;
                                player.Y = y * TILE_HEIGHT - (player.Rect.Height - TILE_HEIGHT - 1);//made this a little more generic
                                player.positionChangedManually();
                            }
                            if (previousRoom != null)
                            {
                                Door previousDoor = new Door(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, null, previousRoom, manager);
                                colliders.Add(previousDoor);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the room.
        /// </summary>
        public void Update(GameTime time)
        {

        }

        /// <summary>
        /// Draws everything in the level.
        /// </summary>
        public void Draw(SpriteBatch batch)
        {
            foreach (ForegroundTile tile in foreground)
            {
                tile.Draw(batch);
            }
        }

        /// <summary>
        /// Show the physics bounding boxes for all tiles, for debugging.
        /// </summary>
        public void BoundsDraw(SpriteBatch batch)
        {
            foreach (ForegroundTile tile in foreground)
            {
                tile.DrawBounds(batch);
            }
        }
    }
}
