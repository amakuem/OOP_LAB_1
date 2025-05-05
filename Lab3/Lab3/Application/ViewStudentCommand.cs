using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Application
{
    public class ViewStudentCommand: ICommand
    {
        private readonly StudentService _studentService;

        public ViewStudentCommand(StudentService studentService)
        {
            _studentService = studentService;
        }

        public void Execute()
        {
            var students = _studentService.GetAllStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("The list of students is empty.");
            }
            else
            {
                foreach (var student in students)
                {
                    Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Grade: {student.Grade}");
                }
            }
        }
    }
}

