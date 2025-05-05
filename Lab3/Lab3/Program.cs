using Lab3.Application;
using Lab3.DataAccess;
using Lab3.Domain;
using System.ComponentModel.DataAnnotations;

namespace Lab3
{
    internal class Program
    {
        static private void PressAnyButton()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            // Инициализация зависимостей
            var httpClient = new HttpClient();
            var quoteService = new QuoteApiAdapter(httpClient);
            var studentRepository = new StudentRepository();
            var studentService = new StudentService(studentRepository);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n1. Add student");
                Console.WriteLine("2. Edit student");
                Console.WriteLine("3. View all students");
                Console.WriteLine("4. Exit");
                Console.Write("Choose the option: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Enter name: ");
                            string name = Console.ReadLine();
                            Console.Write("Enter grade: ");
                            int grade = int.Parse(Console.ReadLine());
                            var studentDto = new StudentDTO { Name = name, Grade = grade };
                            try
                            {
                                Lab3.Domain.Validator.ValidateNewStudent(studentDto);
                            }
                            catch(ValidationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                PressAnyButton();
                                break;
                            }
                        
                            var addCommand = new AddStudentCommand(studentService, quoteService, studentDto);
                            addCommand.Execute();
                            Console.WriteLine("!!!Student added!!!");
                            PressAnyButton();
                            break;

                        case "2":
                            Console.Write("Enter the name of student to edit: ");
                            string editName = Console.ReadLine();
                            Console.Write("Enter new grade: ");
                            int newGrade = int.Parse(Console.ReadLine());
                            var editDto = new StudentDTO { Name = editName, Grade = newGrade };
                            try
                            {
                                Lab3.Domain.Validator.ValidateNewStudent(editDto);
                            }
                            catch (ValidationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                PressAnyButton();
                                break;
                            }
                            
                            var editCommand = new EditStudentCommand(studentService, editDto);
                            try
                            {
                                editCommand.Execute();

                            }
                            catch(ValidationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                PressAnyButton();
                                break;
                            }
                            Console.WriteLine("!!!Student edited!!!");

                            break;

                        case "3":
                            var viewCommand = new ViewStudentCommand(studentService);
                            viewCommand.Execute();
                            PressAnyButton();
                            break;

                        case "4":
                            
                            return;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
