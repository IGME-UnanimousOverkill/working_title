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

    enum Direction
    {
        up,right, down, left
    }
    class Fan : PhysicsEntity
    {

        private Direction direction;
        private List<EffectBox> effectBoxes;

        private void SpawnEffect(Direction d)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (d)
                {
                    case Direction.up:
                        effectBoxes.Add(new EffectBox(this.X, this.Y  - (2*Room.TILE_HEIGHT), Room.TILE_WIDTH, Room.TILE_HEIGHT*3, new Vector2(0, -25)));
                        break;
                    case Direction.right:
                        effectBoxes.Add(new EffectBox(this.X, this.Y, Room.TILE_WIDTH*3, Room.TILE_HEIGHT, new Vector2(25, 0)));
                        break;
                    case Direction.down:
                        effectBoxes.Add(new EffectBox(this.X, this.Y, Room.TILE_WIDTH, Room.TILE_HEIGHT*3, new Vector2(0, 25)));
                        break;
                    case Direction.left:
                        effectBoxes.Add(new EffectBox(this.X - (2 * Room.TILE_WIDTH), this.Y , Room.TILE_WIDTH*3, Room.TILE_HEIGHT, new Vector2(-25, 0)));
                        break;
                    default:
                        break;
                }
            }
        }

        public List<EffectBox> getEffects()
        {
            return effectBoxes;
        }

        public Fan(int x, int y, int width, int height, char fanType)
            : base(x, y, width, height, null)
        {
            isCollidable = false;
            switch (fanType)
            {
                case '┬':
                    direction = Direction.down;
                    break;
                case '├':
                    direction = Direction.right;
                    break;
                case '┤':
                    direction = Direction.left;
                    break;
                default:
                    direction = Direction.up;
                    break;
            }
            effectBoxes = new List<EffectBox>();

            SpawnEffect(direction);
        }




    }
}
