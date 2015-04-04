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

        public float XAccelerationChange { get { return accelerationChange.X; } }

        public float YAccelerationChange { get { return accelerationChange.Y; } }

        public override void OnCollide(PhysicsEntity other)
        {
            other.AddForce(accelerationChange);
        }


        public EffectBox(int x, int y, int width, int height,Vector2 accelerationChange, Texture2D texture = null) : base(x,y,width,height,texture,false)
        {
            this.accelerationChange = accelerationChange;
        }

    }
}
