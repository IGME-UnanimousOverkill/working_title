using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnanimousOverkillGame
{
    public enum CollisionSide { top =  0, right = 1, bottom = 2, left = 3}//this is just sort of handy, doesnt have to be used
    class Collision
    {

        private PhysicsEntity entity;
        private GameObject gameObject;

        public PhysicsEntity Entity { get { return entity; } }
        public GameObject GameObject { get { return gameObject; } }

        public bool[] collideArray;

        public Collision(PhysicsEntity entity, GameObject gameObject, CollisionSide side)
        {
            this.entity = entity;
            this.gameObject = gameObject;
            collideArray = new bool[4];
            collideArray[(int)side] = true;
        }


    }
}
