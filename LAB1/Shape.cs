using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LAB1
{
    [JsonDerivedType(typeof(Circle), typeDiscriminator: "circle")]
    [JsonDerivedType(typeof(Rectangle), typeDiscriminator: "rectangle")]
    [JsonDerivedType(typeof(Line), typeDiscriminator: "line")]
    abstract class Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
        public Shape(int x, int y, char symbol)
        {
            X = x;
            Y = y;
            Symbol = symbol;
        }
    }
}
