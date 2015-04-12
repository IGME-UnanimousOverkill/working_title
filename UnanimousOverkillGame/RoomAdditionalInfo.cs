using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnanimousOverkillGame
{
    class RoomAdditionalInfo
    {
        public PhysicsEntity entity;
        public int x;
        public int y;

        public RoomAdditionalInfo(int x, int y, PhysicsEntity entity)
        {
            this.x = x;
            this.y = y;
            this.entity = entity;
        }

    }
}
