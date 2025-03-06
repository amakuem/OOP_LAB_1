using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB1
{
    class Rectangle: Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle (int x, int y, int width, int height, char symbol):base (x,y,symbol)
        {
            Width = width;
            Height = height;
        }
        public override void Draw(char[,] canvas)
        {
            for(int i = Y; i < Y + Height && i < canvas.GetLength(0); i++)
            {
                for(int j = X; j < X + Width && j < canvas .GetLength(1); j++)
                {
                    if(i >= 0 && j >= 0)
                    {
                        canvas[i, j] = Symbol;
                    }
                }
            }
        }
    }
}
