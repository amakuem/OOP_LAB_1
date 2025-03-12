using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB1
{
    public class Line: Shape
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
    }
}
