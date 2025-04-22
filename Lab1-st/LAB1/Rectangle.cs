using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB1
{
    public class Rectangle: Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle (int x, int y, int width, int height, char symbol):base (x,y,symbol)
        {
            Width = width;
            Height = height;
        }
    }
}
