using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnanimousOverkillGame
{
    class CollisionManager
    {
        private List<GameObject> objects;
        private List<PhysicsEntity> collisioCheckObjects;






        public void CheckCollisions()
        {
            for (int col = 0; col < collisioCheckObjects.Count; col++)
            {
                for (int obj = 0; obj < objects.Count; obj++)
                {
                    if (objects.ElementAt(obj) != collisioCheckObjects.ElementAt(col) && collisioCheckObjects.ElementAt(col).Rect.Intersects(objects.ElementAt(obj).Rect))
                    {
                        
                    }
                }
            }
        }

        private double findIntercept(int x1, int y1, int x2, int y2)
        {
            return y1-(((y2-y1)/(x2-x1))*x1);
        }



        public CollisionManager(GameObject[] allGameObjects, params PhysicsEntity[] objectsToBeChecked)
        {
            objects = new List<GameObject>(allGameObjects);
            collisioCheckObjects = new List<PhysicsEntity>(objectsToBeChecked);
        }

    }
}
