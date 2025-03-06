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
                Console.WriteLine("4. Установить фон (укажите символ)");
                Console.WriteLine("5. Сохранить в файл");
                Console.WriteLine("6. Загрузить из файла");
                Console.WriteLine("7. Undo");
                Console.WriteLine("8. Redo");
                Console.WriteLine("9. Выход");
                Console.WriteLine("10. Показать список фигур"); // Новая команда
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
                    Console.Write("Тип фигуры (R - прямоугольник, C - круг): ");
                    char type = Console.ReadLine().ToUpper()[0];
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

                case "4": // Фон
                   // Console.Write("Символ фона: ");
                    //char bgSymbol = Console.ReadLine()[0];
                    //canvas.SetBackground(bgSymbol);
                    //SaveState();
                    break;

                case "5": // Сохранение
                    Console.Write("Имя файла: ");
                    string saveFile = Console.ReadLine();
                    File.WriteAllText(saveFile, canvas.GetState());
                    break;

                case "6": // Загрузка
                    Console.Write("Имя файла: ");
                    string loadFile = Console.ReadLine();
                    if (File.Exists(loadFile))
                    {
                        string state = File.ReadAllText(loadFile);
                        canvas.SetState(state);
                        SaveState();
                    }
                    else
                    {
                        Console.WriteLine("Файл не найден.");
                    }
                    break;

                case "7": // Undo
                    if (undoStack.Count > 1) // Оставляем текущее состояние
                    {
                        redoStack.Push(undoStack.Pop()); // Сохраняем текущее для redo
                        string prevState = undoStack.Peek();
                        canvas.SetState(prevState);
                    }
                    break;

                case "8": // Redo
                    if (redoStack.Count > 0)
                    {
                        string nextState = redoStack.Pop();
                        canvas.SetState(nextState);
                        undoStack.Push(nextState);
                    }
                    break;

                case "9": // Выход
                    Environment.Exit(0);
                    break;

                case "10": // Список фигур
                    canvas.ListShapes();
                    break;

                default:
                    Console.WriteLine("Неверная команда.");
                    break;
            }
        }

        private void SaveState()
        {
            undoStack.Push(canvas.GetState());
            redoStack.Clear(); // Очищаем redo при новом действии
        }
    }
}
