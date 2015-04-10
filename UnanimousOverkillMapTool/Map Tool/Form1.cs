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

        private PictureBox[,] tiles;
        private string currentTile;
        private Image currentImage;
        private PictureBox selected;

        private const int TILE_SIZE = 25;

        public MapTool()
        {
            InitializeComponent();
            tiles = new PictureBox[20,20];
            populateTiles();
            currentTile = "*";
            selected = pictureBox2;
            currentImage = gridFilled;
        }

        private void populateTiles()
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    if (tiles[x,y] != null)
                    {
                        tiles[x, y].Dispose();
                    }
                }
            }
            tiles = new PictureBox[(int)widthNumericUpDown.Value, (int)heightNumericUpDown.Value];
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
                        if (tile.Image == currentImage)
                        {
                            tile.Image = grid;
                            tile.Tag = " ";
                        }
                        else
                        {
                            tile.Image = currentImage;
                            tile.Tag = currentTile;
                        }
                    };
                    tile.Tag = " ";
                    tiles[x,y] = tile;
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

        private void ExportButton_Click(object sender, EventArgs e)
        {
            (new System.IO.FileInfo("Rooms/" + nameTextBox.Text + ".txt")).Directory.Create();
            System.IO.StreamWriter writer = new System.IO.StreamWriter("Rooms/" + nameTextBox.Text + ".txt");
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                string line = "";
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    if (tiles[x, y] != null)
                    {
                        line += tiles[x, y].Tag as string;
                    }
                }
                if (!string.IsNullOrWhiteSpace(line))
                {
                    writer.WriteLine(line);
                }
            }
            writer.WriteLine("//");
            writer.Close();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox box = sender as PictureBox;
            selected.Padding = new Padding(0);
            selected.Refresh();
            selected = box;
            box.Padding = new Padding(2);
            box.Refresh();
            currentImage = box.Image;
            currentTile = box.Tag as string;
        }
    }
}
