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
        private Rectangle originalRect;
        private PhysicsEntity parent;
        private PhysicsEntity child;


        public Rectangle OriginalRect { get { return originalRect; } }
        public bool HasParent { get { return parent != null; } }
        public float XAccelerationChange { get { return accelerationChange.X; } }

        public float YAccelerationChange { get { return accelerationChange.Y; } }

        public override void OnCollide(PhysicsEntity other)
        {
            if((HasParent && !(parent is EffectBox)) || !HasParent )
                other.AddForce(accelerationChange);
            if (HasParent)
            {
                parent.OnCollide(other);
                if (parent is EffectBox)
                {
                    EffectBox par = parent as EffectBox;
                    if (par.Rect.Equals(par.OriginalRect) || other.Rect.Contains(this.originalRect))
                    {
                        RoomManager.GetRoomManager.Current.Colliders.Remove(this);
                        RoomManager.GetRoomManager.Current.Enemies.Remove(this);
                        RoomManager.GetRoomManager.UpdateCollisionManager(RoomManager.GetRoomManager.Current);
                        par.child = null;
                        //got to make sure all calls to this are gone.
                        return;
                    }


                }
            }
        }


        private Rectangle GetOriginalBoxRect(EffectBox box)
        {
            if (box.HasParent && box.parent is EffectBox)
            {
                return GetOriginalBoxRect(box.parent as EffectBox);
            }

            return box.OriginalRect;
        }

        private Rectangle GetOriginalParentRect(EffectBox box)
        {
            if (!box.HasParent)
            {
                return box.rectangle;
            }

            if (box.HasParent && !(box.parent is EffectBox))
            {
                return box.parent.Rect;
            }

            return GetOriginalParentRect(box.parent as EffectBox);


        }


        private void AdjustSize(PhysicsEntity other)
        {
            if (parent == null)
            {
                return;
            }
        }

        public void ResetSize()
        {
            rectangle.X = originalRect.X;
            rectangle.Y = originalRect.Y;
            rectangle.Width = originalRect.Width;
            rectangle.Height = originalRect.Height;
        }


        public EffectBox(int x, int y, int width, int height,Vector2 accelerationChange, Texture2D texture = null) : base(x,y,width,height,texture,null,false)
        {
            this.accelerationChange = accelerationChange;
            parent = null;
            originalRect = new Rectangle(x, y, width, height);
        }

        public EffectBox(int x, int y, int width, int height, Vector2 accelerationChange,PhysicsEntity parent, Texture2D texture = null)
            : base(x, y, width, height, texture, null, false)
        {
            this.accelerationChange = accelerationChange;
            this.parent = parent;
            originalRect = new Rectangle(x, y, width, height);
        }

    }
}
