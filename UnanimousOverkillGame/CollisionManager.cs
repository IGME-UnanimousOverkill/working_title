// Worked on by: Sean Coffey
//Worked on by: Jeannette Forbes
//Worked on by Gavin Keirstead

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UnanimousOverkillGame
{

    class CollisionManager
    {

        private List<PhysicsEntity> objects = null;//list of all objects in to check collisions against
        private List<PhysicsEntity> entities;//objects who collisions we care about

        private List<Collision> collisions;

        PhysicsEntity physEntity; //current physics object that we're checking for collisions
        PhysicsEntity gameObject; //current object we're checking against

        int count = 0; //debugging

        public List<PhysicsEntity> Objects
        {
            get { return objects; }
        }

        public CollisionManager(params PhysicsEntity[] objectsToBeChecked)
        {
            entities = new List<PhysicsEntity>(objectsToBeChecked);
            collisions = new List<Collision>();
        }

        public void UpdateObjects(List<PhysicsEntity> newObjects)
        {
            objects = newObjects;
        }

        public void ClearCollisions()
        {
            collisions.Clear();
        }

        public void DetectCollisions()
        {

            bool newCollision;
            if (objects != null)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    newCollision = true;

                    physEntity = entities[i];//if it hasn't move, it won't have new collisions, if somehting hits it, that entity will take care of collision
                    if (physEntity.X == physEntity.PrevX && physEntity.Y == physEntity.PrevY)
                    {
                        newCollision = false;
                    }
                    // Reset collision array
                    physEntity.colliderArray[0] = false;
                    physEntity.colliderArray[1] = false;
                    physEntity.colliderArray[2] = false;
                    physEntity.colliderArray[3] = false;

                    // Calculate the bottom and right side locations for the physEntity
                    float entBottom = physEntity.Y + physEntity.Rect.Height - 5;
                    float entRight = physEntity.X + physEntity.Rect.Width;

                    for (int j = 0; j < objects.Count; j++)
                    {
                        gameObject = objects[j];

                        // Calculate the bottom and right side locations for the gameObject
                        float objBottom = gameObject.Y + gameObject.Rect.Height;
                        float objRight = gameObject.X + gameObject.Rect.Width;

                        // Check distances between the sides of the objects.
                        float tDistance = objBottom - physEntity.Y;
                        float bDistance = entBottom - gameObject.Y;
                        float rDistance = entRight - gameObject.X;
                        float lDistance = objRight - physEntity.X;

                        // Whichever side is closest is the side they are colliding on.
                        if (physEntity.Rect.Intersects(gameObject.Rect))
                        {

                            gameObject.OnCollide(physEntity);
                            if (gameObject is Door)//stops nast problems from happening during room spawn, should  be changed in case of like locked doors or something
                                continue;
                            if (!gameObject.IsCollidable)
                                continue;
                            //below, sets collide array and creates new collision object
                            //TOP
                            if (tDistance < bDistance && tDistance < lDistance && tDistance < rDistance)
                            {
                                physEntity.colliderArray[0] = true;
                                if (physEntity.Y != physEntity.PrevY && newCollision)
                                collisions.Add(new Collision(physEntity,gameObject,CollisionSide.top));
                            }

                            //RIGHT
                            else if (rDistance < bDistance && rDistance < lDistance && rDistance < tDistance)
                            { 
                                physEntity.colliderArray[1] = true;
                                if (physEntity.X != physEntity.PrevX && newCollision)
                                collisions.Add(new Collision(physEntity, gameObject, CollisionSide.right)); 
                            }

                            //BOTTOM
                            else if (bDistance < tDistance && bDistance < lDistance && bDistance < rDistance)
                            {
                                physEntity.colliderArray[2] = true;
                                if (physEntity.Y != physEntity.PrevY && newCollision)
                                collisions.Add(new Collision(physEntity, gameObject, CollisionSide.bottom)); 
                            }

                            //LEFT
                            else if (lDistance < bDistance && lDistance < tDistance && lDistance < rDistance)
                            { 
                                physEntity.colliderArray[3] = true; 
                                if(physEntity.X != physEntity.PrevX && newCollision)
                                collisions.Add(new Collision(physEntity, gameObject, CollisionSide.left)); 
                            }
                        }
                    }
                }
            }
        }

        public void HandleCollisions()
        {
            /*
            for (int i = 0; i < entities.Count; i++)
            {
                physEntity = entities[i];
                //TOP
                if (physEntity.colliderArray[0]) { physEntity.Y = physEntity.Y + 5; }
                //RIGHT
                if (physEntity.colliderArray[1]) { physEntity.X = physEntity.X - 5; }
                //BOTTOM
                if (physEntity.colliderArray[2]) { physEntity.Y = physEntity.Y - 5; }
                //LEFT
                if (physEntity.colliderArray[3]) { physEntity.X = physEntity.X + 5; }
            }
             * */
            GameObject gObject;
            PhysicsEntity entity;
            for (int i = 0; i < collisions.Count; i++)//deals with all collisions 1 by 1, currently a little broken because always runs, even if still on floor..
            {
                gObject = collisions[i].GameObject;
                entity = collisions[i].Entity;
                if (collisions[i].collideArray[0])
                {
                    entity.Y = gObject.Y + gObject.Rect.Height -1;
                    if (entity.velocity.Y < 0)
                    {
                        entity.velocity.Y = 0;
                    }
                }
                if (collisions[i].collideArray[1])
                {
                    entity.X = gObject.X - entity.Rect.Width +1;
                    if (entity.velocity.X > 0)
                    {
                        entity.velocity.X = 0;
                    }
                }
                if (collisions[i].collideArray[2])
                {
                    entity.Y = gObject.Y - entity.Rect.Height +1;
                    entity.activateGravity = false;//dont want to keep falling lol
                    if (entity.velocity.Y > 0)
                    {
                        entity.velocity.Y = 0;
                    }
                }
                if (collisions[i].collideArray[3])
                {
                    entity.X = gObject.X + gObject.Rect.Width - 1;
                    if (entity.velocity.X < 0)
                    {
                        entity.velocity.X = 0;
                    }
                }
                gObject.positionChangedManually();
            }
            collisions = new List<Collision>();

        }
        public bool OnGround(PhysicsEntity p)
        {
            float pBottom = (p.Y + p.Rect.Height) + 5;
            for (int i = 0; i < objects.Count; i++)
            {

                if ((pBottom) >= objects[i].Y && (pBottom) < (objects[i].Rect.Height + objects[i].Y))
                    return true;
            }
            return false;
        }
    }
}
