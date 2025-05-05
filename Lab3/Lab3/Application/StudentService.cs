using Lab3.DataAccess;
using Lab3.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Application
{
    public class StudentService: IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        public void AddStudent(StudentDTO studentDto)
        {
            var student = StudentFactory.Create(studentDto.Name, studentDto.Grade);
            _studentRepository.AddStudent(student);
        }
        public void EditStudent(StudentDTO studentDto)
        {
            

            var updatedStudent = StudentFactory.Create(studentDto.Name, studentDto.Grade);
            //updatedStudent.Id = id;
            _studentRepository.UpdateStudent(updatedStudent);
        }

        public List<Student> GetAllStudents()
        {
            //return _studentRepository.GetAllStudents().Select(s => new StudentDTO
            //{
            //    Name = s.Name,
            //    Grade = s.Grade
            //});
            return _studentRepository.GetAllStudents();
        }
    }
}
