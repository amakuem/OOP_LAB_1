using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB1
{
    class Circle: Shape
    {
        public int Radius { get; set; }

        public Circle(int x, int y, int radius, char symbol)
            : base(x, y, symbol)
        {
            Radius = radius;
        }

        public override void Draw(char[,] canvas)
        {
            for (int i = Y - Radius; i <= Y + Radius && i < canvas.GetLength(0); i++)
            {
                for (int j = X - Radius; j <= X + Radius && j < canvas.GetLength(1); j++)
                {
                    if (i >= 0 && j >= 0 && Math.Sqrt(Math.Pow(i - Y, 2) + Math.Pow(j - X, 2)) <= Radius)
                    {
                        canvas[i, j] = Symbol;
                    }
                }
            }
        }
    }
}
