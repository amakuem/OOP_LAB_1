using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LAB1
{
    public class Canvas
    {
        private char[,] grid;
        public int Width { get; private set; }
        public int Height { get; private set; }
        private List<Shape> shapes = new List<Shape>();

        private void DrawCircle(Circle circle)
        {
            for (int i = circle.Y - circle.Radius; i <= circle.Y + circle.Radius && i < grid.GetLength(0); i++)
            {
                for (int j = circle.X - circle.Radius; j <= circle.X + circle.Radius && j < grid.GetLength(1); j++)
                {
                    if (i >= 0 && j >= 0 && Math.Sqrt(Math.Pow(i - circle.Y, 2) + Math.Pow(j - circle.X, 2)) <= circle.Radius)
                    {
                        grid[i, j] = circle.Symbol;
                    }
                }
            }
        }

        private void DrawRectangle(Rectangle rectangle)
        {
            for (int i = rectangle.Y; i < rectangle.Y + rectangle.Height && i < grid.GetLength(0); i++)
            {
                for (int j = rectangle.X; j < rectangle.X + rectangle.Width && j < grid.GetLength(1); j++)
                {
                    if (i >= 0 && j >= 0)
                    {
                        grid[i, j] = rectangle.Symbol;
                    }
                }
            }
        }
        private void DrawLine(Line line)
        {
            int x0 = line.X;
            int y0 = line.Y;
            int x1 = line.EndX;
            int y1 = line.EndY;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (x0 >= 0 && x0 < grid.GetLength(1) && y0 >= 0 && y0 < grid.GetLength(0))
                {
                    grid[y0, x0] = line.Symbol;
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

        public string GetShapesState()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(shapes, options);
        }

        
        public void SetShapesState(string shapesState)
        {
            var options = new JsonSerializerOptions();
            shapes = JsonSerializer.Deserialize<List<Shape>>(shapesState, options) ?? new List<Shape>();
           
            Redraw();
        }
        public Canvas(int width, int height)
        {
            Width = width;
            Height = height;
            grid = new char[height, width];
          
            Clear(); 
        }
        public void Clear()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    grid[i, j] = '·';
                }
            }
        }
        public void AddShape(Shape shape)
        {
            shapes.Add(shape);
            Redraw();
        }
        public void EraseShape(int index)
        {
            if (index >= 0 && index < shapes.Count)
            {
                shapes.RemoveAt(index);
                Redraw();
            }
        }
        public void MoveShape(int index, int newX, int newY)
        {
            if (index >= 0 && index < shapes.Count)
            {
                Shape shape = shapes[index];
                int deltaX = newX - shape.X; 
                int deltaY = newY - shape.Y;

                shape.X = newX;
                shape.Y = newY;

               
                if (shape is Line line)
                {
                    line.EndX += deltaX;
                    line.EndY += deltaY;
                }

                Redraw();
            }
        }
        public void ClearCanvas()
        {
            shapes.Clear();
            Clear();        
        }
        public void Redraw()
        {
            Clear(); 
            for (int i = 0; i < shapes.Count; i++)
            {
                Shape shape = shapes[i];
                if (shape is Circle circle)
                {
                    
                    DrawCircle((Circle)shape);
                }
                else if (shape is Rectangle recangle)
                {
                    DrawRectangle((Rectangle)shape);
                }
                else if (shape is Line line)
                {
                    DrawLine((Line)shape);
                }
            }
        }
        public void Display()
        {
            Console.Clear();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Console.Write(grid[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        public void ListShapes()
        {
            if (shapes.Count == 0)
            {
                Console.WriteLine("На холсте нет фигур.");
                return;
            }

            Console.WriteLine("Фигуры на холсте:");
            for (int i = 0; i < shapes.Count; i++)
            {
                Shape shape = shapes[i];
                string shapeType;
                string details = "";

                if (shape is Circle circle)
                {
                    shapeType = "Круг";
                    details = $"Радиус: {circle.Radius}";
                }
                else if (shape is Rectangle rectangle)
                {
                    shapeType = "Прямоугольник";
                    details = $"Ширина: {rectangle.Width}, Высота: {rectangle.Height}";
                }
                else if (shape is Line line)
                {
                    shapeType = "Линия";
                    details = $"Конец X: {line.EndX}, Конец Y: {line.EndY}";
                }
                else
                {
                    shapeType = "Неизвестная фигура";
                }

                Console.WriteLine($"Индекс: {i}, Тип: {shapeType}, X: {shape.X}, Y: {shape.Y}, Символ: {shape.Symbol}, {details}");
            }
            Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
            Console.ReadLine();
        }
    }
}
