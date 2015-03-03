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

        private readonly double verticalSlope = -100000;
        private List<GameObject> objects;
        private List<PhysicsEntity> collisioCheckObjects;
        





        public void CheckCollisions()
        {
            PhysicsEntity tempPhys;
            GameObject tempGame;
            int interceptPoint = 0;
            double slope = 0;
            int i = 0, j = 0;
            int objHeightMod = 0, objWidthMod = 0;

            bool breaker = false;
            for (int col = 0; col < collisioCheckObjects.Count; col++)
            {
                tempPhys = collisioCheckObjects.ElementAt(col);
                for (int obj = 0; obj < objects.Count; obj++)
                {
                    

                    slope = FindSlope(tempPhys.PrevX, tempPhys.X, tempPhys.PrevY, tempPhys.Y);

                    tempGame = objects.ElementAt(obj);
                    //if  the object was already edited in one direction, change shit here?
                    //I can see clipping being a huge problem the way i'm doing it

                    objWidthMod = (tempPhys.X - tempPhys.PrevX >= 0) ? 0 : 1;
                    objHeightMod = (tempPhys.Y - tempPhys.PrevY >= 0) ? 0 : 1;

                    i = -(0 - objHeightMod);
                    j = -(0 - objWidthMod);

                    if (slope == 0)
                    {
                        if ((interceptPoint = FindX(tempGame.Y + tempGame.Rect.Height * objHeightMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.X && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                        {
                            tempPhys.X = interceptPoint + (tempGame.Rect.Height * objHeightMod) - (tempPhys.Rect.Width * j);
                            //let it know it hit a wall, 
                            breaker = true;
                        }
                        else if ((interceptPoint = FindX(tempGame.Y + tempGame.Rect.Height * objHeightMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.X && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                        {
                            tempPhys.X = interceptPoint + (tempGame.Rect.Height * objHeightMod) - (tempPhys.Rect.Width * j);
                            //let it know it hit a floor or ceiling
                            breaker = true;
                        }
                        continue;
                    }
                    if (slope == verticalSlope)
                    {
                        if ((interceptPoint = FindY(tempGame.X + tempGame.Rect.Width * objWidthMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.Y && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                        {
                            tempPhys.Y = interceptPoint - (tempPhys.Rect.Height * i);
                            //let it know it hit a wall, 
                            breaker = true;
                        }
                        else if ((interceptPoint = FindY(tempGame.X + tempGame.Rect.Width * objWidthMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.Y && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                        {
                            tempPhys.Y = interceptPoint - (tempPhys.Rect.Height * i);
                            //let it know it hit a floor or ceiling
                            breaker = true;
                        }
                        continue;
                    }



                    for (i = 1; i >= 0; i--)
                    {
                        for (j = 1; j >= 0; j--)
                        {
                            if ((interceptPoint = FindY(tempGame.X + tempGame.Rect.Width * objWidthMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.Y && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                            {
                                tempPhys.X = (tempGame.X + tempGame.Rect.Width * objWidthMod) - (tempPhys.Rect.Width * j);
                                tempPhys.Y = interceptPoint - (tempPhys.Rect.Height * i);
                                //let it know it hit a wall, 
                                breaker = true;
                            }
                            else if ((interceptPoint = FindX(tempGame.Y + tempGame.Rect.Height * objHeightMod, slope, FindB(tempPhys.Y + tempPhys.Rect.Height * i, tempPhys.X + tempPhys.Rect.Width * j, slope))) >= tempGame.X && interceptPoint <= tempGame.Y + tempGame.Rect.Height)
                            {
                                tempPhys.X = interceptPoint + (tempGame.Rect.Height * objHeightMod) - (tempPhys.Rect.Width * j);
                                tempPhys.Y = tempGame.Y - (tempPhys.Rect.Height * i);
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
            return (x2 - x1 == 0) ? verticalSlope : (y2 - y1) / (x2 - x1);
        }
        private int FindX(int y, double m, double b)
        {
            return (int)((y - b) / m);
        }

        private int FindY(int x, double m, double b)
        {
            return (int)(((m * x) + b));
        }

        public CollisionManager(GameObject[] allGameObjects, params PhysicsEntity[] objectsToBeChecked)
        {
            objects = new List<GameObject>(allGameObjects);
            collisioCheckObjects = new List<PhysicsEntity>(objectsToBeChecked);
        }

    }
}
