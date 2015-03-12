﻿using System;
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

        private List<PhysicsEntity> objects;//list of all objects in to check collisions against
        private List<PhysicsEntity> entities;//objects who collisions we care about

        PhysicsEntity physEntity; //current physics object that we're checking for collisions
        PhysicsEntity gameObject; //current object we're checking against

        int count = 0; //debugging

        public void DetectCollisions()
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


        /*
        public void CheckCollisions()
        {

            PhysicsEntity tempPhys;//current physics object being used
            GameObject tempGame;//current game object being used
            int interceptPoint = 0;//place where objects intercept, is either an x or a y depending on the situation
            double slope = 0;//physics objects movement
            int i = 0, j = 0;//physics objects height modifier and width modifier respectivelu
            int objHeightMod = 0, objWidthMod = 0;//same as above, but game object

            bool breaker = false;//helps to break from nested loop later on
            for (int col = 0; col < collisioCheckObjects.Count; col++)//iterates through important objects
            {

                tempPhys = collisioCheckObjects.ElementAt(col);

                if (tempPhys.PrevY == tempPhys.Y && tempPhys.PrevX == tempPhys.X)//this object hasn't moved, so we don't care
                {
                    continue;
                }

                for (int obj = 0; obj < objects.Count; obj++)//iterates through list of all objects
                {
                    //if (obj == 22)
                     //   continue;
                    tempGame = objects.ElementAt(obj);

                    objWidthMod = (tempPhys.X > tempPhys.PrevX) ? 0 : 1;//if the physics object is moving forward, mod = 0 because only right side of other entity needs check
                    objHeightMod = (tempPhys.Y > tempPhys.PrevY) ? 0 : 1;//if physics object is moving  down, mod = 0 because only top side of other entity needs check
                    i = 1 - objHeightMod;//height mod of physics entity
                    j = 1 - objWidthMod;//width mod of physics object

                    if(tempPhys.X - tempPhys.PrevX==0) {//if this is a vertical line
                        if (tempPhys.X >= tempGame.X && tempPhys.X <= tempGame.X + tempGame.Rect.Width && (interceptPoint = tempGame.Y + (tempGame.Rect.Height * objHeightMod)) <= Math.Max(tempPhys.X + (tempPhys.Rect.Height * i), tempPhys.PrevY + (tempPhys.Rect.Height * i)) && interceptPoint >= Math.Min(tempPhys.X + (tempPhys.Rect.Height * i), tempPhys.PrevY + (tempPhys.Rect.Height * i)))
                        {
                            tempPhys.Y = interceptPoint - (tempPhys.Rect.Height * i) + ((i == 1) ? -1 : 1);
                            //let it know it hit a wall, 

                        }
                        else if (tempPhys.X + tempPhys.Rect.Width >= tempGame.X && tempPhys.X + tempPhys.Rect.Width <= tempGame.X + tempGame.Rect.Width && (interceptPoint = tempGame.Y + (tempGame.Rect.Height * objHeightMod)) <= Math.Max(tempPhys.X + (tempPhys.Rect.Height * i), tempPhys.PrevY + (tempPhys.Rect.Height * i)) && interceptPoint >= Math.Min(tempPhys.X + (tempPhys.Rect.Height * i), tempPhys.PrevY + (tempPhys.Rect.Height * i)))
                        {
                            tempPhys.Y = interceptPoint - (tempPhys.Rect.Height * i) + ((i == 1) ? -1 : 1);
                            //let it know it hit a floor or ceiling

                        }
                        continue;
                    }

                    slope = FindSlope(tempPhys.PrevX, tempPhys.X, tempPhys.PrevY, tempPhys.Y);

                    if (slope == 0)
                    {
                        if (tempPhys.Y >= tempGame.Y && tempPhys.Y <= tempGame.Y+tempGame.Rect.Height && (interceptPoint = tempGame.X + (tempGame.Rect.Width * objWidthMod)) <= Math.Max(tempPhys.X + (tempPhys.Rect.Width * j), tempPhys.PrevX + (tempPhys.Rect.Width * j)) && interceptPoint >= Math.Min(tempPhys.X + (tempPhys.Rect.Width * j), tempPhys.PrevX + (tempPhys.Rect.Width * j)))
                        {
                            tempPhys.X = interceptPoint - (tempPhys.Rect.Width * j) + ((j==1) ? -1:1);
                            tempPhys.positionChangedManually();
                            //let it know it hit a wall, 
                            
                        }
                        else if (tempPhys.Y + tempPhys.Rect.Height >= tempGame.Y && tempPhys.Y + tempPhys.Rect.Height <= tempGame.Y+tempGame.Rect.Height && (interceptPoint = tempGame.X + (tempGame.Rect.Width * objWidthMod)) <= Math.Max(tempPhys.X + (tempPhys.Rect.Width * j), tempPhys.PrevX + (tempPhys.Rect.Width * j)) && interceptPoint >= Math.Min(tempPhys.X + (tempPhys.Rect.Width * j), tempPhys.PrevX + (tempPhys.Rect.Width * j)))
                        {
                            tempPhys.X = interceptPoint - (tempPhys.Rect.Width * j) + ((j == 1) ? -1 : 1);
                            tempPhys.positionChangedManually();//may not be necessary
                            //let it know it hit a floor or ceiling
                            
                        }
                        continue;
                    }



                    for (i = 1; i >= 0; i--)
                    {
                        for (j = 1; j >= 0; j--)
                        {
                            if ((interceptPoint = FindY(tempGame.X + tempGame.Rect.Width * objWidthMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.Y && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                            {
                                tempPhys.X = (tempGame.X + tempGame.Rect.Width * objWidthMod) - (tempPhys.Rect.Width * j) + ((j == 1) ? -1 : 1);
                                tempPhys.Y = interceptPoint - (tempPhys.Rect.Height * i) + ((i == 1) ? -1 : 1);
                                //let it know it hit a wall, 
                                breaker = true;
                            }
                            else if ((interceptPoint = FindX(tempGame.Y + tempGame.Rect.Height * objHeightMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.X && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                            {
                                tempPhys.X = interceptPoint + (tempGame.Rect.Height * objHeightMod) - (tempPhys.Rect.Width * j) + ((j == 1) ? -1 : 1);
                                tempPhys.Y = tempGame.Y - (tempPhys.Rect.Height * i) + ((i == 1) ? -1 : 1);
                                //let it know it hit a floor or ceiling
                                breaker = true;
                            }
                            if (breaker)
                                break;
                        }
                        if (breaker)
                            break;
                    }
                    breaker = false;

                }
            }
        }




        private double FindB(int y, int x, double m)
        {
            return y - (m) * x;
        }

        private double FindSlope(int x1, int x2, int y1, int y2)
        {

            if (x2 - x1 == 0)
            {
                throw new DivideByZeroException("vertical line");
            }
            return (y2 - y1) / (x2 - x1);
        }
        private int FindX(int y, double m, double b)
        {
            return (int)((y - b) / m);
        }

        private int FindY(int x, double m, double b)
        {
            return (int)(((m * x) + b));
        }
        
        */
        public CollisionManager(PhysicsEntity[] allEntities, params PhysicsEntity[] objectsToBeChecked)
        {
            objects = new List<PhysicsEntity>(allEntities);
            entities = new List<PhysicsEntity>(objectsToBeChecked);
        }



    }
}
