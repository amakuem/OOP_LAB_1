using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB1
{
    class Line: Shape
    {
        private int num;
        public int EndX { get; set; }
        public int EndY { get; set; }
        public Line() : base(0, 0, ' ')
        {
        }
        public Line(int startX, int startY, int endX, int endY, char symbol)
            : base(startX, startY, symbol)
        {
            EndX = endX;
            EndY = endY;
        }

        public override void Draw(char[,] canvas)
        {
            int x0 = X;
            int y0 = Y;
            int x1 = EndX;
            int y1 = EndY;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (x0 >= 0 && x0 < canvas.GetLength(1) && y0 >= 0 && y0 < canvas.GetLength(0))
                {
                    canvas[y0, x0] = Symbol;
                }

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}
