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

        private Point parentCentrer;

        public Rectangle OriginalRect { get { return originalRect; } }
        public bool HasParent { get { return parent != null; } }
        public float XAccelerationChange { get { return accelerationChange.X; } }

        public float YAccelerationChange { get { return accelerationChange.Y; } }

        public override void OnCollide(PhysicsEntity other)
        {
            other.AddForce(accelerationChange);
            if (HasParent)
            {
                parent.OnCollide(other);
                if (other.X >= parentCentrer.X && other.X < rectangle.Width + X && other.X > X)
                {
                    rectangle.Width = X + (other.X - X);
                }
                else if (other.X >= parentCentrer.X && other.X + other.Rect.Width > X && other.X < X)
                {
                    rectangle.Width = 0;
                }
                else if (other.X + other.Rect.Width <= parentCentrer.X && other.X + other.Rect.Width < rectangle.Width + X && other.X + other.Rect.Width > X)
                {
                    rectangle.Width = other.X + other.Rect.Width + ((X+rectangle.Width) - (other.X + other.Rect.Width));
                    rectangle.X = other.X + other.Rect.Width;
                }
                else if (other.X + other.Rect.Width <= parentCentrer.X && other.X + other.Rect.Width > X + rectangle.Width && other.X > X)
                {
                    rectangle.Width = 0;
                }

                if (other.Y >= parentCentrer.Y && other.Y < rectangle.Height + Y && other.Y > Y)
                {
                    rectangle.Height = Y + (other.Y - Y);
                }
                else if (other.Y >= parentCentrer.Y && other.Y + other.Rect.Height > Y && other.Y < Y)
                {
                    rectangle.Height = 0;
                }
                else if (other.Y + other.Rect.Height <= parentCentrer.Y && other.Y + other.Rect.Height < rectangle.Height + Y && other.Y + other.Rect.Height > Y)
                {
                    rectangle.Height = other.Y + other.Rect.Height + ((Y + rectangle.Height) - (other.Y + other.Rect.Height));
                    rectangle.Y = other.Y + other.Rect.Height;
                }
                else if (other.Y + other.Rect.Height <= parentCentrer.Y && other.Y + other.Rect.Height > Y + rectangle.Height && other.Y > Y)
                {
                    rectangle.Height = 0;
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


        /*//this shouldn't be a thing for now, parents that are effects boxes
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
        */

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
        {
            base.Draw(device, spriteBatch, x, y);
            ResetSize();

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


        public EffectBox(int x, int y, int width, int height, Vector2 accelerationChange, Texture2D texture = null)
            : base(x, y, width, height, texture, null, false)
        {
            this.accelerationChange = accelerationChange;
            parent = null;
            originalRect = new Rectangle(x, y, width, height);
        }

        public EffectBox(int x, int y, int width, int height, Vector2 accelerationChange, PhysicsEntity parent, Texture2D texture = null)
            : base(x, y, width, height, texture, null, false)
        {
            this.accelerationChange = accelerationChange;
            this.parent = parent;
            parentCentrer = new Point(parent.X + parent.Rect.Width / 2, parent.Y + parent.Rect.Height / 2);
            originalRect = new Rectangle(x, y, width, height);
        }

    }
}
