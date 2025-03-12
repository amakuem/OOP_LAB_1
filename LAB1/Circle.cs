using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB1
{
    public class Circle: Shape
    {
        public int Radius { get; set; }

        public Circle(int x, int y, int radius, char symbol)
            : base(x, y, symbol)
        {
            Radius = radius;
        }
    }
}
