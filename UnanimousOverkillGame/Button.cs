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
        private bool isThrowButton;
        private SpriteFont font;

        public EffectBox Box { get { return box; } }
        public bool PlayerInRange { get { return playerInRange; } }

        public void PressButton()
        {
            if (click != null)
                click();
        }

        public override void AddInformation(List<String> infoLines, GameObject[,] objects)
        {
            List<Point> tempPoint = new List<Point>();
            int xNum, yNum;
            if (infoLines != null)
                for (int i = 0; i < infoLines.Count; i++)
                {
                    if (i + 1 < infoLines.Count)
                    {
                        if (Int32.TryParse(infoLines[i], out xNum) && Int32.TryParse(infoLines[i + 1], out yNum))
                        {
                            tempPoint.Add(new Point(xNum, yNum));
                        }
                    }
                }
            foreach (GameObject obj in getAttatchedObjects(objects,tempPoint.ToArray()))
            {
                if (obj is IsClickableObject)
                    AddObject((IsClickableObject)obj);
            }
        }

        private List<GameObject> getAttatchedObjects(GameObject[,] levelObjects, params Point[] locations)
        {

            List<GameObject> attatchedObjects = new List<GameObject>();
            foreach (Point p in locations)
            {
                if (p.X < 0 || p.X >= levelObjects.GetLength(0) || p.Y < 0 || p.Y >= levelObjects.GetLength(1))
                    continue;


                if (levelObjects[p.X, p.Y] == null)
                    continue;

                attatchedObjects.Add(levelObjects[p.X, p.Y]);
            }

            return attatchedObjects;
        }

        public void AddObject(IsClickableObject obj)
        {
            click += obj.onClick;
        }

        public override void OnCollide(PhysicsEntity other)
        {
            base.OnCollide(other);
            if (!isThrowButton)
            {
                if (other is Player)
                {
                    playerInRange = true;
                    (other as Player).ButtonInRange = this;
                }
            }
            else
            {
                if (other is Bottle)
                {
                    PressButton();
                }
            }
        }

        protected override void onPositionChange()
        {
            base.onPositionChange();
            box.X = X - BUTTON_RANGE;
            box.Y = Y - BUTTON_RANGE;
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y)
        {
            base.Draw(device, spriteBatch, x, y);

            if (playerInRange && !isThrowButton)
            {
                spriteBatch.DrawString(font, "E", new Vector2(x/* + this.Rect.Width / 2 - 5*/, y/* - 10*/), Color.White);
                playerInRange = false;
            }


        }

        public Button(int x, int y, int width, int height, Texture2D texture, SpriteFont font, IsClickableObject[] objs, bool isThrowButton = false) :
            base(x, y, width, height, texture, null)
        {
            this.font = font;
            this.isThrowButton = isThrowButton;
            if (objs != null)
                foreach (IsClickableObject obj in objs)
                {
                    click += obj.onClick;
                }
            if (!isThrowButton)
            {
                box = new EffectBox(x - BUTTON_RANGE, y - BUTTON_RANGE, (2 * BUTTON_RANGE) + width, (2 * BUTTON_RANGE) + height, Vector2.Zero, this);
            }
            else
            {
                box = new EffectBox(x - 5, y - 5, width + 10, height + 10,Vector2.Zero,this);
            }
        }
    }
}
