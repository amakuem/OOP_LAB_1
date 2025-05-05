using Lab3.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Application
{
    public class EditStudentCommand: ICommand
    {
        private readonly StudentService _studentService;
        private readonly StudentDTO _studentDto;
        private readonly int _id;

        public EditStudentCommand(StudentService studentService, StudentDTO studentDto)
        {
            _studentService = studentService;
            _studentDto = studentDto;
            
        }

        public void Execute()
        {
            _studentService.EditStudent(_studentDto);  //нужно ли id????????
        }
    }
}
