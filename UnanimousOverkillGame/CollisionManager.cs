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

        PhysicsEntity physEntity; //current physics object that we're checking for collisions
        PhysicsEntity gameObject; //current object we're checking against

        int count = 0; //debugging

        public CollisionManager(params PhysicsEntity[] objectsToBeChecked)
        {
            entities = new List<PhysicsEntity>(objectsToBeChecked);
        }

        public void UpdateObjects(List<PhysicsEntity> newObjects)
        {
            objects = newObjects;
        }

        public void DetectCollisions()
        {
            if (objects != null)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    physEntity = entities[i];
                    // Reset collision array
                    physEntity.colliderArray[0] = false;
                    physEntity.colliderArray[1] = false;
                    physEntity.colliderArray[2] = false;
                    physEntity.colliderArray[3] = false;

                    // Calculate the bottom and right side locations for the physEntity
                    float entBottom = physEntity.Y + physEntity.Rect.Height;
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

                            //TOP
                            if (tDistance < bDistance && tDistance < lDistance && tDistance < rDistance)
                            { physEntity.colliderArray[0] = true; }

                            //RIGHT
                            else if (rDistance < bDistance && rDistance < lDistance && rDistance < tDistance)
                            { physEntity.colliderArray[1] = true; }

                            //BOTTOM
                            else if (bDistance < tDistance && bDistance < lDistance && bDistance < rDistance)
                            { physEntity.colliderArray[2] = true; }

                            //LEFT
                            else if (lDistance < bDistance && lDistance < tDistance && lDistance < rDistance)
                            { physEntity.colliderArray[3] = true; }
                        }
                    }
                }
            }
        }

        public void HandleCollisions()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                physEntity = entities[i];
                //TOP
                if (physEntity.colliderArray[0]) { physEntity.Y = physEntity.Y + 5;}
                //RIGHT
                if (physEntity.colliderArray[1]) { physEntity.X = physEntity.X - 5;}
                //BOTTOM
                if (physEntity.colliderArray[2]) { physEntity.Y = physEntity.Y - 5;}
                //LEFT
                if (physEntity.colliderArray[3]) { physEntity.X = physEntity.X + 5;}
            }
        }
    }
}
