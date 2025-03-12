using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LAB1
{
    class Canvas
    {
        private char[,] grid;
        public int Width { get; private set; }
        public int Height { get; private set; }
        private List<Shape> shapes = new List<Shape>();

        public string GetShapesState()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(shapes, options);
        }

        // Метод для установки состояния фигур из JSON-строки
        public void SetShapesState(string shapesState)
        {
            var options = new JsonSerializerOptions();
            shapes = JsonSerializer.Deserialize<List<Shape>>(shapesState, options) ?? new List<Shape>();
            //Console.WriteLine($"Загружено фигур: {shapes.Count}");
            foreach (var shape in shapes)
            {
                //Console.WriteLine($"Тип: {shape.GetType().Name}, X: {shape.X}, Y: {shape.Y}");
                if (shape is Line line)
                {
                    //Console.WriteLine($"EndX: {line.EndX}, EndY: {line.EndY}");
                }
            }
            Redraw();
        }
        public Canvas(int width, int height)
        {
            Width = width;
            Height = height;
            grid = new char[height, width];
            // Background = '·';
            Clear(); // Заполняем пробелами
        }
        public void Clear()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    grid[i, j] = '·';//Background; // Фон (точка как пустой фон)
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
                int deltaX = newX - shape.X; // Смещение по X
                int deltaY = newY - shape.Y; // Смещение по Y

                shape.X = newX;
                shape.Y = newY;

                // Если фигура — линия, обновляем конечные координаты
                if (shape is Line line)
                {
                    line.EndX += deltaX;
                    line.EndY += deltaY;
                }

                Redraw();
            }
        }
        private void Redraw()
        {
            Clear(); // Очищаем холст
            foreach (var shape in shapes)
            {
                shape.Draw(grid); // Рисуем каждую фигуру
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
        public string GetState()
        {
            string state = "";
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    state += grid[i, j];
                }
            }
            return state;
        }
        public void SetState(string state)
        {
            int index = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (index < state.Length)
                    {
                        grid[i, j] = state[index++];
                    }
                }
            }
        }
    }
}
