using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_Tool
{
    public partial class MapTool : Form
    {
        public Image grid = Image.FromFile("Content/grid.png");
        public Image gridFilled = Image.FromFile("Content/gridFilled.png");

        private List<PictureBox> tiles;

        private const int TILE_SIZE = 25;

        public MapTool()
        {
            InitializeComponent();
            tiles = new List<PictureBox>(400);
            populateTiles();
        }

        private void populateTiles()
        {
            foreach (PictureBox box in tiles)
            {
                box.Dispose();
            }
            tiles.Clear();
            for (int y = 0; y < heightNumericUpDown.Value; y++)
            {
                for (int x = 0; x < widthNumericUpDown.Value; x++)
                {
                    PictureBox tile = new PictureBox();
                    tile.BringToFront();
                    tile.Parent = panel1;
                    tile.Location = new Point(x * TILE_SIZE, y * TILE_SIZE);
                    tile.Size = new Size(TILE_SIZE, TILE_SIZE);
                    tile.Image = grid;
                    tile.MouseDown += (s, e) =>
                    {
                        if (tile.Image == gridFilled)
                        {
                            tile.Image = grid;
                        }
                        else
                        {
                            tile.Image = gridFilled;
                        }
                    };
                    tiles.Add(tile);
                    panel1.Controls.Add(tile);
                }
            }
        }

        private void widthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            populateTiles();
        }

        private void heightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            populateTiles();
        }

        private void disableMousewheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }
}
