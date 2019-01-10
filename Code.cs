using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Maze
{
    class Code
    {
        public enum CellMeaning
        {
            Unvisited,
            Wall,
            Passage,
            Visited,
            Carver
        };

        public enum Directions
        {
            North,
            East,
            South,
            West
        };

        static Point[] steps = new Point[4]
        {
            new Point(0, -1),
            new Point(1, 0),
            new Point(0, 1),
            new Point(-1, 0)
        };

        public static int[,] InitializeGrid(int[,] grid)
        {
            int[,] result = grid;
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        result[i, j] = (int)CellMeaning.Unvisited;
                    }
                    else result[i, j] = (int)CellMeaning.Wall;
                }
            }
            return result;
        }

        public static Bitmap Render(Color color, Bitmap maze, int leftPixel, int rightPixel,
                                                              int topPixel,  int bottomPixel)
        {
            for (int x = leftPixel; x < rightPixel; x++)
            {
                for (int y = topPixel; y < bottomPixel; y++)
                {
                    maze.SetPixel(x, y, color);
                }
            }
            return maze;
        }
        
        public static Bitmap Render(int[,] grid, Bitmap maze, int cellSize, int left, int right, int top, int bottom)
        {
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    for (int x = i * cellSize; x < (i + 1) * cellSize; x++)
                    {
                        for (int y = j * cellSize; y < (j + 1) * cellSize; y++)
                        {
                            switch (grid[i, j])
                            {
                                case (int)CellMeaning.Unvisited:
                                    maze.SetPixel(x, y, Color.LightGray);
                                    break;
                                case (int)CellMeaning.Wall:
                                    maze.SetPixel(x, y, Color.Black);
                                    break;
                                case (int)CellMeaning.Passage:
                                    maze.SetPixel(x, y, Color.White);
                                    break;
                                case (int)CellMeaning.Visited:
                                    maze.SetPixel(x, y, Color.LightPink);
                                    break;
                                case (int)CellMeaning.Carver:
                                    maze.SetPixel(x, y, Color.LightGreen);
                                    break;
                                default:
                                    maze.SetPixel(x, y, Color.Black);
                                    break;
                            }
                        }
                    }
                }
            }
            return maze;
        }

        public static Tuple<Point, Directions, int> MoveTo(Bitmap maze, Point location, int cellSize, Directions direction)
        {
            direction = (Directions)((int)(direction + 3) % 4);
            int rotation = 3;
            for (int i = 0; i < 4; i++)
            {
                bool notWall = maze.GetPixel(location.X + steps[(int)direction].X * cellSize,
                                             location.Y + steps[(int)direction].Y * cellSize).ToArgb() 
                                             != Color.Black.ToArgb();
                if (notWall)
                {
                    location.X += (steps[(int)direction].X * cellSize);
                    location.Y += (steps[(int)direction].Y * cellSize);
                    break;
                }
                else
                {
                    direction = (Directions)((int)(direction + 1) % 4);
                    rotation = (rotation + 1) % 4;
                }
            }
            return Tuple.Create(location, direction, rotation);
        }

        public static Point CarveTo(int[,] grid, Point currentLocation)
        {
            Random rnd = new Random();
            var shuffledDirections = new int[] { 0, 1, 2, 3 }.OrderBy(a => rnd.Next()).ToArray();
            var destination = currentLocation;
            for (int i = 0; i < 4; i++)
            {
                bool inBounds = currentLocation.X + steps[shuffledDirections[i]].X * 2 >= 0 &&
                                currentLocation.X + steps[shuffledDirections[i]].X * 2 < grid.GetLength(0) &&
                                currentLocation.Y + steps[shuffledDirections[i]].Y * 2 >= 0 &&
                                currentLocation.Y + steps[shuffledDirections[i]].Y * 2 < grid.GetLength(1);
                bool notVisited = false;
                if (inBounds)
                    notVisited = grid[currentLocation.X + steps[shuffledDirections[i]].X * 2,
                                      currentLocation.Y + steps[shuffledDirections[i]].Y * 2] == 0;
                if (inBounds && notVisited)
                {
                    destination.X = currentLocation.X + steps[shuffledDirections[i]].X * 2;
                    destination.Y = currentLocation.Y + steps[shuffledDirections[i]].Y * 2;
                    Mark(grid, currentLocation, destination);
                    break;
                }
            }
            return destination;
        }

        public static void Mark(int[,] grid, Point currentLocation, Point destination)
        {
            int left = Math.Min(currentLocation.X, destination.X);
            int right = Math.Max(currentLocation.X, destination.X);
            int top = Math.Min(currentLocation.Y, destination.Y);
            int bottom = Math.Max(currentLocation.Y, destination.Y);
            int destinationValue = grid[destination.X, destination.Y];
            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    if (destinationValue == (int)CellMeaning.Visited)
                        grid[x, y] = (int)CellMeaning.Passage;
                    else
                        grid[x, y] = (int)CellMeaning.Visited;
                }
            }
            grid[destination.X, destination.Y] = (int)CellMeaning.Carver;
        }

        public static Bitmap DrawSchematicMouse(Bitmap mouse)
        {
            for (int y = 0; y < mouse.Height; y++)
            {
                for (int x = 0; x < mouse.Width; x++)
                {
                    if (x >= y && x < mouse.Width - y)
                        mouse.SetPixel(x, y, Color.LightBlue);
                    else
                        mouse.SetPixel(x, y, Color.Gray);
                }
            }
            return mouse;
        }
    }
}
