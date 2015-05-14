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
        private int spawnTimer;

        private int spawnX;
        private int spawnY;
        private int spawnWidth;
        private int spawnHeight;
        private float spawnScale;
        private Texture2D spawnTexture;
        private Texture2D spawnNormal;

        public EntitySpawner(int x, int y, int width, int height,int objectWidth, Texture2D texture = null, Texture2D normal = null) :
            base(x,y,width,height,texture,normal)
        {

            spawnTimer = 0;
            canSpawn = false;

            children = new List<PhysicsEntity>();
        }

        public override void Updates(GameTime gameTime)
        {
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

        public void removeChild(PhysicsEntity child)
        {
            if (children.Contains(child))
            {
                children.Remove(child);
            }
        }

        public void Spawn()
        {
            switch (type)
            {
                case EntityType.ooze:
                   // children.Add(new Ooze(int X, int y,))
                    break;
                case EntityType.bottle:
                    break;
                default:
                    break;
            }
        }

        private void SetType(String type)
        {
            if (EntityType.bottle.ToString().Equals(type))
            {
                this.type = EntityType.bottle;
            }

            if (EntityType.ooze.ToString().Equals(type))
            {
                this.type = EntityType.ooze;
            }

        }

        public override void AddInformation(List<string> infoLines, GameObject[,] objects)
        {
            base.AddInformation(infoLines, objects);

            SetType(infoLines[0]);

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
                    spawnX = X;
                    spawnY = Y - spawnHeight;
                    break;
            }

            if (!Int32.TryParse(infoLines[2], out maxSpawns))
            {
                maxSpawns = 1;
            }

            if (!Int32.TryParse(infoLines[3], out spawnTime))
            {
                spawnTime = 5000;
            }

            if (type == EntityType.ooze)
            {
                spawnScale = .5f;
                texture = RoomManager.GetRoomManager.Current.OozeTexture;
            }

            if (type == EntityType.bottle)
            {
                spawnHeight = 50;
                spawnWidth = 50;
                texture = RoomManager.GetRoomManager.Current.BottleTexture;
            }

            canSpawn = true;
            Spawn();
        }
    }
}
