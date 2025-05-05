using Lab3.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Application
{
    public interface IStudentService
    {
        void AddStudent(StudentDTO studentDto);
        void EditStudent(StudentDTO studentDto);
        List<Student> GetAllStudents();
    }
}
