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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace UnanimousOverkillGame
{
    class EffectBox : PhysicsEntity
    {
        Vector2 accelerationChange;
        private int originalX;
        private int originalY;
        private PhysicsEntity parent;

        public bool HasParent { get { return parent != null; } }
        public float XAccelerationChange { get { return accelerationChange.X; } }

        public float YAccelerationChange { get { return accelerationChange.Y; } }

        public override void OnCollide(PhysicsEntity other)
        {
            other.AddForce(accelerationChange);
            if (HasParent)
            {
                
                parent.OnCollide(other);
            }
        }


        public EffectBox(int x, int y, int width, int height,Vector2 accelerationChange, Texture2D texture = null) : base(x,y,width,height,texture,false)
        {
            this.accelerationChange = accelerationChange;
            originalX = x;
            originalY = y;
        }

        public EffectBox(int x, int y, int width, int height, Vector2 accelerationChange,PhysicsEntity parent, Texture2D texture = null)
            : base(x, y, width, height, texture, false)
        {
            this.accelerationChange = accelerationChange;
            this.parent = parent;
            originalX = x;
            originalY = y;
        }

    }
}
