using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maze
{
    public partial class Form1 : Form
    {
        public static Stack<Point> backtrack = new Stack<Point>();

        public static Point CarveFrom(Point point, Bitmap maze, int[,] grid, int cellSize)
        {
            var nextPoint = Code.CarveTo(grid, point);
            if (nextPoint != point)
            {
                backtrack.Push(point);
            }
            else
            {
                nextPoint = backtrack.Pop();
                Code.Mark(grid, point, nextPoint);
            }
            int renderLeftBorder = Math.Min(point.X, nextPoint.X);
            int renderRightBorder = Math.Max(point.X, nextPoint.X) + 1;
            int renderTopBorder = Math.Min(point.Y, nextPoint.Y);
            int renderBottomBorder = Math.Max(point.Y, nextPoint.Y) + 1;
            maze = Code.Render(grid, maze, cellSize, renderLeftBorder,
                                                     renderRightBorder,
                                                     renderTopBorder,
                                                     renderBottomBorder);
            return nextPoint;
        }

        Bitmap maze = new Bitmap(1125, 675);
        int cellSize = 0;

        PictureBox pictureBox2 = new PictureBox()
        {
            Visible = false
        };
        Bitmap mouse;
        Point exit;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            pictureBox1.Controls.Add(pictureBox2);
        }

        int[,] grid = new int[375, 225];

        private void button1_Click(object sender, EventArgs e)
        {
            cellSize = Convert.ToInt32(comboBox1.SelectedItem);
            grid = new int[1125 / cellSize, 675 / cellSize];
            grid = Code.InitializeGrid(grid);
            maze = Code.Render(grid, maze, cellSize, 0, grid.GetLength(0), 0, grid.GetLength(1));
            pictureBox1.Image = maze;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            comboBox1.Enabled = false;
            Point carvePoint = new Point(1, 1);
            backtrack.Push(carvePoint);
            while (backtrack.Count != 0)
            {
                carvePoint = CarveFrom(carvePoint, maze, grid, cellSize);
                pictureBox1.Refresh();
            }
            grid[1, 1] = (int)Code.CellMeaning.Passage;
            maze = Code.Render(grid, maze, cellSize, 1, 2, 1, 2);

            mouse = new Bitmap(cellSize, cellSize);
            mouse = Code.DrawSchematicMouse(mouse);
            pictureBox2.Height = cellSize;
            pictureBox2.Width = cellSize;
            pictureBox2.Image = mouse;
            exit = new Point(cellSize, cellSize);

            button2.Enabled = false;
            pictureBox1.Enabled = true;
            MessageBox.Show("Укажите на лабиринте местоположение выхода и мыши.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var currentDirection = Code.Directions.North;
            while (maze.GetPixel(pictureBox2.Location.X, pictureBox2.Location.Y).ToArgb() != Color.LightGreen.ToArgb())
            {
                var tuple = Code.MoveTo(maze, pictureBox2.Location, cellSize, currentDirection);
                pictureBox2.Location = tuple.Item1;
                currentDirection = tuple.Item2;
                pictureBox2.Image.RotateFlip((RotateFlipType)tuple.Item3);
                pictureBox1.Refresh();
                pictureBox2.Refresh();
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (maze.GetPixel(e.X, e.Y).ToArgb() != Color.Black.ToArgb())
            {
                maze = Code.Render(Color.White, maze, exit.X, exit.X + cellSize, exit.Y, exit.Y + cellSize);
                pictureBox2.Location = exit;
                exit.X = e.X / cellSize * cellSize;
                exit.Y = e.Y / cellSize * cellSize;
                pictureBox2.Visible = true;
                maze = Code.Render(Color.LightGreen, maze, exit.X, exit.X + cellSize, exit.Y, exit.Y + cellSize);
                pictureBox1.Image = maze;
                button3.Enabled = true;
            }
            else
            {
                MessageBox.Show("Здесь стена!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            maze = new Bitmap(1125, 675);
            pictureBox1.Image = maze;
            comboBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            pictureBox1.Enabled = false;
            pictureBox2.Visible = false;
        }
    }
}
