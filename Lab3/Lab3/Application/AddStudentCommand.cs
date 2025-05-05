using Lab3.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Application
{
    public class AddStudentCommand: ICommand
    {
        private readonly StudentService _studentService;
        private readonly IQuoteService _quoteService;
        private readonly StudentDTO _studentDto;

        public AddStudentCommand(StudentService studentService, IQuoteService quoteService, StudentDTO studentDto)
        {
            _studentService = studentService;
            _quoteService = quoteService;
            _studentDto = studentDto;
        }

        public async void Execute()
        {
            _studentService.AddStudent(_studentDto);
            var quote = await _quoteService.GetMotivationalQuoteAsync();
            Console.WriteLine("!!!Student added!!!");
            Console.WriteLine($"Motivation quote: {quote.Content}");
        }
    }
}
