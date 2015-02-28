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
                        //previous position and current position required? or do we not think clipping will happen
                        //i do fix position here
                        //i feel there needs to be a way for the movables to know they've collided, like an on collision method 

                    }
                }
            }
        }

        public CollisionManager(GameObject[] allGameObjects, params PhysicsEntity[] objectsToBeChecked)
        {
            objects = new List<GameObject>(allGameObjects);
            collisioCheckObjects = new List<PhysicsEntity>(objectsToBeChecked);
        }

    }
}
