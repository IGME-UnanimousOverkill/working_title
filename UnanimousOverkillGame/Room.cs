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

        public int ID;

        private readonly float ISO_SCALE = 1.33f;

        // 2D array for level
        private char[,] level;
        private GameObject[,] levelObjects;
        private List<ForegroundTile> foreground;
        private List<BackgroundTile> background;
        private List<PhysicsEntity> colliders;
        private List<Enemy> enemies;
        private List<Door> doors;
        private Room previousRoom;
        private List<Room> nextRooms;
        // This will be replaced with tile sets.
        private Texture2D tileSet;
        private Texture2D backgroundSet;
        private Texture2D boundsTexture;
        private RoomManager manager;
        private static Random rand = new Random();

        List<RoomAdditionalInfo> addInfos;
        List<string> levelsInfo;
        private SpriteFont roomFont;
        private Dictionary<Vector2, IsClickableObject> clickables;
        private List<IsClickableObject> tempClickables;

        public const int TILE_WIDTH = 50;
        public const int TILE_HEIGHT = 50;

        /// <summary>
        /// Default constructor for RoomManager
        /// </summary>
        public Room(RoomManager roomManager, SpriteFont roomFont)
        {
            this.roomFont = roomFont;
            manager = roomManager;
            foreground = new List<ForegroundTile>();
            background = new List<BackgroundTile>();
            colliders = new List<PhysicsEntity>();
            tempClickables = new List<IsClickableObject>();
            enemies = new List<Enemy>();
            nextRooms = new List<Room>();
            doors = new List<Door>();
            ID = manager.MakeID();
        }

        public Room(RoomManager roomManager, Room previous, SpriteFont roomFont)
        {
            this.roomFont = roomFont;
            manager = roomManager;
            previousRoom = previous;
            foreground = new List<ForegroundTile>();
            background = new List<BackgroundTile>();
            colliders = new List<PhysicsEntity>();
            tempClickables = new List<IsClickableObject>();
            enemies = new List<Enemy>();
            nextRooms = new List<Room>();
            doors = new List<Door>();
            ID = manager.MakeID();
        }

        /// <summary>
        /// Sets the tile texture. This will be editted to include texture packs.
        /// </summary>
        public void SetTileTexture(Texture2D tileTexture, Texture2D bounds, Texture2D backBounds)
        {
            tileSet = tileTexture;
            boundsTexture = bounds;
            backgroundSet = backBounds;
        }

        /// <summary>
        /// Gets a list of collidable objects in the room.
        /// </summary>
        public List<PhysicsEntity> GetColliders()
        {
            return colliders;
        }

        /// <summary>
        /// Gets a list of collidable objects in the room.
        /// </summary>
        public List<PhysicsEntity> GetEntities()
        {
            List<PhysicsEntity> entities = new List<PhysicsEntity>();
            entities.Add(manager.player);
            entities.AddRange(enemies);
            return entities;
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
            levelsInfo = new List<string>();
            while (readerLine != "//")
            {
                levels.Add(readerLine);
                readerLine = levelReader.ReadLine();
            }
            readerLine = levelReader.ReadLine();
            while (readerLine != null)
            {
                levelsInfo.Add(readerLine);
                readerLine = levelReader.ReadLine();
            }
            levelReader.Close();
            level = new char[levels[0].Length, levels.Count];
            levelObjects = new GameObject[levels[0].Length, levels.Count];
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
        public void SpawnRoom(Player player, Room lastRoom)
        {
            addInfos = new List<RoomAdditionalInfo>();
            if (colliders.Count > 0)
            {
                foreach (Door door in doors)
                {
                    if (door.destination == lastRoom && door.destination != previousRoom)
                    {
                        player.X = door.X - TILE_WIDTH;
                        player.Y = door.Y + TILE_HEIGHT - player.Rect.Height;//made this a little more generic
                        player.positionChangedManually();
                        player.velocity = Vector2.Zero;
                    }
                    else if (door.destination == lastRoom)
                    {
                        player.X = door.X + TILE_WIDTH;
                        player.Y = door.Y + TILE_HEIGHT - player.Rect.Height;//made this a little more generic
                        player.positionChangedManually();
                        player.velocity = Vector2.Zero;
                    }
                }
                return;
            }
            foreground.Clear();
            int exitNum = 0;
            for (int y = level.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < level.GetLength(0); x++)
                {
                    switch (level[x, y])
                    {
                        case ('*'):
                            ForegroundTile tile = new ForegroundTile(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, (int)(TILE_WIDTH * ISO_SCALE), (int)(TILE_HEIGHT * ISO_SCALE), tileSet, boundsTexture, rand.Next(3));

                            int percent = rand.Next(100);
                            if (percent > 80)
                            {
                                tile.activateGravity = true;
                            }
                            foreground.Add(tile);
                            colliders.Add(tile);
                            levelObjects[x, y] = tile;
                            break;
                        // Cases for entrances and exits.
                        case ('>'):
                            if (nextRooms.Contains(lastRoom))
                            {
                                if (nextRooms[exitNum] == lastRoom)
                                {
                                    player.X = (x - 1) * TILE_WIDTH;
                                    player.Y = ((y + 1) * TILE_HEIGHT) - player.Rect.Height;//made this a little more generic
                                    player.positionChangedManually();
                                    player.velocity = Vector2.Zero;
                                }

                                exitNum++;
                            }
                            else
                            {
                                Room room = manager.RandomRoom(this);
                                Door nextDoor = new Door(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, null, room, manager);
                                nextRooms.Add(room);
                                colliders.Add(nextDoor);
                                levelObjects[x, y] = nextDoor;
                                doors.Add(nextDoor);
                            }
                            break;
                        case ('<'):
                            if (previousRoom != null)
                            {
                                if (previousRoom == lastRoom)
                                {
                                    player.X = (x + 1) * TILE_WIDTH;
                                    player.Y = ((y + 1) * TILE_HEIGHT) - player.Rect.Height;//made this a little more generic
                                    player.positionChangedManually();
                                    player.velocity = Vector2.Zero;
                                }
                                Door previousDoor = new Door(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, null, previousRoom, manager);
                                colliders.Add(previousDoor);
                                doors.Add(previousDoor);

                                levelObjects[x, y] = previousDoor;
                            }
                            break;
                        case ('┴'):
                            Fan fan = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan);
                            colliders.AddRange(fan.getEffects());

                            levelObjects[x, y] = fan;
                            break;
                        case ('┤'):
                            Fan fan1 = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan1);
                            colliders.AddRange(fan1.getEffects());

                            levelObjects[x, y] = fan1;
                            break;
                        case ('┬'):
                            Fan fan2 = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan2);
                            colliders.AddRange(fan2.getEffects());

                            levelObjects[x, y] = fan2;
                            break;
                        case ('├'):
                            Fan fan3 = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan3);
                            colliders.AddRange(fan3.getEffects());

                            levelObjects[x, y] = fan3;
                            break;
                        case ('h'):
                            HoppingEnemy hopEnemy = new HoppingEnemy(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH / 2, TILE_HEIGHT / 2, tileSet, player);
                            colliders.Add(hopEnemy);
                            enemies.Add(hopEnemy);

                            levelObjects[x, y] = hopEnemy;
                            break;
                        case ('p'):
                            PhaseBlock block = new PhaseBlock(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, (int)(TILE_WIDTH * ISO_SCALE), (int)(TILE_HEIGHT * ISO_SCALE), tileSet, boundsTexture, rand.Next(3), true);
                            //colliders.Add(block);

                            levelObjects[x, y] = block;
                            break;

                        case ('b'):
                            
                            Button button = new Button(x * TILE_WIDTH + TILE_WIDTH / 3, y * TILE_HEIGHT + TILE_HEIGHT / 3, TILE_WIDTH / 3, TILE_HEIGHT / 3, boundsTexture, roomFont, null);

                            addInfos.Add(new RoomAdditionalInfo(x, y, button));
                            levelObjects[x, y] = button;
                            colliders.Add(button);
                            colliders.Add(button.Box);
                            break;
                        case('B'):
                            Bottle bottl = new Bottle(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH / 2, TILE_HEIGHT / 2, boundsTexture, player);
                            colliders.Add(bottl);
                            levelObjects[x, y] = bottl;
                            break;

                    }
                    if (level[x, y] != ' ')
                    {
                        BackgroundTile back = new BackgroundTile((x * TILE_WIDTH) + 8, (y * TILE_HEIGHT) - 8, TILE_WIDTH, TILE_HEIGHT, backgroundSet, rand.Next(7));
                        background.Add(back);
                    }
                }
            }
            AddInformation();
        }

        private void AddInformation()
        {
            foreach (RoomAdditionalInfo addInf in addInfos)
            {
                if (addInf.entity is Button)
                {
                    List<string> temp = checkAdditionalInformation(addInf.x, addInf.y);
                    List<Point> tempPoint = new List<Point>();
                    int xNum, yNum;
                    if (temp != null)
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (i + 1 < temp.Count)
                            {
                                if (Int32.TryParse(temp[i], out xNum) && Int32.TryParse(temp[i + 1], out yNum))
                                {
                                    tempPoint.Add(new Point(xNum, yNum));
                                }
                            }
                        }
                    foreach (GameObject obj in getAttatchedObjects(tempPoint.ToArray()))
                    {
                        if (obj is IsClickableObject)
                            (addInf.entity as Button).AddObject((IsClickableObject)obj);
                    }
                }
            }
        }

        private List<GameObject> getAttatchedObjects(params Point[] locations)
        {

            List<GameObject> attatchedObjects = new List<GameObject>();
            foreach (Point p in locations)
            {
                if (p.X < 0 || p.X >= levelObjects.GetLength(0) || p.Y < 0 || p.Y >= levelObjects.GetLength(1))
                    continue;


                if (levelObjects[p.X, p.Y] == null)
                    continue;

                attatchedObjects.Add(levelObjects[p.X, p.Y]);
            }

            return attatchedObjects;
        }


        private List<string> checkAdditionalInformation(int x, int y)
        {

            List<String> lineSplits;
            lineSplits = new List<string>();
            foreach (string line in levelsInfo)
            {

                lineSplits.Clear();
                lineSplits.AddRange(line.Split(' '));

                if (lineSplits[0].Equals("" + x) && lineSplits[1].Equals("" + y))
                {
                    return lineSplits.GetRange(3, lineSplits.Count - 3);
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the room.
        /// </summary>
        public void Update(GameTime time)
        {
            if (foreground.Count > 0)
            {
                foreach (ForegroundTile tile in foreground)
                {
                    tile.Updates(time);
                }
            }

            if (enemies.Count > 0)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.Update(time);
                }
            }
        }

        /// <summary>
        /// Draws everything in the level.
        /// </summary>
        public void Draw(SpriteBatch batch)
        {

            if (background.Count > 0)
            {
                foreach (BackgroundTile tile in background)
                {
                    Vector2 drawLocation = manager.WorldToScreen(tile.X, tile.Y);
                    tile.Draw(batch, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }

            if (foreground.Count > 0)
            {
                foreach (ForegroundTile tile in foreground)
                {
                    Vector2 drawLocation = manager.WorldToScreen(tile.X, tile.Y);
                    tile.Draw(batch, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }

            if (enemies.Count > 0)
            {
                foreach (Enemy enemy in enemies)
                {
                    Vector2 drawLocation = manager.WorldToScreen(enemy.X, enemy.Y);
                    enemy.Draw(batch, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }

            if (colliders.Count > 0)
            {
                foreach (PhysicsEntity phys in colliders)
                {
                    Vector2 drawLocation = manager.WorldToScreen(phys.X, phys.Y);
                    phys.Draw(batch, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }
        }

        /// <summary>
        /// Show the physics bounding boxes for all tiles, for debugging.
        /// </summary>
        public void BoundsDraw(SpriteBatch batch)
        {
            if (foreground.Count > 0)
            {
                foreach (ForegroundTile tile in foreground)
                {
                    tile.DrawBounds(batch);
                }
            }

            if (enemies.Count > 0)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.DrawBounds(batch, manager.boundsTexture);
                }
            }
        }
    }
}


