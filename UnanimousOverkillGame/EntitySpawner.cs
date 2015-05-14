using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace UnanimousOverkillGame
{
    enum EntityType{ooze, bottle}
    class EntitySpawner : PhysicsEntity
    {

        private List<PhysicsEntity> children;

        private EntityType type;
        private int maxSpawns;
        private int spawnTime;
        private bool canSpawn;
        private bool infinitePickups;
        private int spawnTimer;

        private int spawnX;
        private int spawnY;
        private int spawnWidth;
        private int spawnHeight;
        private float spawnScale;
        private Texture2D spawnTexture;
        private Texture2D spawnNormal;
        private Room room;

        public EntitySpawner(int x, int y, int width, int height, Room room,Texture2D texture = null, Texture2D normal = null) :
            base(x,y,width,height,texture,normal)
        {
            this.room = room;
            spawnTimer = 0;
            canSpawn = false;
            isCollidable = false;
            children = new List<PhysicsEntity>();
        }

        public override void Updates(GameTime gameTime)
        {
            if (children.Count == 0 && type == EntityType.bottle && room.Manager.player.bottlesOnHand == 0)
            {
                canSpawn = true;
            }
            base.Updates(gameTime);
            if (canSpawn)
            {
                spawnTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (spawnTimer >= spawnTime)
                {
                    Spawn();
                    if (canSpawn)
                    {
                        spawnTime -= spawnTime;
                    }
                    else
                    {
                        spawnTimer = 0;
                    }
                }
            }
        }

        public void RemoveChild(PhysicsEntity child)
        {
            if (children.Contains(child))
            {
                children.Remove(child);
            }
            if (children.Count < maxSpawns && (type != EntityType.bottle || infinitePickups))
            {
                canSpawn = true;
            }
        }

        public void Spawn()
        {

            if (!canSpawn)
                return;

            int i = children.Count;

            switch (type)
            {
                case EntityType.ooze:
                    children.Add(new Ooze(spawnX, spawnY, spawnScale, spawnTexture, spawnNormal, room.Manager.player));
                    room.EnemiesToAdd.Add(children[i]);
                    break;
                case EntityType.bottle:
                    children.Add(new Bottle(spawnX, spawnY, spawnWidth, spawnHeight, spawnTexture, room.Manager.player, room.Manager));

                    break;
                default:
                    break;


            }
            room.Colliders.Add(children[i]);

            room.Drawable.Add(children[i]);
            children[i].SetSpawner(this);
         //   room.Manager.UpdateCollisionManager(room);

            if (i + 1 >= maxSpawns)
            {
                canSpawn = false;
            }
        }

        private void SetType(String type)
        {
            if (EntityType.bottle.ToString().Equals(type))
            {
                this.type = EntityType.bottle;
                return;
            }

            if (EntityType.ooze.ToString().Equals(type))
            {
                this.type = EntityType.ooze;
                return;
            }

            this.type = EntityType.bottle;

        }

        public override void AddInformation(List<string> infoLines, GameObject[,] objects)
        {
            infinitePickups = false;
            base.AddInformation(infoLines, objects);
            if (infoLines == null || infoLines.Count == 0)
            {
                return;
            }


            SetType(infoLines[0]);
            int place = 2;

            if (infoLines.Count > 1)
            {
                switch (infoLines[1])
                {

                    case "DOWN":
                        spawnX = X;
                        spawnY = Y + rectangle.Height;
                        break;
                    case "LEFT":
                        spawnX = X - spawnWidth;
                        spawnY = Y;
                        break;
                    case "RIGHT":
                        spawnX = X + rectangle.Width;
                        spawnY = Y;
                        break;
                    default://default case is UP
                        place = 1;
                        spawnX = X;
                        spawnY = Y - spawnHeight;
                        break;
                }
            }
            if (infoLines.Count > place && !Int32.TryParse(infoLines[place], out maxSpawns))
            {
                maxSpawns = 1;
                place--;
            }
            place++;
            if (infoLines.Count > place && !Int32.TryParse(infoLines[place], out spawnTime))
            {
                spawnTime = 5000;
                place--;
            }
            place++;
            if (type == EntityType.ooze)
            {
                spawnScale = .5f;
                spawnTexture = RoomManager.GetRoomManager.Current.OozeTexture;
            }

            if (type == EntityType.bottle)
            {
                int x = 0 ;
                if (infoLines.Count > place && !Int32.TryParse(infoLines[place], out x))
                {
                    x = 0;
                }

                infinitePickups = (x == 1) ? true : false;
                spawnHeight = 50;
                spawnWidth = 50;
                spawnTexture = RoomManager.GetRoomManager.Current.BottleTexture;
            }

            spawnNormal = room.Manager.content.Load<Texture2D>("Normals/BlankNormal.png");

            canSpawn = true;
            Spawn();
        }
    }
}
