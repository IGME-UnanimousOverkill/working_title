using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Text;

namespace UnanimousOverkillGame
{
    class Button : PhysicsEntity
    {
        private static readonly int BUTTON_RANGE = 100;


        private bool playerInRange;
        private List<PhysicsEntity> objs;
        private event Clicked click;
        private EffectBox box;

        private SpriteFont font;

        public EffectBox Box { get { return box; } }
        public bool PlayerInRange { get { return playerInRange; } }

        public void PressButton()
        {
            if (click != null)
                click();
        }

        public void AddObject(IsClickableObject obj)
        {
            click += obj.onClick;
        }

        public override void OnCollide(PhysicsEntity other)
        {
            base.OnCollide(other);
            if (other is Player)
            {
                playerInRange = true;
                (other as Player).ButtonInRange = this;
            }
        }

        protected override void onPositionChange()
        {
            base.onPositionChange();
            box.X = X - BUTTON_RANGE;
            box.Y = Y - BUTTON_RANGE;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            base.Draw(spriteBatch, x, y);

            if (playerInRange)
            {
                spriteBatch.DrawString(font, "E", new Vector2(x/* + this.Rect.Width / 2 - 5*/, y/* - 10*/), Color.White);
                playerInRange = false;
            }


        }

        public Button(int x, int y, int width, int height, Texture2D texture, SpriteFont font, params IsClickableObject[] objs) :
            base(x, y, width, height, texture)
        {
            this.font = font;

            if (objs != null)
                foreach (IsClickableObject obj in objs)
                {
                    click += obj.onClick;
                }

            box = new EffectBox(x - BUTTON_RANGE, y - BUTTON_RANGE, (2 * BUTTON_RANGE) + width, (2 * BUTTON_RANGE) + height, Vector2.Zero, this);
        }
    }
}
