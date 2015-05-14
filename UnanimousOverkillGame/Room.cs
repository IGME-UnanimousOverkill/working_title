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
        private List<PhysicsEntity> enemies;
        private List<GameObject> drawable;
        private List<Door> doors;
        private Room previousRoom;
        private List<Room> nextRooms;
        // This will be replaced with tile sets.
        private Texture2D tileSet;
        private Texture2D backgroundSet;
        private Texture2D boundsTexture;
        private Texture2D doorTexture;
        private Texture2D bottleTexture;
        private Texture2D hopEnemyTexture;
        private Texture2D oozeTexture;
        private Texture2D spikeTexture;
        private RoomManager manager;
        private static Random rand = new Random();

        private List<PhysicsEntity> enemiesToAdd;


        public Texture2D OozeTexture { get { return oozeTexture; } }
        public Texture2D BottleTexture { get { return bottleTexture; } }
        public RoomManager Manager { get { return manager; } }


        public List<PhysicsEntity> EnemiesToAdd { get { return enemiesToAdd; } }
        public List<PhysicsEntity> Colliders { get { return colliders; } set { colliders= value; } }
        public List<PhysicsEntity> Enemies { get { return enemies; } set { enemies = value; } }
        public List<GameObject> Drawable { get { return drawable; } set { drawable = value; } }
        int prevSize;
        int prevSize2;
        List<string> levelsInfo;
        private SpriteFont roomFont;
        private Dictionary<Vector2, IsClickableObject> clickables;
        private List<IsClickableObject> tempClickables;

        public int depth;

        public const int TILE_WIDTH = 50;
        public const int TILE_HEIGHT = 50;

        /// <summary>
        /// Default constructor for RoomManager
        /// </summary>
        public Room(RoomManager roomManager, SpriteFont roomFont, int num)
        {
            this.roomFont = roomFont;
            manager = roomManager;
            foreground = new List<ForegroundTile>();
            background = new List<BackgroundTile>();
            colliders = new List<PhysicsEntity>();
            tempClickables = new List<IsClickableObject>();
            drawable = new List<GameObject>();
            enemies = new List<PhysicsEntity>();
            nextRooms = new List<Room>();
            doors = new List<Door>();
            enemiesToAdd = new List<PhysicsEntity>();
            ID = manager.MakeID();
            depth = num;
        }

        

        public Room(RoomManager roomManager, Room previous, SpriteFont roomFont, int num)
        {
            this.roomFont = roomFont;
            manager = roomManager;
            previousRoom = previous;
            foreground = new List<ForegroundTile>();
            background = new List<BackgroundTile>();
            colliders = new List<PhysicsEntity>();
            tempClickables = new List<IsClickableObject>();
            drawable = new List<GameObject>();
            enemies = new List<PhysicsEntity>();
            nextRooms = new List<Room>();
            doors = new List<Door>();
            enemiesToAdd = new List<PhysicsEntity>();
            ID = manager.MakeID();
            depth = num;
        }

        /// <summary>
        /// Sets the tile texture. This will be editted to include texture packs.
        /// </summary>
        public void SetTileTexture(Texture2D tileTexture, Texture2D bounds, Texture2D backBounds, Texture2D doorTexture, Texture2D bottleTexture, Texture2D hopEnemyTexture, Texture2D oozeTexture, Texture2D spikeTexture)
        {
            tileSet = tileTexture;
            boundsTexture = bounds;
            backgroundSet = backBounds;
            this.doorTexture = doorTexture;
            this.bottleTexture = bottleTexture;
            this.hopEnemyTexture = hopEnemyTexture;
            this.oozeTexture = oozeTexture;
            this.spikeTexture = spikeTexture;
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
                        player.X = door.X + TILE_WIDTH + 4;
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
                            int tileNum = rand.Next(7);
                            Texture2D normal = manager.content.Load<Texture2D>("Normals/frontNormals/FrontNormalMap_0" + (tileNum + 1) + ".png");
                            ForegroundTile tile = new ForegroundTile(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, (int)(TILE_WIDTH * ISO_SCALE), (int)(TILE_HEIGHT * ISO_SCALE), tileSet, boundsTexture, normal, tileNum);
                            tile.PositionLocked = true;
                            if (Game1.RumbleMode)
                            {
                                tile.PositionLocked = false;
                                int percent = rand.Next(100);
                                if (percent > 100 - depth)
                                {
                                    tile.rumble = true;
                                    enemies.Add(tile);
                                    tile.color = Color.Gray;
                                    percent = rand.Next(100);
                                    if (percent > 110 - depth)
                                    {
                                        tile.falling = true;
                                    }
                                }
                            }
                            foreground.Add(tile);
                            colliders.Add(tile);
                            levelObjects[x, y] = tile;
                            drawable.Add(tile);
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
                                Door nextDoor = new Door(x * TILE_WIDTH + 5, y * TILE_HEIGHT - 6, TILE_WIDTH, TILE_HEIGHT, doorTexture, manager.content.Load<Texture2D>("Normals/BlankNormal.png"), room, manager);
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
                                    player.X = ((x + 1) * TILE_WIDTH) + 4;
                                    player.Y = ((y + 1) * TILE_HEIGHT) - player.Rect.Height;//made this a little more generic
                                    player.positionChangedManually();
                                    player.velocity = Vector2.Zero;
                                }
                                Door previousDoor = new Door(x * TILE_WIDTH +3, y * TILE_HEIGHT -6, TILE_WIDTH, TILE_HEIGHT, doorTexture, manager.content.Load<Texture2D>("Normals/BlankNormal.png"), previousRoom, manager);
                                colliders.Add(previousDoor);
                                doors.Add(previousDoor);

                                levelObjects[x, y] = previousDoor;
                            }
                            break;
                        case ('┴'):
                            Fan fan = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan);
                            colliders.AddRange(fan.getEffects());
                            enemies.AddRange(fan.getEffects());
                            drawable.Add(fan);
                            levelObjects[x, y] = fan;
                            break;
                        case ('┤'):
                            Fan fan1 = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan1);
                            colliders.AddRange(fan1.getEffects());
                            enemies.AddRange(fan1.getEffects());
                            drawable.Add(fan1);
                            levelObjects[x, y] = fan1;
                            break;

                        case ('┬'):
                            Fan fan2 = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan2);
                            colliders.AddRange(fan2.getEffects());
                            enemies.AddRange(fan2.getEffects());
                            drawable.Add(fan2);
                            levelObjects[x, y] = fan2;
                            break;

                        case ('├'):
                            Fan fan3 = new Fan(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, level[x, y]);
                            colliders.Add(fan3);
                            colliders.AddRange(fan3.getEffects());
                            enemies.AddRange(fan3.getEffects());
                            drawable.Add(fan3);
                            levelObjects[x, y] = fan3;
                            break;

                        case ('h'):
                            HoppingEnemy hopEnemy = new HoppingEnemy(x * TILE_WIDTH, y * TILE_HEIGHT, 0.6f, hopEnemyTexture, manager.content.Load<Texture2D>("Normals/BlankNormal.png"), player);
                            colliders.Add(hopEnemy);
                            enemies.Add(hopEnemy);
                            drawable.Add(hopEnemy);
                            levelObjects[x, y] = hopEnemy;
                            break;

                        case ('o'):
                            Ooze ooze = new Ooze(x * TILE_WIDTH, y * TILE_HEIGHT, .50f, oozeTexture, manager.content.Load<Texture2D>("Normals/BlankNormal.png"), player);
                            colliders.Add(ooze);
                            enemies.Add(ooze);
                            drawable.Add(ooze);
                            levelObjects[x, y] = ooze;
                            break;

                        case ('p'):
                            int phaseTileNum = rand.Next(7);
                            Texture2D phaseNormal = manager.content.Load<Texture2D>("Normals/frontNormals/FrontNormalMap_0" + (phaseTileNum + 1) + ".png");
                            PhaseBlock block = new PhaseBlock(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, (int)(TILE_WIDTH * ISO_SCALE), (int)(TILE_HEIGHT * ISO_SCALE), tileSet, boundsTexture, phaseNormal, phaseTileNum, true);
                            colliders.Add(block);
                            drawable.Add(block);
                            levelObjects[x, y] = block;
                            break;

                        case ('b'):
                            
                            Button button = new Button(x * TILE_WIDTH + TILE_WIDTH / 3, y * TILE_HEIGHT + TILE_HEIGHT / 3, TILE_WIDTH / 3, TILE_HEIGHT / 3, boundsTexture, roomFont, null);
                            levelObjects[x, y] = button;
                            colliders.Add(button);
                            colliders.Add(button.Box);
                            enemies.Add(button.Box);
                            drawable.Add(button);
                            break;

                        case ('T'):
                            Button button1 = new Button(x * TILE_WIDTH + TILE_WIDTH / 3, y * TILE_HEIGHT + TILE_HEIGHT / 3, TILE_WIDTH / 3, TILE_HEIGHT / 3, boundsTexture, roomFont, null,true);
                            levelObjects[x, y] = button1;
                            colliders.Add(button1);
                            colliders.Add(button1.Box);
                            enemies.Add(button1.Box);
                            drawable.Add(button1);
                            break;

                        case ('B'):
                            Bottle bottle = new Bottle(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, bottleTexture, player, manager);
                            colliders.Add(bottle);
                            drawable.Add(bottle);
                            levelObjects[x, y] = bottle;
                            break;

                        case ('s'):
                            bool onGround = false;
                            try
                            {
                                if (levelObjects[x, y + 1] == null)
                                    onGround = false;
                                else
                                    onGround = true;
                            }
                            catch (Exception e)
                            { }
                            Spikes spikes = new Spikes(x * TILE_WIDTH, y * TILE_HEIGHT, spikeTexture, manager.content.Load<Texture2D>("Normals/BlankNormal.png"), player, onGround);
                            colliders.Add(spikes);
                            enemies.Add(spikes);
                            drawable.Add(spikes);
                            levelObjects[x, y] = spikes;
                            break;
                        case('S'):
                            EntitySpawner spawner = new EntitySpawner(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT, this);
                            colliders.Add(spawner);
                            enemies.Add(spawner);
                            drawable.Add(spawner);
                            levelObjects[x, y] = spawner;
                            break;
                    }
                    if (level[x, y] != ' ')
                    {
                        int tileNum = rand.Next(7);
                        Texture2D normal = manager.content.Load<Texture2D>("Normals/backgroundNormals/BackNormalMap_0" + (tileNum + 1) + ".png");
                        BackgroundTile back = new BackgroundTile((x * TILE_WIDTH) + 8, (y * TILE_HEIGHT) - 8, TILE_WIDTH, TILE_HEIGHT, backgroundSet, normal, tileNum);
                        background.Add(back);
                    }
                }
            }
            checkAdditionalInformation();
            prevSize = colliders.Count;
            prevSize2 = enemies.Count;
            drawable.Add(player);
        }   
   
        public void RespawnRoom()
        {
            colliders.Clear();
            drawable.Clear();
            enemies.Clear();
            foreground.Clear();
            doors.Clear();
            background.Clear();
            manager.player.Health = 100;
            manager.player.bottlesOnHand /=2;
            manager.player.Intox = 0;

            SpawnRoom(manager.player, previousRoom);
        }

        private void checkAdditionalInformation()
        {
            List<String> lineSplits;
            lineSplits = new List<string>();
            int xNum,yNum;
            foreach (string line in levelsInfo)
            {

                lineSplits.Clear();
                lineSplits.AddRange(line.Split(' '));

                if (Int32.TryParse(lineSplits[0], out xNum) && Int32.TryParse(lineSplits[1], out yNum))
                {
                    levelObjects[xNum, yNum].AddInformation(lineSplits.GetRange(3, lineSplits.Count - 3), levelObjects);
                }
            }
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
                foreach (PhysicsEntity enemy in enemies)
                {
                    if (enemy is Enemy)
                        (enemy as Enemy).Update(time);
                    else
                        enemy.Updates(time);
                }
            }

            if (enemiesToAdd != null)
            {
                enemies.AddRange(enemiesToAdd);
                enemiesToAdd.Clear();
            }
            if(prevSize != colliders.Count)
            {
                manager.UpdateCollisionManager(this);
                prevSize = colliders.Count;
            }
            if(prevSize2 != enemies.Count)
            {
                manager.UpdateCollisionManager(this);
                prevSize2 = enemies.Count;
            }
        }

        /// <summary>
        /// Draws everything in the level.
        /// </summary>
        public void Draw(GraphicsDevice device, SpriteBatch batch)
        {
            if (background.Count > 0)
            {
                foreach (BackgroundTile tile in background)
                {
                    Vector2 drawLocation = manager.WorldToScreen(tile.X, tile.Y);
                    tile.Draw(device, batch, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }
            if (doors.Count > 0)
            {
                foreach (Door door in doors)
                {
                    Vector2 drawLocation = manager.WorldToScreen(door.X, door.Y);
                    door.Draw(device, batch, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }

            if (drawable.Count > 0)
            {
                foreach (GameObject thing in drawable)
                {
                    Vector2 drawLocation = manager.WorldToScreen(thing.X, thing.Y);
                    thing.Draw(device, batch, (int)drawLocation.X, (int)drawLocation.Y);
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
                    Vector2 drawLocation = manager.WorldToMinimap((int)(tile.X * RoomManager.MINIMAP_SCALE), (int)(tile.Y * RoomManager.MINIMAP_SCALE));
                    tile.DrawBounds(batch, manager.boundsTexture, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }

            if (enemies.Count > 0)
            {
                foreach (PhysicsEntity enemy in enemies)
                {
                    Vector2 drawLocation = manager.WorldToMinimap((int)(enemy.X * RoomManager.MINIMAP_SCALE), (int)(enemy.Y * RoomManager.MINIMAP_SCALE));
                    enemy.DrawBounds(batch, manager.boundsTexture, (int)drawLocation.X, (int)drawLocation.Y);
                }
            }
        }
    }
}


