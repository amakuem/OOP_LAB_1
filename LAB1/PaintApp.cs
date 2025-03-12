using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB1
{
    class PaintApp
    {
        private Canvas canvas;
        private Stack<string> undoStack;
        private Stack<string> redoStack;
        public PaintApp(int width, int height)
        {
            canvas = new Canvas(width, height);
            undoStack = new Stack<string>();
            redoStack = new Stack<string>();
            SaveState(); // Сохраняем начальное состояние
        }

        public void Run()
        {
            while (true)
            {
                canvas.Display();
                Console.WriteLine("\nКоманды:");
                Console.WriteLine("1. Нарисовать фигуру (R - прямоугольник, C - круг)");
                Console.WriteLine("2. Стереть объект (укажите индекс фигуры)");
                Console.WriteLine("3. Переместить объект (укажите индекс фигуры и новые X Y)");
                Console.WriteLine("4. Сохранить в файл");
                Console.WriteLine("5. Загрузить из файла");
                Console.WriteLine("6. Undo");
                Console.WriteLine("7. Redo");
                Console.WriteLine("8. Выход");
                Console.WriteLine("9. Показать список фигур"); // Новая команда
                Console.Write("Выберите команду: ");

                string input = Console.ReadLine();
                try
                {
                    ProcessCommand(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        private void ProcessCommand(string input)
        {
            switch (input)
            {
                case "1": // Рисование фигуры
                    Console.Write("Тип фигуры (R - прямоугольник, C - круг, L - линия): ");
                    char type = Console.ReadLine().ToUpper()[0];
                    if(type != 'R' && type != 'C' && type != 'L')
                    {
                        break;
                    }
                    Console.Write("X: ");
                    int x = int.Parse(Console.ReadLine());
                    Console.Write("Y: ");
                    int y = int.Parse(Console.ReadLine());
                    Console.Write("Символ: ");
                    char symbol = Console.ReadLine()[0];

                    if (type == 'R')
                    {
                        Console.Write("Ширина: ");
                        int width = int.Parse(Console.ReadLine());
                        Console.Write("Высота: ");
                        int height = int.Parse(Console.ReadLine());
                        var rect = new Rectangle(x, y, width, height, symbol);
                        canvas.AddShape(rect);
                    }
                    else if (type == 'C')
                    {
                        Console.Write("Радиус: ");
                        int radius = int.Parse(Console.ReadLine());
                        var circle = new Circle(x, y, radius, symbol);
                        canvas.AddShape(circle);
                    }
                    else if (type == 'L')
                    {
                        Console.Write("Конец X: ");
                        int endX = int.Parse(Console.ReadLine());
                        Console.Write("Конец Y: ");
                        int endY = int.Parse(Console.ReadLine());
                        var line = new Line(x, y, endX, endY, symbol);
                        canvas.AddShape(line);
                    }
                    else
                    {
                        Console.WriteLine("Неверный тип фигуры.");
                        break;
                        //return;
                    }
                    SaveState();
                    break;

                case "2": // Стирание фигуры
                    Console.Write("Индекс фигуры для стирания: ");
                    int eraseIndex = int.Parse(Console.ReadLine());
                    canvas.EraseShape(eraseIndex);
                    SaveState();
                    break;

                case "3": // Перемещение фигуры
                    Console.Write("Индекс фигуры для перемещения: ");
                    int moveIndex = int.Parse(Console.ReadLine());
                    Console.Write("Новый X: ");
                    int newX = int.Parse(Console.ReadLine());
                    Console.Write("Новый Y: ");
                    int newY = int.Parse(Console.ReadLine());
                    canvas.MoveShape(moveIndex, newX, newY);
                    SaveState();
                    break;

                case "4": // Сохранение
                    Console.Write("Имя файла(с .json расширением): ");
                    string saveFile = Console.ReadLine();
                    string shapesState = canvas.GetShapesState(); // Получаем JSON фигур
                    File.WriteAllText(saveFile, shapesState); // Сохраняем в файл
                    Console.WriteLine("Сохранено успешно.");
                    break;

                case "5": // Загрузка
                    Console.Write("Имя файла: ");
                    string loadFile = Console.ReadLine();
                    if (File.Exists(loadFile))
                    {
                        shapesState = File.ReadAllText(loadFile); // Читаем JSON из файла
                        canvas.SetShapesState(shapesState); // Восстанавливаем фигуры и перерисовываем
                        Console.WriteLine("Загружено успешно.");
                    }
                    else
                    {
                        Console.WriteLine("Файл не найден.");
                    }
                    break;

                case "6": // Undo
                    if (undoStack.Count > 1) // Оставляем текущее состояние
                    {
                        string currentState = canvas.GetShapesState();
                        redoStack.Push(currentState);
                        undoStack.Pop(); // Удаляем текущее состояние
                        string prevState = undoStack.Peek();
                        canvas.SetShapesState(prevState);
                    }
                    break;

                case "7": // Redo
                    if (redoStack.Count > 0)
                    {
                        string nextState = redoStack.Pop();
                        undoStack.Push(nextState);
                        canvas.SetShapesState(nextState);
                    }
                    break;

                case "8": // Выход
                    Environment.Exit(0);
                    break;

                case "9": // Список фигур
                    canvas.ListShapes();
                    break;

                default:
                    Console.WriteLine("Неверная команда.");
                    break;
            }
        }

        private void SaveState()
        {
            string shapesState = canvas.GetShapesState();
            undoStack.Push(shapesState);
            redoStack.Clear(); // Очищаем redo при новом действии
        }
    }
}
